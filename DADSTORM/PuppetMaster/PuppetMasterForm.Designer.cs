namespace PuppetMaster
{
    partial class PuppetMasterForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this.button3 = new System.Windows.Forms.Button();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.button11 = new System.Windows.Forms.Button();
            this.logFilesTreeView = new System.Windows.Forms.TreeView();
            this.button9 = new System.Windows.Forms.Button();
            this.ConfigFilestreeView = new System.Windows.Forms.TreeView();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.button5 = new System.Windows.Forms.Button();
            this.button4 = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.button10 = new System.Windows.Forms.Button();
            this.ReplicaLabel = new System.Windows.Forms.Label();
            this.replicasList = new System.Windows.Forms.ComboBox();
            this.button7 = new System.Windows.Forms.Button();
            this.button8 = new System.Windows.Forms.Button();
            this.button6 = new System.Windows.Forms.Button();
            this.OperatorLabel = new System.Windows.Forms.Label();
            this.operatorList = new System.Windows.Forms.ComboBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.logText = new System.Windows.Forms.TextBox();
            this.button12 = new System.Windows.Forms.Button();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(6, 46);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(93, 23);
            this.button1.TabIndex = 0;
            this.button1.Text = "Next";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(105, 46);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(96, 23);
            this.button2.TabIndex = 1;
            this.button2.Text = "Execute All";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // textBox1
            // 
            this.textBox1.Location = new System.Drawing.Point(6, 20);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(149, 20);
            this.textBox1.TabIndex = 2;
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(161, 20);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(40, 20);
            this.button3.TabIndex = 3;
            this.button3.Text = "Send Command";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.textBox1);
            this.groupBox1.Controls.Add(this.button2);
            this.groupBox1.Controls.Add(this.button3);
            this.groupBox1.Controls.Add(this.button1);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(212, 85);
            this.groupBox1.TabIndex = 4;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Operations";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.button9);
            this.groupBox2.Controls.Add(this.ConfigFilestreeView);
            this.groupBox2.Location = new System.Drawing.Point(230, 12);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(147, 164);
            this.groupBox2.TabIndex = 5;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "ConfigFiles";
            // 
            // button11
            // 
            this.button11.Location = new System.Drawing.Point(79, 117);
            this.button11.Name = "button11";
            this.button11.Size = new System.Drawing.Size(62, 22);
            this.button11.TabIndex = 3;
            this.button11.Text = "Read";
            this.button11.UseVisualStyleBackColor = true;
            this.button11.Click += new System.EventHandler(this.button11_Click);
            // 
            // logFilesTreeView
            // 
            this.logFilesTreeView.Location = new System.Drawing.Point(6, 16);
            this.logFilesTreeView.Name = "logFilesTreeView";
            this.logFilesTreeView.Size = new System.Drawing.Size(135, 99);
            this.logFilesTreeView.TabIndex = 2;
            // 
            // button9
            // 
            this.button9.Location = new System.Drawing.Point(6, 134);
            this.button9.Name = "button9";
            this.button9.Size = new System.Drawing.Size(135, 24);
            this.button9.TabIndex = 1;
            this.button9.Text = "Start";
            this.button9.UseVisualStyleBackColor = true;
            this.button9.Click += new System.EventHandler(this.button9_Click);
            // 
            // ConfigFilestreeView
            // 
            this.ConfigFilestreeView.Location = new System.Drawing.Point(6, 19);
            this.ConfigFilestreeView.Name = "ConfigFilestreeView";
            this.ConfigFilestreeView.Size = new System.Drawing.Size(135, 114);
            this.ConfigFilestreeView.TabIndex = 0;
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.button5);
            this.groupBox4.Controls.Add(this.button4);
            this.groupBox4.Location = new System.Drawing.Point(12, 260);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(212, 65);
            this.groupBox4.TabIndex = 7;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "BroadCast Operations";
            // 
            // button5
            // 
            this.button5.Location = new System.Drawing.Point(116, 20);
            this.button5.Name = "button5";
            this.button5.Size = new System.Drawing.Size(85, 33);
            this.button5.TabIndex = 1;
            this.button5.Text = "Status";
            this.button5.UseVisualStyleBackColor = true;
            this.button5.Click += new System.EventHandler(this.button5_Click);
            // 
            // button4
            // 
            this.button4.Location = new System.Drawing.Point(18, 20);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(81, 33);
            this.button4.TabIndex = 0;
            this.button4.Text = "CrashAll";
            this.button4.UseVisualStyleBackColor = true;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.button10);
            this.groupBox5.Controls.Add(this.ReplicaLabel);
            this.groupBox5.Controls.Add(this.replicasList);
            this.groupBox5.Controls.Add(this.button7);
            this.groupBox5.Controls.Add(this.button8);
            this.groupBox5.Controls.Add(this.button6);
            this.groupBox5.Controls.Add(this.OperatorLabel);
            this.groupBox5.Controls.Add(this.operatorList);
            this.groupBox5.Location = new System.Drawing.Point(12, 103);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(212, 151);
            this.groupBox5.TabIndex = 8;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Generic Operations";
            // 
            // button10
            // 
            this.button10.Location = new System.Drawing.Point(136, 12);
            this.button10.Name = "button10";
            this.button10.Size = new System.Drawing.Size(65, 29);
            this.button10.TabIndex = 7;
            this.button10.Text = "Start";
            this.button10.UseVisualStyleBackColor = true;
            this.button10.Click += new System.EventHandler(this.button10_Click);
            // 
            // ReplicaLabel
            // 
            this.ReplicaLabel.AutoSize = true;
            this.ReplicaLabel.Location = new System.Drawing.Point(7, 73);
            this.ReplicaLabel.Name = "ReplicaLabel";
            this.ReplicaLabel.Size = new System.Drawing.Size(43, 13);
            this.ReplicaLabel.TabIndex = 6;
            this.ReplicaLabel.Text = "Replica";
            // 
            // replicasList
            // 
            this.replicasList.FormattingEnabled = true;
            this.replicasList.Location = new System.Drawing.Point(6, 94);
            this.replicasList.Name = "replicasList";
            this.replicasList.Size = new System.Drawing.Size(104, 21);
            this.replicasList.TabIndex = 5;
            // 
            // button7
            // 
            this.button7.Location = new System.Drawing.Point(136, 46);
            this.button7.Name = "button7";
            this.button7.Size = new System.Drawing.Size(65, 29);
            this.button7.TabIndex = 3;
            this.button7.Text = "Freeze";
            this.button7.UseVisualStyleBackColor = true;
            this.button7.Click += new System.EventHandler(this.button7_Click);
            // 
            // button8
            // 
            this.button8.Location = new System.Drawing.Point(136, 81);
            this.button8.Name = "button8";
            this.button8.Size = new System.Drawing.Size(65, 29);
            this.button8.TabIndex = 4;
            this.button8.Text = "Unfreeze";
            this.button8.UseVisualStyleBackColor = true;
            this.button8.Click += new System.EventHandler(this.button8_Click);
            // 
            // button6
            // 
            this.button6.Location = new System.Drawing.Point(136, 116);
            this.button6.Name = "button6";
            this.button6.Size = new System.Drawing.Size(65, 29);
            this.button6.TabIndex = 2;
            this.button6.Text = "Crash";
            this.button6.UseVisualStyleBackColor = true;
            this.button6.Click += new System.EventHandler(this.button6_Click);
            // 
            // OperatorLabel
            // 
            this.OperatorLabel.AutoSize = true;
            this.OperatorLabel.Location = new System.Drawing.Point(7, 20);
            this.OperatorLabel.Name = "OperatorLabel";
            this.OperatorLabel.Size = new System.Drawing.Size(48, 13);
            this.OperatorLabel.TabIndex = 1;
            this.OperatorLabel.Text = "Operator";
            // 
            // operatorList
            // 
            this.operatorList.FormattingEnabled = true;
            this.operatorList.Location = new System.Drawing.Point(6, 36);
            this.operatorList.Name = "operatorList";
            this.operatorList.Size = new System.Drawing.Size(104, 21);
            this.operatorList.TabIndex = 0;
            this.operatorList.SelectedIndexChanged += new System.EventHandler(this.operatorList_SelectedIndexChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.logText);
            this.groupBox3.Location = new System.Drawing.Point(383, 22);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(373, 296);
            this.groupBox3.TabIndex = 9;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "CurrentLog";
            // 
            // logText
            // 
            this.logText.Location = new System.Drawing.Point(6, 19);
            this.logText.Multiline = true;
            this.logText.Name = "logText";
            this.logText.ReadOnly = true;
            this.logText.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.logText.Size = new System.Drawing.Size(361, 271);
            this.logText.TabIndex = 0;
            // 
            // button12
            // 
            this.button12.Location = new System.Drawing.Point(6, 117);
            this.button12.Name = "button12";
            this.button12.Size = new System.Drawing.Size(67, 22);
            this.button12.TabIndex = 4;
            this.button12.Text = "Refresh";
            this.button12.UseVisualStyleBackColor = true;
            this.button12.Click += new System.EventHandler(this.button12_Click);
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.logFilesTreeView);
            this.groupBox6.Controls.Add(this.button11);
            this.groupBox6.Controls.Add(this.button12);
            this.groupBox6.Location = new System.Drawing.Point(230, 179);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(147, 146);
            this.groupBox6.TabIndex = 10;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "LogFiles";
            // 
            // PuppetMasterForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(767, 333);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Name = "PuppetMasterForm";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.PuppetMasterForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.TreeView ConfigFilestreeView;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button button5;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.ComboBox operatorList;
        private System.Windows.Forms.Button button8;
        private System.Windows.Forms.Button button7;
        private System.Windows.Forms.Button button6;
        private System.Windows.Forms.Button button9;
        private System.Windows.Forms.ComboBox replicasList;
        private System.Windows.Forms.Label ReplicaLabel;
        private System.Windows.Forms.Label OperatorLabel;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.TextBox logText;
        private System.Windows.Forms.Button button10;
        private System.Windows.Forms.Button button11;
        private System.Windows.Forms.TreeView logFilesTreeView;
        private System.Windows.Forms.Button button12;
        private System.Windows.Forms.GroupBox groupBox6;
    }
}

