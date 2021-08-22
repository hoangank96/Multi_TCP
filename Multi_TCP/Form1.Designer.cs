namespace Multi_TCP
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
            this.button1 = new System.Windows.Forms.Button();
            this.button2 = new System.Windows.Forms.Button();
            this.lb_proxy = new System.Windows.Forms.Label();
            this.Check_GetSock = new System.Windows.Forms.CheckBox();
            this.Check_live_proxy = new System.Windows.Forms.CheckBox();
            this.thread_run = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.button3 = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Location = new System.Drawing.Point(24, 21);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(70, 32);
            this.button1.TabIndex = 0;
            this.button1.Text = "Start";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // button2
            // 
            this.button2.Location = new System.Drawing.Point(24, 75);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(70, 30);
            this.button2.TabIndex = 1;
            this.button2.Text = "Sock_Proxy";
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // lb_proxy
            // 
            this.lb_proxy.AutoSize = true;
            this.lb_proxy.Location = new System.Drawing.Point(169, 75);
            this.lb_proxy.Name = "lb_proxy";
            this.lb_proxy.Size = new System.Drawing.Size(0, 13);
            this.lb_proxy.TabIndex = 2;
            // 
            // Check_GetSock
            // 
            this.Check_GetSock.AutoSize = true;
            this.Check_GetSock.Location = new System.Drawing.Point(172, 21);
            this.Check_GetSock.Name = "Check_GetSock";
            this.Check_GetSock.Size = new System.Drawing.Size(109, 17);
            this.Check_GetSock.TabIndex = 3;
            this.Check_GetSock.Text = "Get Socks Online";
            this.Check_GetSock.UseVisualStyleBackColor = true;
            // 
            // Check_live_proxy
            // 
            this.Check_live_proxy.AutoSize = true;
            this.Check_live_proxy.Checked = true;
            this.Check_live_proxy.CheckState = System.Windows.Forms.CheckState.Checked;
            this.Check_live_proxy.Location = new System.Drawing.Point(172, 59);
            this.Check_live_proxy.Name = "Check_live_proxy";
            this.Check_live_proxy.Size = new System.Drawing.Size(109, 17);
            this.Check_live_proxy.TabIndex = 4;
            this.Check_live_proxy.Text = "Check Live Proxy";
            this.Check_live_proxy.UseVisualStyleBackColor = true;
            // 
            // thread_run
            // 
            this.thread_run.AutoSize = true;
            this.thread_run.Location = new System.Drawing.Point(119, 168);
            this.thread_run.Name = "thread_run";
            this.thread_run.Size = new System.Drawing.Size(0, 13);
            this.thread_run.TabIndex = 5;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(40, 164);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 13);
            this.label1.TabIndex = 6;
            this.label1.Text = "Complate:";
            // 
            // button3
            // 
            this.button3.Location = new System.Drawing.Point(36, 213);
            this.button3.Name = "button3";
            this.button3.Size = new System.Drawing.Size(113, 34);
            this.button3.TabIndex = 7;
            this.button3.Text = "Testbtn";
            this.button3.UseVisualStyleBackColor = true;
            this.button3.Click += new System.EventHandler(this.button3_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(284, 261);
            this.Controls.Add(this.button3);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.thread_run);
            this.Controls.Add(this.Check_live_proxy);
            this.Controls.Add(this.Check_GetSock);
            this.Controls.Add(this.lb_proxy);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.button1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.Label lb_proxy;
        private System.Windows.Forms.CheckBox Check_GetSock;
        private System.Windows.Forms.CheckBox Check_live_proxy;
        private System.Windows.Forms.Label thread_run;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button button3;
    }
}

