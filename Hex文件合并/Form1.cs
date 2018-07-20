using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;

namespace Hex文件合并
{
    public partial class Form1 : Form
    {
        //hex格式解析：<0x3a>[数据长度1Byte][数据地址2Byte][数据类型1Byte][数据nByte][校验1Byte]<0x0d><0x0a>
        /*
        '00' Data Record 数据
        '01' End of File Record 文件结束标志
        '02' Extended Segment Address Record 延伸段地址
        '03' Start Segment Address Record   起始延伸地址
        '04' Extended Linear Address Record 扩展线性地址 也就是基地址
        '05' Start Linear Address Record       程序起始地址也就是程序入口地址(main)
        0800 这个就是基地址(0x0800<<16)
         */
        struct DataLineMessage
        {
            public byte length;
            public UInt32 addr;
            public byte type;
            public UInt32 ExtAddr;//数据域
            public byte checksum;
        };  
        public Form1()
        {
            InitializeComponent();
            checkBox1.Checked = false;
            btn_help.Visible = false;
        }
        bool ParaseFileLine(string inoutstr,out DataLineMessage formatnew)
        {
            formatnew.length = 0;
            formatnew.type = 0;
            formatnew.addr = 0;
            formatnew.ExtAddr = 0;
            formatnew.checksum = 0;
            try
            {
                //DataLineMessage line=new DataLineMessage();
                byte[] data = HexStringToByteArray(inoutstr.Substring(1));
                if ((inoutstr.Substring(0, 1) != ":"))
                {
                    return false;
                }
                if(data.Length != 1+2+1+1+ data[0])
                {
                    return false;
                }
                //长度
                formatnew.length = data[0];
                //数据地址
                formatnew.addr = (UInt32)((data[1] << 8) | (data[2] << 0));
                //数据类型
                formatnew.type = data[3];

                if((formatnew.type<=0x05)&&(formatnew.type>=0x02))
                {
                    //扩展地址
                    if (formatnew.length==2)
                    {
                        formatnew.ExtAddr = (UInt32)((data[4] << 8) | (data[5] << 0));
                        formatnew.ExtAddr <<= 16;
                    }
                    else if(formatnew.length==4)
                    {
                        formatnew.ExtAddr = (UInt32)((data[4] << 8) | (data[5] << 0));
                        formatnew.ExtAddr |= (UInt32)((data[6] << 8) | (data[7] << 0));
                    }
                }
                formatnew.checksum =data[data.Length - 1];
                return true;
            }
            catch
            {
                return false;
            }
            finally
            {

            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            OpenFileDialog open_fd = new OpenFileDialog();
            open_fd.Multiselect = true;
            open_fd.Title = @"请选择文件";
            open_fd.Filter = @"所有文件(*.*)|*.*|HEX文件(*.hex)|*.hex";
            open_fd.FilterIndex = 2;
            if (open_fd.ShowDialog() == DialogResult.OK)
            {
                //Multiselect=true后  open_fd.FileNames是一个字符串数组
                FileStream filefd = null;
                StreamReader streamReader = null;

                if(open_fd.FileNames.Length<=2)
                {
                    DataLineMessage line = new DataLineMessage();
                    filefd = new FileStream(open_fd.FileNames[0], FileMode.Open, FileAccess.Read);
                    streamReader = new StreamReader(filefd, Encoding.Default);
                    filefd.Seek(0, SeekOrigin.Begin);
                    string content = streamReader.ReadLine();
                    if(ParaseFileLine(content,out line))
                    {
                        UInt32 BaseAddr = line.ExtAddr;
                        Debug.WriteLine("line.ExtAddr=0x{0:x8}", BaseAddr);//基地址                 
                        content = streamReader.ReadLine();
                        if(ParaseFileLine(content,out line))
                        {
                            BaseAddr |= line.addr;//加上偏移地址
                            if (BaseAddr == 0x08000000)
                            {
                                //IsBootLoader = true;
                                textBox1.Text = open_fd.FileNames[0];
                                if (open_fd.FileNames.Length == 2)
                                    textBox2.Text = open_fd.FileNames[1];
                            }
                            else
                            {
                                textBox2.Text = open_fd.FileNames[0];
                                if (open_fd.FileNames.Length == 2)
                                    textBox1.Text = open_fd.FileNames[1];
                            }
                            textOutPath.Text = GetNewPathForDupes(System.IO.Path.GetDirectoryName(open_fd.FileNames[0]) + @"\merge.hex");
                        }
                        else
                        {
                            textBox1.Text = "";
                            textBox2.Text = "";
                            MessageBox.Show("file is not correct", "error");
                        }
                    }
                    else
                    {
                        textBox1.Text = "";
                        textBox2.Text = "";
                        MessageBox.Show("file is not correct", "error");
                    }
                    if (filefd != null)
                    {
                        filefd.Close();
                    }
                    if (streamReader != null)
                    {
                        streamReader.Close();
                    } 
                }
                else
                {
                    MessageBox.Show("there are too many files","error");
                }
                
            }
        }
        private void btn_merge_Click(object sender, EventArgs e)
        {
            string str1, str2;
            str1 = textBox1.Text.Trim();
            str2 = textBox2.Text.Trim();
            if (str1 == str2)
            {
                MessageBox.Show("please select diffirent file", "error");
                return;
            }
            if(str1=="")
            {
                MessageBox.Show("please select bootloader file", "error");
                return;
            }
            if (str2 == "")
            {
                MessageBox.Show("please select app file", "error");
                return;
            }
            Debug.WriteLine(str1);
            Debug.WriteLine(str2);
            StreamReader fileReader=null;
            StreamWriter Newfile = null;
            
            try
            {
                DataLineMessage line = new DataLineMessage();
                textOutPath.Text = GetNewPathForDupes(textOutPath.Text);

                //读取文件一
                fileReader = new StreamReader(str1);
                Newfile = new StreamWriter(textOutPath.Text);
                string strline = null;
                UInt32 Nowaddr=0;
                do
                {
                    strline = fileReader.ReadLine();
                    if (ParaseFileLine(strline, out line))
                    {

                        if(line.type==0x01)//结束标志
                        {
                            break;
                        }
                        else if (line.type == 0x05)//入口地址
                        {
                            if(checkBox1.Checked)
                            {
                                if (Nowaddr <= 0x2ff8)
                                {
                                    Newfile.WriteLine(":022FFA00A5A58B");//往地址（基地址+0x2FFA）写入0xA5A5
                                }
                                else
                                {
                                    checkBox1.Checked = false;
                                    MessageBox.Show("bootloader过大，不支持特殊字节写入");
                                }
                                
                            }
                            Newfile.WriteLine(strline);
                        }
                        else//数据等
                        {
                            Nowaddr = line.addr;
                            Newfile.WriteLine(strline);
                        }
                    }
                    else
                    {
                        MessageBox.Show("There have some error in file","merge faild");
                        return;
                    }

                } while (strline != null);
                //读取文件二 Newfile.WriteLine
                fileReader = new StreamReader(str2);
                do
                {
                    strline = fileReader.ReadLine();
                    Newfile.WriteLine(strline);
                } while (strline != null);
                MessageBox.Show("merge successful");
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
            finally
            {
                if (fileReader != null)
                {
                    fileReader.Close();
                }
                if (Newfile != null)
                {
                    Newfile.Close();
                } 
            }
        }

        #region 字符串转换函数
        //翻转byte数组
        public static void ReverseBytes(byte[] bytes)
        {
            byte tmp;
            int len = bytes.Length;

            for (int i = 0; i < len / 2; i++)
            {
                tmp = bytes[len - 1 - i];
                bytes[len - 1 - i] = bytes[i];
                bytes[i] = tmp;
            }
        }
        //规定转换起始位置和长度
        public static void ReverseBytes(byte[] bytes, int start, int len)
        {
            int end = start + len - 1;
            byte tmp;
            int i = 0;
            for (int index = start; index < start + len / 2; index++, i++)
            {
                tmp = bytes[end - i];
                bytes[end - i] = bytes[index];
                bytes[index] = tmp;
            }
        }

        // 翻转字节顺序 (16-bit)
        public static UInt16 ReverseBytes(UInt16 value)
        {
            return (UInt16)((value & 0xFFU) << 8 | (value & 0xFF00U) >> 8);
        }


        // 翻转字节顺序 (32-bit)
        public static UInt32 ReverseBytes(UInt32 value)
        {
            return (value & 0x000000FFU) << 24 | (value & 0x0000FF00U) << 8 |
                   (value & 0x00FF0000U) >> 8 | (value & 0xFF000000U) >> 24;
        }
        // 翻转字节顺序 (64-bit)
        public static UInt64 ReverseBytes(UInt64 value)
        {
            return (value & 0x00000000000000FFUL) << 56 | (value & 0x000000000000FF00UL) << 40 |
                   (value & 0x0000000000FF0000UL) << 24 | (value & 0x00000000FF000000UL) << 8 |
                   (value & 0x000000FF00000000UL) >> 8 | (value & 0x0000FF0000000000UL) >> 24 |
                   (value & 0x00FF000000000000UL) >> 40 | (value & 0xFF00000000000000UL) >> 56;
        }
        public string ByteArrayToHexString(byte[] data)
        {
            StringBuilder sb = new StringBuilder(data.Length * 3);
            foreach (byte b in data)
                sb.Append(Convert.ToString(b, 16).PadLeft(2, '0'));
            return sb.ToString().ToUpper();
        }

        public byte[] HexStringToByteArray(string s)
        {
            s = s.Replace(" ", "");
            if (s.Length % 2 != 0)
            {
                s = s.Substring(0, s.Length - 1) + "0" + s.Substring(s.Length - 1);
            }
            byte[] buffer = new byte[s.Length / 2];

            try
            {
                for (int i = 0; i < s.Length; i += 2)
                    buffer[i / 2] = (byte)Convert.ToByte(s.Substring(i, 2), 16);
                return buffer;
            }
            catch
            {
                string errorString = "E4";
                byte[] errorData = new byte[errorString.Length / 2];
                errorData[0] = (byte)Convert.ToByte(errorString, 16);
                return errorData;
            }
        }

        public string StringToHexString(string s)
        {
            s = s.Replace(" ", "");
            string buffer = "";
            char[] myChar;
            myChar = s.ToCharArray();
            for (int i = 0; i < s.Length; i++)
            {
                buffer = buffer + Convert.ToString(myChar[i], 16);
                buffer = buffer.ToUpper();
            }
            return buffer;
        }
        #endregion
        /// <summary>
        /// Generates a new path for duplicate filenames.
        /// </summary>
        /// <param name="path">The path.</param>
        /// <returns></returns>
        private string GetNewPathForDupes(string path)
        {
            string newFullPath = path.Trim();
            //if (System.IO.File.Exists(path))
            //    MessageBox.Show("存在");
            //else
            //    MessageBox.Show("不存在");
            if (System.IO.File.Exists(path))
            {
                string directory = Path.GetDirectoryName(path);
                string filename = Path.GetFileNameWithoutExtension(path);
                string extension = Path.GetExtension(path);
                int counter = 1;
                do
                {
                    string newFilename = string.Format("{0}({1}){2}", filename, counter, extension);
                    newFullPath = Path.Combine(directory, newFilename);
                    counter++;
                } while (System.IO.File.Exists(newFullPath));
            }
            return newFullPath;
        }
        private void btn_outpath_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveImageDialog = new SaveFileDialog();
            saveImageDialog.FileName = "merge";
            saveImageDialog.Title = "保存";
            saveImageDialog.Filter = @"HEX文件|*.hex";
            if (saveImageDialog.ShowDialog() == DialogResult.OK)
            {
                string fileName = saveImageDialog.FileName.ToString();
                textOutPath.Text = fileName;
            }
        }

        private void btn_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("在起始地址偏移0X2FFA的位置写入字节0xA5A5\r\n因此需要使用此功能时Bootloader大小不应超过0x2FFA Byte\r\n", "写入特殊值说明");
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            //Debug.WriteLine("checkBox1_CheckedChanged");
            if(checkBox1.Checked==true)
            {
                btn_help.Visible=true;
            }
            else
            {
                btn_help.Visible = false;
            }
            
        }

    }
}
