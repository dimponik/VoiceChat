namespace VoiceChat
{
    partial class VoiceChat
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
            this.btnCall = new System.Windows.Forms.Button();
            this.btnEndCall = new System.Windows.Forms.Button();
            this.txtCallToIP = new System.Windows.Forms.TextBox();
            this.txtName = new System.Windows.Forms.TextBox();
            this.lblCallTo = new System.Windows.Forms.Label();
            this.lblName = new System.Windows.Forms.Label();
            this.cmbCodecs = new System.Windows.Forms.ComboBox();
            this.lblCodec = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // btnCall
            // 
            this.btnCall.Location = new System.Drawing.Point(30, 119);
            this.btnCall.Name = "btnCall";
            this.btnCall.Size = new System.Drawing.Size(75, 23);
            this.btnCall.TabIndex = 0;
            this.btnCall.Text = "&Call";
            this.btnCall.UseVisualStyleBackColor = true;
            this.btnCall.Click += new System.EventHandler(this.btnCall_Click);
            // 
            // btnEndCall
            // 
            this.btnEndCall.Enabled = false;
            this.btnEndCall.Location = new System.Drawing.Point(178, 119);
            this.btnEndCall.Name = "btnEndCall";
            this.btnEndCall.Size = new System.Drawing.Size(75, 23);
            this.btnEndCall.TabIndex = 1;
            this.btnEndCall.Text = "&End Call";
            this.btnEndCall.UseVisualStyleBackColor = true;
            this.btnEndCall.Click += new System.EventHandler(this.btnEndCall_Click);
            // 
            // txtCallToIP
            // 
            this.txtCallToIP.Location = new System.Drawing.Point(63, 15);
            this.txtCallToIP.Name = "txtCallToIP";
            this.txtCallToIP.Size = new System.Drawing.Size(205, 20);
            this.txtCallToIP.TabIndex = 3;
            // 
            // txtName
            // 
            this.txtName.Location = new System.Drawing.Point(63, 41);
            this.txtName.Name = "txtName";
            this.txtName.Size = new System.Drawing.Size(205, 20);
            this.txtName.TabIndex = 4;
            // 
            // lblCallTo
            // 
            this.lblCallTo.AutoSize = true;
            this.lblCallTo.Location = new System.Drawing.Point(14, 18);
            this.lblCallTo.Name = "lblCallTo";
            this.lblCallTo.Size = new System.Drawing.Size(43, 13);
            this.lblCallTo.TabIndex = 5;
            this.lblCallTo.Text = "Call &To:";
            // 
            // lblName
            // 
            this.lblName.AutoSize = true;
            this.lblName.Location = new System.Drawing.Point(14, 44);
            this.lblName.Name = "lblName";
            this.lblName.Size = new System.Drawing.Size(35, 13);
            this.lblName.TabIndex = 6;
            this.lblName.Text = "&Name";
            // 
            // cmbCodecs
            // 
            this.cmbCodecs.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCodecs.FormattingEnabled = true;
            this.cmbCodecs.Items.AddRange(new object[] {
            "None",
            "A-Law",
            "u-Law"});
            this.cmbCodecs.Location = new System.Drawing.Point(63, 67);
            this.cmbCodecs.Name = "cmbCodecs";
            this.cmbCodecs.Size = new System.Drawing.Size(205, 21);
            this.cmbCodecs.TabIndex = 7;
            // 
            // lblCodec
            // 
            this.lblCodec.AutoSize = true;
            this.lblCodec.Location = new System.Drawing.Point(14, 70);
            this.lblCodec.Name = "lblCodec";
            this.lblCodec.Size = new System.Drawing.Size(41, 13);
            this.lblCodec.TabIndex = 8;
            this.lblCodec.Text = "C&odec:";
            // 
            // VoiceChat
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(283, 158);
            this.Controls.Add(this.lblCodec);
            this.Controls.Add(this.cmbCodecs);
            this.Controls.Add(this.lblName);
            this.Controls.Add(this.lblCallTo);
            this.Controls.Add(this.txtName);
            this.Controls.Add(this.txtCallToIP);
            this.Controls.Add(this.btnEndCall);
            this.Controls.Add(this.btnCall);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.Name = "VoiceChat";
            this.Text = "VoiceChat";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.VoiceChat_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button btnCall;
        private System.Windows.Forms.Button btnEndCall;
        private System.Windows.Forms.TextBox txtCallToIP;
        private System.Windows.Forms.TextBox txtName;
        private System.Windows.Forms.Label lblCallTo;
        private System.Windows.Forms.Label lblName;
        private System.Windows.Forms.ComboBox cmbCodecs;
        private System.Windows.Forms.Label lblCodec;
    }
}

