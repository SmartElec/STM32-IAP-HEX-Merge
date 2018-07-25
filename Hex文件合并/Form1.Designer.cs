namespace Hex文件合并
{
    partial class Form1
    {
        /// <summary>
        /// 必需的设计器变量。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// 清理所有正在使用的资源。
        /// </summary>
        /// <param name="disposing">如果应释放托管资源，为 true；否则为 false。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows 窗体设计器生成的代码

        /// <summary>
        /// 设计器支持所需的方法 - 不要
        /// 使用代码编辑器修改此方法的内容。
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Form1));
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.btn_merge = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.textOutPath = new System.Windows.Forms.TextBox();
            this.btn_outpath = new System.Windows.Forms.Button();
            this.checkBox1 = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.btn_help = new System.Windows.Forms.Button();
            this.checkBox2 = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.SuspendLayout();
            // 
            // textBox1
            // 
            this.textBox1.Enabled = false;
            this.textBox1.Location = new System.Drawing.Point(6, 20);
            this.textBox1.Multiline = true;
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(269, 29);
            this.textBox1.TabIndex = 0;
            // 
            // textBox2
            // 
            this.textBox2.Enabled = false;
            this.textBox2.Location = new System.Drawing.Point(6, 65);
            this.textBox2.Multiline = true;
            this.textBox2.Name = "textBox2";
            this.textBox2.Size = new System.Drawing.Size(269, 28);
            this.textBox2.TabIndex = 1;
            // 
            // btn_merge
            // 
            this.btn_merge.Location = new System.Drawing.Point(316, 217);
            this.btn_merge.Name = "btn_merge";
            this.btn_merge.Size = new System.Drawing.Size(53, 35);
            this.btn_merge.TabIndex = 2;
            this.btn_merge.Text = "合并";
            this.btn_merge.UseVisualStyleBackColor = true;
            this.btn_merge.Click += new System.EventHandler(this.btn_merge_Click);
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(281, 20);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(66, 73);
            this.button1.TabIndex = 3;
            this.button1.Text = "选择文件";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // textOutPath
            // 
            this.textOutPath.Location = new System.Drawing.Point(6, 20);
            this.textOutPath.Multiline = true;
            this.textOutPath.Name = "textOutPath";
            this.textOutPath.Size = new System.Drawing.Size(269, 30);
            this.textOutPath.TabIndex = 4;
            // 
            // btn_outpath
            // 
            this.btn_outpath.Location = new System.Drawing.Point(285, 20);
            this.btn_outpath.Name = "btn_outpath";
            this.btn_outpath.Size = new System.Drawing.Size(66, 30);
            this.btn_outpath.TabIndex = 5;
            this.btn_outpath.Text = "修改目录";
            this.btn_outpath.UseVisualStyleBackColor = true;
            this.btn_outpath.Click += new System.EventHandler(this.btn_outpath_Click);
            // 
            // checkBox1
            // 
            this.checkBox1.AutoSize = true;
            this.checkBox1.Location = new System.Drawing.Point(12, 221);
            this.checkBox1.Name = "checkBox1";
            this.checkBox1.Size = new System.Drawing.Size(72, 28);
            this.checkBox1.TabIndex = 6;
            this.checkBox1.Text = "合并时写\r\n入特殊值\r\n";
            this.checkBox1.UseVisualStyleBackColor = true;
            this.checkBox1.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.textBox2);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(357, 107);
            this.groupBox1.TabIndex = 7;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "需要合并的hex文件 for stm32";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.textOutPath);
            this.groupBox2.Controls.Add(this.btn_outpath);
            this.groupBox2.Location = new System.Drawing.Point(12, 134);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(358, 75);
            this.groupBox2.TabIndex = 8;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "输出目录选择";
            // 
            // btn_help
            // 
            this.btn_help.Location = new System.Drawing.Point(90, 220);
            this.btn_help.Name = "btn_help";
            this.btn_help.Size = new System.Drawing.Size(62, 29);
            this.btn_help.TabIndex = 9;
            this.btn_help.Text = "帮助";
            this.btn_help.UseVisualStyleBackColor = true;
            this.btn_help.Click += new System.EventHandler(this.btn_help_Click);
            // 
            // checkBox2
            // 
            this.checkBox2.AutoSize = true;
            this.checkBox2.Location = new System.Drawing.Point(186, 227);
            this.checkBox2.Name = "checkBox2";
            this.checkBox2.Size = new System.Drawing.Size(126, 16);
            this.checkBox2.TabIndex = 10;
            this.checkBox2.Text = "合并后生成bin文件";
            this.checkBox2.UseVisualStyleBackColor = true;
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 12F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(385, 266);
            this.Controls.Add(this.checkBox2);
            this.Controls.Add(this.btn_help);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkBox1);
            this.Controls.Add(this.btn_merge);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "Form1";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "HEX合并 for stm32 IAP";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Button btn_merge;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.TextBox textOutPath;
        private System.Windows.Forms.Button btn_outpath;
        private System.Windows.Forms.CheckBox checkBox1;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Button btn_help;
        private System.Windows.Forms.CheckBox checkBox2;
    }
}

