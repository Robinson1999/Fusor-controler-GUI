namespace Fusor_controler_GUI
{
    partial class Form1
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
            this.btnEXIT = new System.Windows.Forms.Button();
            this.btnHVon = new System.Windows.Forms.Button();
            this.btnForm2 = new System.Windows.Forms.Button();
            this.button3 = new System.Windows.Forms.Button();
            this.label1 = new System.Windows.Forms.Label();
            this.lblMainsCurrent = new System.Windows.Forms.Label();
            this.lbl12V = new System.Windows.Forms.Label();
            this.lbl5V2 = new System.Windows.Forms.Label();
            this.lblMainsVoltage = new System.Windows.Forms.Label();
            this.lblHV = new System.Windows.Forms.Label();
            this.btnHVoff = new System.Windows.Forms.Button();
            this.lblHVstatus = new System.Windows.Forms.Label();
            this.listBox1 = new System.Windows.Forms.ListBox();
            this.lblSafe = new System.Windows.Forms.Label();
            this.button1 = new System.Windows.Forms.Button();
            this.lblWAPIN = new System.Windows.Forms.Label();
            this.lblSAPIN = new System.Windows.Forms.Label();
            this.lblCheckSpeed = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.lblComSpeed = new System.Windows.Forms.Label();
            this.button4 = new System.Windows.Forms.Button();
            this.lblTime = new System.Windows.Forms.Label();
            this.lblCPM5 = new System.Windows.Forms.Label();
            this.lblCPM15 = new System.Windows.Forms.Label();
            this.lblCPM30 = new System.Windows.Forms.Label();
            this.lblCPM60 = new System.Windows.Forms.Label();
            this.lblSV5 = new System.Windows.Forms.Label();
            this.lblSV15 = new System.Windows.Forms.Label();
            this.lblSV30 = new System.Windows.Forms.Label();
            this.lblSV60 = new System.Windows.Forms.Label();
            this.lblTemp = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnEXIT
            // 
            this.btnEXIT.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.btnEXIT.Location = new System.Drawing.Point(15, 12);
            this.btnEXIT.Name = "btnEXIT";
            this.btnEXIT.Size = new System.Drawing.Size(100, 75);
            this.btnEXIT.TabIndex = 0;
            this.btnEXIT.Text = "EXIT";
            this.btnEXIT.UseVisualStyleBackColor = false;
            this.btnEXIT.Click += new System.EventHandler(this.button1_Click);
            // 
            // btnHVon
            // 
            this.btnHVon.BackColor = System.Drawing.Color.Red;
            this.btnHVon.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnHVon.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHVon.ForeColor = System.Drawing.SystemColors.ControlText;
            this.btnHVon.Location = new System.Drawing.Point(472, 12);
            this.btnHVon.Name = "btnHVon";
            this.btnHVon.Size = new System.Drawing.Size(200, 100);
            this.btnHVon.TabIndex = 1;
            this.btnHVon.Text = "HIGH VOLTAGE ON";
            this.btnHVon.UseVisualStyleBackColor = false;
            this.btnHVon.Click += new System.EventHandler(this.SPI_Click);
            // 
            // btnForm2
            // 
            this.btnForm2.BackColor = System.Drawing.SystemColors.HighlightText;
            this.btnForm2.Enabled = false;
            this.btnForm2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnForm2.Location = new System.Drawing.Point(221, 450);
            this.btnForm2.Name = "btnForm2";
            this.btnForm2.Size = new System.Drawing.Size(97, 69);
            this.btnForm2.TabIndex = 2;
            this.btnForm2.Text = "Form2\r\nCalibration data";
            this.btnForm2.UseVisualStyleBackColor = false;
            this.btnForm2.Visible = false;
            this.btnForm2.Click += new System.EventHandler(this.BtnForm2_Click);
            // 
            // button3
            // 
            this.button3.BackColor = System.Drawing.SystemColors.HighlightText;
            this.button3.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button3.Location = new System.Drawing.Point(221, 343);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(150, 104);
            this.button3.TabIndex = 3;
            this.button3.Text = "Reset error log";
            this.button3.UseVisualStyleBackColor = false;
            this.button3.Click += new System.EventHandler(this.Button3_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.White;
            this.label1.Location = new System.Drawing.Point(488, 450);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(110, 16);
            this.label1.TabIndex = 6;
            this.label1.Text = "STARTING UP";
            // 
            // lblMainsCurrent
            // 
            this.lblMainsCurrent.AutoSize = true;
            this.lblMainsCurrent.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainsCurrent.ForeColor = System.Drawing.Color.White;
            this.lblMainsCurrent.Location = new System.Drawing.Point(15, 176);
            this.lblMainsCurrent.Name = "lblMainsCurrent";
            this.lblMainsCurrent.Size = new System.Drawing.Size(251, 31);
            this.lblMainsCurrent.TabIndex = 9;
            this.lblMainsCurrent.Text = "Mains current  = X";
            // 
            // lbl12V
            // 
            this.lbl12V.AutoSize = true;
            this.lbl12V.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl12V.ForeColor = System.Drawing.Color.White;
            this.lbl12V.Location = new System.Drawing.Point(12, 237);
            this.lbl12V.Name = "lbl12V";
            this.lbl12V.Size = new System.Drawing.Size(160, 16);
            this.lbl12V.TabIndex = 10;
            this.lbl12V.Text = "+12V Input voltage = X";
            // 
            // lbl5V2
            // 
            this.lbl5V2.AutoSize = true;
            this.lbl5V2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl5V2.ForeColor = System.Drawing.Color.White;
            this.lbl5V2.Location = new System.Drawing.Point(12, 221);
            this.lbl5V2.Name = "lbl5V2";
            this.lbl5V2.Size = new System.Drawing.Size(160, 16);
            this.lbl5V2.TabIndex = 11;
            this.lbl5V2.Text = "+5V2 Input voltage = X";
            // 
            // lblMainsVoltage
            // 
            this.lblMainsVoltage.AutoSize = true;
            this.lblMainsVoltage.Font = new System.Drawing.Font("Microsoft Sans Serif", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblMainsVoltage.ForeColor = System.Drawing.Color.White;
            this.lblMainsVoltage.Location = new System.Drawing.Point(12, 145);
            this.lblMainsVoltage.Name = "lblMainsVoltage";
            this.lblMainsVoltage.Size = new System.Drawing.Size(254, 31);
            this.lblMainsVoltage.TabIndex = 12;
            this.lblMainsVoltage.Text = "Mains voltage  = X";
            // 
            // lblHV
            // 
            this.lblHV.AutoSize = true;
            this.lblHV.Font = new System.Drawing.Font("Microsoft Sans Serif", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHV.ForeColor = System.Drawing.Color.White;
            this.lblHV.Location = new System.Drawing.Point(12, 103);
            this.lblHV.Name = "lblHV";
            this.lblHV.Size = new System.Drawing.Size(321, 25);
            this.lblHV.TabIndex = 13;
            this.lblHV.Text = "HIGH VOLTAGE OUTPUT = X";
            // 
            // btnHVoff
            // 
            this.btnHVoff.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(192)))), ((int)(((byte)(0)))));
            this.btnHVoff.FlatStyle = System.Windows.Forms.FlatStyle.Popup;
            this.btnHVoff.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnHVoff.Location = new System.Drawing.Point(472, 118);
            this.btnHVoff.Name = "btnHVoff";
            this.btnHVoff.Size = new System.Drawing.Size(200, 100);
            this.btnHVoff.TabIndex = 14;
            this.btnHVoff.Text = "HIGH VOLTAGE OFF";
            this.btnHVoff.UseVisualStyleBackColor = false;
            this.btnHVoff.Click += new System.EventHandler(this.btnHVoff_Click);
            // 
            // lblHVstatus
            // 
            this.lblHVstatus.AutoSize = true;
            this.lblHVstatus.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblHVstatus.ForeColor = System.Drawing.Color.Lime;
            this.lblHVstatus.Location = new System.Drawing.Point(477, 221);
            this.lblHVstatus.Name = "lblHVstatus";
            this.lblHVstatus.Size = new System.Drawing.Size(195, 24);
            this.lblHVstatus.TabIndex = 17;
            this.lblHVstatus.Text = "HIGH VOLTAGE:OFF";
            // 
            // listBox1
            // 
            this.listBox1.Font = new System.Drawing.Font("Arial Narrow", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listBox1.ForeColor = System.Drawing.Color.Maroon;
            this.listBox1.FormattingEnabled = true;
            this.listBox1.ItemHeight = 20;
            this.listBox1.Location = new System.Drawing.Point(15, 343);
            this.listBox1.Name = "listBox1";
            this.listBox1.SelectionMode = System.Windows.Forms.SelectionMode.None;
            this.listBox1.Size = new System.Drawing.Size(200, 104);
            this.listBox1.TabIndex = 18;
            // 
            // lblSafe
            // 
            this.lblSafe.AutoSize = true;
            this.lblSafe.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSafe.ForeColor = System.Drawing.Color.Orange;
            this.lblSafe.Location = new System.Drawing.Point(478, 245);
            this.lblSafe.Name = "lblSafe";
            this.lblSafe.Size = new System.Drawing.Size(78, 16);
            this.lblSafe.TabIndex = 23;
            this.lblSafe.Text = "#ERROR#";
            // 
            // button1
            // 
            this.button1.BackColor = System.Drawing.SystemColors.HighlightText;
            this.button1.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button1.Location = new System.Drawing.Point(522, 343);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(150, 104);
            this.button1.TabIndex = 24;
            this.button1.Text = "Save log";
            this.button1.UseVisualStyleBackColor = false;
            this.button1.Click += new System.EventHandler(this.button1_Click_1);
            // 
            // lblWAPIN
            // 
            this.lblWAPIN.AutoSize = true;
            this.lblWAPIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblWAPIN.ForeColor = System.Drawing.Color.White;
            this.lblWAPIN.Location = new System.Drawing.Point(9, 297);
            this.lblWAPIN.Name = "lblWAPIN";
            this.lblWAPIN.Size = new System.Drawing.Size(83, 16);
            this.lblWAPIN.TabIndex = 25;
            this.lblWAPIN.Text = "WA Pin = X";
            // 
            // lblSAPIN
            // 
            this.lblSAPIN.AutoSize = true;
            this.lblSAPIN.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSAPIN.ForeColor = System.Drawing.Color.White;
            this.lblSAPIN.Location = new System.Drawing.Point(12, 313);
            this.lblSAPIN.Name = "lblSAPIN";
            this.lblSAPIN.Size = new System.Drawing.Size(79, 16);
            this.lblSAPIN.TabIndex = 26;
            this.lblSAPIN.Text = "SA Pin = X";
            // 
            // lblCheckSpeed
            // 
            this.lblCheckSpeed.AutoSize = true;
            this.lblCheckSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCheckSpeed.ForeColor = System.Drawing.Color.White;
            this.lblCheckSpeed.Location = new System.Drawing.Point(34, 450);
            this.lblCheckSpeed.Name = "lblCheckSpeed";
            this.lblCheckSpeed.Size = new System.Drawing.Size(152, 16);
            this.lblCheckSpeed.TabIndex = 27;
            this.lblCheckSpeed.Text = "Checksum speed = X";
            // 
            // button2
            // 
            this.button2.BackColor = System.Drawing.SystemColors.HighlightText;
            this.button2.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.button2.Location = new System.Drawing.Point(324, 450);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(76, 66);
            this.button2.TabIndex = 29;
            this.button2.Text = "serial test";
            this.button2.UseVisualStyleBackColor = false;
            this.button2.Visible = false;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lblComSpeed
            // 
            this.lblComSpeed.AutoSize = true;
            this.lblComSpeed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblComSpeed.ForeColor = System.Drawing.Color.White;
            this.lblComSpeed.Location = new System.Drawing.Point(12, 466);
            this.lblComSpeed.Name = "lblComSpeed";
            this.lblComSpeed.Size = new System.Drawing.Size(174, 16);
            this.lblComSpeed.TabIndex = 30;
            this.lblComSpeed.Text = "Comunication speed = X";
            // 
            // button4
            // 
            this.button4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(224)))), ((int)(((byte)(224)))));
            this.button4.Enabled = false;
            this.button4.Location = new System.Drawing.Point(123, 12);
            this.button4.Name = "button4";
            this.button4.Size = new System.Drawing.Size(100, 75);
            this.button4.TabIndex = 31;
            this.button4.Text = "REBOOT";
            this.button4.UseVisualStyleBackColor = false;
            this.button4.Click += new System.EventHandler(this.button4_Click);
            // 
            // lblTime
            // 
            this.lblTime.AutoSize = true;
            this.lblTime.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTime.ForeColor = System.Drawing.Color.White;
            this.lblTime.Location = new System.Drawing.Point(488, 466);
            this.lblTime.Name = "lblTime";
            this.lblTime.Size = new System.Drawing.Size(68, 16);
            this.lblTime.TabIndex = 32;
            this.lblTime.Text = "Time = X";
            // 
            // lblCPM5
            // 
            this.lblCPM5.AutoSize = true;
            this.lblCPM5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCPM5.ForeColor = System.Drawing.Color.White;
            this.lblCPM5.Location = new System.Drawing.Point(145, 313);
            this.lblCPM5.Name = "lblCPM5";
            this.lblCPM5.Size = new System.Drawing.Size(98, 16);
            this.lblCPM5.TabIndex = 33;
            this.lblCPM5.Text = "CPM(5s)   = 0";
            // 
            // lblCPM15
            // 
            this.lblCPM15.AutoSize = true;
            this.lblCPM15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCPM15.ForeColor = System.Drawing.Color.White;
            this.lblCPM15.Location = new System.Drawing.Point(452, 292);
            this.lblCPM15.Name = "lblCPM15";
            this.lblCPM15.Size = new System.Drawing.Size(98, 16);
            this.lblCPM15.TabIndex = 34;
            this.lblCPM15.Text = "CPM(15s) = 0";
            this.lblCPM15.Visible = false;
            // 
            // lblCPM30
            // 
            this.lblCPM30.AutoSize = true;
            this.lblCPM30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCPM30.ForeColor = System.Drawing.Color.White;
            this.lblCPM30.Location = new System.Drawing.Point(452, 308);
            this.lblCPM30.Name = "lblCPM30";
            this.lblCPM30.Size = new System.Drawing.Size(98, 16);
            this.lblCPM30.TabIndex = 35;
            this.lblCPM30.Text = "CPM(30s) = 0";
            this.lblCPM30.Visible = false;
            // 
            // lblCPM60
            // 
            this.lblCPM60.AutoSize = true;
            this.lblCPM60.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblCPM60.ForeColor = System.Drawing.Color.White;
            this.lblCPM60.Location = new System.Drawing.Point(452, 324);
            this.lblCPM60.Name = "lblCPM60";
            this.lblCPM60.Size = new System.Drawing.Size(98, 16);
            this.lblCPM60.TabIndex = 36;
            this.lblCPM60.Text = "CPM(60s) = 0";
            this.lblCPM60.Visible = false;
            // 
            // lblSV5
            // 
            this.lblSV5.AutoSize = true;
            this.lblSV5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSV5.ForeColor = System.Drawing.Color.White;
            this.lblSV5.Location = new System.Drawing.Point(262, 313);
            this.lblSV5.Name = "lblSV5";
            this.lblSV5.Size = new System.Drawing.Size(157, 16);
            this.lblSV5.TabIndex = 37;
            this.lblSV5.Text = "Rad last 5s   = 0µSv/h";
            // 
            // lblSV15
            // 
            this.lblSV15.AutoSize = true;
            this.lblSV15.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSV15.ForeColor = System.Drawing.Color.White;
            this.lblSV15.Location = new System.Drawing.Point(581, 292);
            this.lblSV15.Name = "lblSV15";
            this.lblSV15.Size = new System.Drawing.Size(157, 16);
            this.lblSV15.TabIndex = 38;
            this.lblSV15.Text = "Rad last 15s = 0µSv/h";
            this.lblSV15.Visible = false;
            // 
            // lblSV30
            // 
            this.lblSV30.AutoSize = true;
            this.lblSV30.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSV30.ForeColor = System.Drawing.Color.White;
            this.lblSV30.Location = new System.Drawing.Point(581, 308);
            this.lblSV30.Name = "lblSV30";
            this.lblSV30.Size = new System.Drawing.Size(157, 16);
            this.lblSV30.TabIndex = 39;
            this.lblSV30.Text = "Rad last 30s = 0µSv/h";
            this.lblSV30.Visible = false;
            // 
            // lblSV60
            // 
            this.lblSV60.AutoSize = true;
            this.lblSV60.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblSV60.ForeColor = System.Drawing.Color.White;
            this.lblSV60.Location = new System.Drawing.Point(581, 324);
            this.lblSV60.Name = "lblSV60";
            this.lblSV60.Size = new System.Drawing.Size(157, 16);
            this.lblSV60.TabIndex = 40;
            this.lblSV60.Text = "Rad last 60s = 0µSv/h";
            this.lblSV60.Visible = false;
            // 
            // lblTemp
            // 
            this.lblTemp.AutoSize = true;
            this.lblTemp.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblTemp.ForeColor = System.Drawing.Color.White;
            this.lblTemp.Location = new System.Drawing.Point(18, 267);
            this.lblTemp.Name = "lblTemp";
            this.lblTemp.Size = new System.Drawing.Size(129, 16);
            this.lblTemp.TabIndex = 42;
            this.lblTemp.Text = "Temperature = 27";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Black;
            this.ClientSize = new System.Drawing.Size(684, 491);
            this.Controls.Add(this.lblTemp);
            this.Controls.Add(this.lblSV60);
            this.Controls.Add(this.lblSV30);
            this.Controls.Add(this.lblSV15);
            this.Controls.Add(this.lblSV5);
            this.Controls.Add(this.lblCPM60);
            this.Controls.Add(this.lblCPM30);
            this.Controls.Add(this.lblCPM15);
            this.Controls.Add(this.lblCPM5);
            this.Controls.Add(this.lblTime);
            this.Controls.Add(this.button4);
            this.Controls.Add(this.lblComSpeed);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.lblCheckSpeed);
            this.Controls.Add(this.lblSAPIN);
            this.Controls.Add(this.lblWAPIN);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.lblSafe);
            this.Controls.Add(this.listBox1);
            this.Controls.Add(this.lblHVstatus);
            this.Controls.Add(this.btnHVoff);
            this.Controls.Add(this.lblHV);
            this.Controls.Add(this.lblMainsVoltage);
            this.Controls.Add(this.lbl5V2);
            this.Controls.Add(this.lbl12V);
            this.Controls.Add(this.lblMainsCurrent);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.btnForm2);
            this.Controls.Add(this.btnHVon);
            this.Controls.Add(this.btnEXIT);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Name = "Form1";
            this.Text = "Fusor controler";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnEXIT;
        private System.Windows.Forms.Button btnHVon;
        private System.Windows.Forms.Button btnForm2;
        private System.Windows.Forms.Button button3;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lblMainsCurrent;
        private System.Windows.Forms.Label lbl12V;
        private System.Windows.Forms.Label lbl5V2;
        private System.Windows.Forms.Label lblMainsVoltage;
        private System.Windows.Forms.Label lblHV;
        private System.Windows.Forms.Button btnHVoff;
        private System.Windows.Forms.Label lblHVstatus;
        private System.Windows.Forms.ListBox listBox1;
        private System.Windows.Forms.Label lblSafe;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Label lblWAPIN;
        private System.Windows.Forms.Label lblSAPIN;
        private System.Windows.Forms.Label lblCheckSpeed;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lblComSpeed;
        private System.Windows.Forms.Button button4;
        private System.Windows.Forms.Label lblTime;
        private System.Windows.Forms.Label lblCPM5;
        private System.Windows.Forms.Label lblCPM15;
        private System.Windows.Forms.Label lblCPM30;
        private System.Windows.Forms.Label lblCPM60;
        private System.Windows.Forms.Label lblSV5;
        private System.Windows.Forms.Label lblSV15;
        private System.Windows.Forms.Label lblSV30;
        private System.Windows.Forms.Label lblSV60;
        private System.Windows.Forms.Label lblTemp;
    }
}

