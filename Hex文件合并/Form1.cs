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
            //允许拖拽
            this.AllowDrop = true;
            this.DragEnter += new DragEventHandler(Form1_DragEnter);
            this.DragDrop += new DragEventHandler(Form1_DragDrop);

            btn_help.Visible = true;
            textOutPath.Text = Application.StartupPath + @"/merge.hex";
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
                            //textOutPath.Text = GetNewPathForDupes(System.IO.Path.GetDirectoryName(open_fd.FileNames[0]) + @"\merge.hex");
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
            if ((str1 == str2) && (str1==""))
            {
                MessageBox.Show("please select one file at least", "error");
                return;
            }

            StreamReader fileReader=null;
            StreamWriter Newfile = null;
            DataLineMessage line = new DataLineMessage();
            string savepath = GetNewPathForDupes(textOutPath.Text);
            //if (savepath==null)
            //{
            //    savepath = GetNewPathForDupes(Application.StartupPath + @"/merge.hex");
            //}
            try
            {
                if(str1=="")
                {
                    File.Copy(str2, savepath);
                    if (checkBox2.Checked == true)//确定转换为bin文件
                    {
                        ReadHexFile(savepath);
                        MessageBox.Show("已保存为\n\r" + savepath);
                    }
                    
                    return;
                }
                else if (str2 == "")
                {
                    if (checkBox2.Checked == true)//确定是否合并文件
                    {
                        ReadHexFile(savepath);
                        MessageBox.Show("已保存为\n\r" + savepath);
                    }
                    
                    return;
                }
                //读取文件一
                fileReader = new StreamReader(str1);
                Newfile = new StreamWriter(savepath);
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
                            Newfile.WriteLine(strline);//继续保存该行数据
                        }
                        else//数据等
                        {
                            Nowaddr = line.addr;
                            Newfile.WriteLine(strline);//保存数据
                        }
                    }
                    else
                    {
                        fileReader.Close();
                        Newfile.Close();
                        MessageBox.Show("There have some error in file","merge faild");
                        return;
                    }

                } while (strline != null);
                //读取文件二
                fileReader = new StreamReader(str2);
                do
                {
                    strline = fileReader.ReadLine();
                    Newfile.WriteLine(strline);
                } while (strline != null);

                fileReader.Close();
                Newfile.Close();

                MessageBox.Show("merge successful");
                if (checkBox2.Checked == true)//确定是否合并文件
                {
                    ReadHexFile(savepath);//转换成bin
                }
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
		#region HEX2BIN	
        public bool ReadHexFile(string fileName)
        {
            if (fileName == null || fileName.Trim() == "")  //文件存在
            {
                return false;
            }
            using (FileStream fs = new FileStream(fileName, FileMode.Open))  //open file
            {
                StreamReader HexReader = new StreamReader(fs);    //读取数据流
                string szLine = "";
                string szHex = "";
                string szAddress = "";
                string szLength = "";

                while (true)
                {
                    szLine = HexReader.ReadLine();      //读取Hex中一行
                    if(szLine==null)
                    {
                        break;//读取完毕，退出
                    }
                    int datatype = Parase_HexLineData(szLine);
                    if (datatype < 0)
                    {
                        MessageBox.Show("文件格式有误");
                        break;//解析失败
                    }
                   // if (szLine.Substring(0, 1) == ":")    //判断首字符是":"
                    {
                        if (datatype==1) { break; }  //文件结束标识
                        //直接解析数据类型标识为 : 00 和 01 的格式
                        if (datatype == 0)
                        {
                            szHex += szLine.Substring(9, szLine.Length - 11);  //所有数据分一组 
                            szAddress += szLine.Substring(3, 4); //所有起始地址分一组
                            szLength += szLine.Substring(1, 2); //所有数据长度归一组
                        }
                    }
                }
                //将数据字符转换为Hex，并保存在数组 szBin[]
                Int32 j = 0;
                Int32 Length = szHex.Length;      //获取长度
                byte[] szBin = new byte[Length / 2];
                for (Int32 i = 0; i < Length; i += 2)
                {
                    szBin[j] = (byte)Int16.Parse(szHex.Substring(i, 2), System.Globalization.NumberStyles.HexNumber);   //两个字符合并一个Hex 
                    j++;
                }
                //将起始地址的字符转换为Hex，并保存在数组 szAdd []
                j = 0;
                Length = szAddress.Length;      //get bytes number of szAddress
                Int32[] szAdd = new Int32[Length / 4];
                for (Int32 i = 0; i < Length; i += 4)
                {
                    szAdd[j] = Int32.Parse(szAddress.Substring(i, 4), System.Globalization.NumberStyles.HexNumber);  //两个字符合并一个Hex
                    j++;
                }
                //将长度字符转换为Hex，并保存在数组 szLen []
                j = 0;
                Length = szLength.Length;      //get bytes number of szAddress
                byte[] szLen = new byte[Length / 2];
                for (Int32 i = 0; i < Length; i += 2)
                {
                    szLen[j] = (byte)Int16.Parse(szLength.Substring(i, 2), System.Globalization.NumberStyles.HexNumber); // merge two bytes to a hex number
                    j++;
                }
                array_save_data(szAdd, szBin, szLen, Path.ChangeExtension(fileName, "bin"));   //保存为bin
            }
            return true;
        }
        private void array_save_data(Int32[] address, byte[] data, byte[] length,string Path)
        {
            //1.整理数据所需参数
            int jcount = 0;
            int max_address = (address[(address.Length) - 1]) + 16;
            byte[] all_show_data = new byte[max_address];  //存储解析完成的数据数组
            for (Int32 i = 0; i < all_show_data.Length; i++) { all_show_data[i] = 255; }  //默认全为0

            //2.从address[]数组中获取HEX对应地址
            for (Int32 i = 0; i < address.Length; i++)
            {
                if (i >= 1) { jcount += length[i - 1]; }  //从length[]数组中获取数据对应的长度大小
                for (int j = 0; j < length[i]; j++)
                {
                    all_show_data[address[i] + j] = data[jcount + j]; //all_show_data[]数组中添加数据
                }
            }
            FileStream fs = new FileStream(Path, FileMode.Create);
            fs.Write(all_show_data, 0, all_show_data.Length);
            fs.Close();
        }
		#endregion 
        int Parase_HexLineData(string szLine)//返回数据类型01-05
        {
            //冒号 本行数据长度(1byte) 本行数据的起始地址(2byte) 数据类型(1byte) 数据(N byte) 校验码(1byte)
            int Len = szLine.Length - 1;

            if (Len < 10) { return -1; }

            try
            {
                if (Len % 2 == 0)
                {       
                    if (szLine.Substring(0, 1) == ":")    //判断首字符是":"
                    {
                        byte[] BytesData = new byte[Len / 2];  //声明一个长度为hexstring长度一半的字节组
                        byte checksum = 0;
          
                        for (int i = 0; i < BytesData.Length; i++)
                        {
                            BytesData[i] = Convert.ToByte(szLine.Substring(i * 2 + 1, 2), 16);  //将hexstring的两个字符转换成16进制的字节组            
                        }
                        if (BytesData[0] != (BytesData.Length-5))//长度域与实际长度不符
                       {
                           return -1;
                       }

                        for (int i = 0; i < BytesData.Length -1; i++)
                        {
                            checksum+=BytesData[i];
                        }
                        if(checksum==(byte)(0-BytesData[BytesData.Length-1]))
                        {
                                return BytesData[3];
                        }
                    }
                }
            }
            catch
            {

            }

            return -1;
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
                return newFullPath;
            }
            return path;
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
        void Form1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)) e.Effect = DragDropEffects.All;
            else e.Effect = DragDropEffects.None;
        }
        void Form1_DragDrop(object sender, DragEventArgs e)
        {
            //获取第一个文件名
            string fileName = (e.Data.GetData(DataFormats.FileDrop, false) as String[])[0];

            OpenDropFile(fileName);
        }
        void OpenDropFile(string file1)
        {
            FileStream filefd = null;
            StreamReader streamReader = null;
            DataLineMessage line = new DataLineMessage();
            filefd = new FileStream(file1, FileMode.Open, FileAccess.Read);
            streamReader = new StreamReader(filefd, Encoding.Default);
            filefd.Seek(0, SeekOrigin.Begin);
            string content = streamReader.ReadLine();
            if (ParaseFileLine(content, out line))
            {
                UInt32 BaseAddr = line.ExtAddr;
                Debug.WriteLine("line.ExtAddr=0x{0:x8}", BaseAddr);//基地址                 
                content = streamReader.ReadLine();
                if (ParaseFileLine(content, out line))
                {
                    BaseAddr |= line.addr;//加上偏移地址
                    if (BaseAddr == 0x08000000)
                    {
                        textBox1.Text = file1;
                    }
                    else
                    {
                        textBox2.Text = file1;
                    }
                }
                else
                {
                    MessageBox.Show("file is not correct", "error");
                }
            }
            else
            {
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
        private void btn_help_Click(object sender, EventArgs e)
        {
            MessageBox.Show("可拖拽hex文件到对话框\r\n合并时自动判断有几个文件\r\n勾选\"生成bin\"自动将一个或多个hex文件转换为一个bin", "说明");
        }

    }
}
