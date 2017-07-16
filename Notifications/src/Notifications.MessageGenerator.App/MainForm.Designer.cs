namespace Notifications.MessageGenerator.App
{
    partial class MainForm
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
			this.customerComboBox = new System.Windows.Forms.ComboBox();
			this.messageTypeComboBox = new System.Windows.Forms.ComboBox();
			this.submitButton = new System.Windows.Forms.Button();
			this.messageTypeLabel = new System.Windows.Forms.Label();
			this.customerLabel = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// customerComboBox
			// 
			this.customerComboBox.FormattingEnabled = true;
			this.customerComboBox.Location = new System.Drawing.Point(111, 72);
			this.customerComboBox.Name = "customerComboBox";
			this.customerComboBox.Size = new System.Drawing.Size(209, 21);
			this.customerComboBox.TabIndex = 0;
			// 
			// messageTypeComboBox
			// 
			this.messageTypeComboBox.FormattingEnabled = true;
			this.messageTypeComboBox.Location = new System.Drawing.Point(111, 119);
			this.messageTypeComboBox.Name = "messageTypeComboBox";
			this.messageTypeComboBox.Size = new System.Drawing.Size(209, 21);
			this.messageTypeComboBox.TabIndex = 1;
			// 
			// submitButton
			// 
			this.submitButton.Location = new System.Drawing.Point(111, 168);
			this.submitButton.Name = "submitButton";
			this.submitButton.Size = new System.Drawing.Size(209, 23);
			this.submitButton.TabIndex = 2;
			this.submitButton.Text = "Submit";
			this.submitButton.UseVisualStyleBackColor = true;
			this.submitButton.Click += new System.EventHandler(this.SubmitButton_Click);
			// 
			// messageTypeLabel
			// 
			this.messageTypeLabel.AutoSize = true;
			this.messageTypeLabel.Location = new System.Drawing.Point(111, 103);
			this.messageTypeLabel.Name = "messageTypeLabel";
			this.messageTypeLabel.Size = new System.Drawing.Size(77, 13);
			this.messageTypeLabel.TabIndex = 3;
			this.messageTypeLabel.Text = "Message Type";
			this.messageTypeLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// customerLabel
			// 
			this.customerLabel.AutoSize = true;
			this.customerLabel.Location = new System.Drawing.Point(111, 56);
			this.customerLabel.Name = "customerLabel";
			this.customerLabel.Size = new System.Drawing.Size(51, 13);
			this.customerLabel.TabIndex = 4;
			this.customerLabel.Text = "Customer";
			this.customerLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
			// 
			// MainForm
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(431, 261);
			this.Controls.Add(this.customerLabel);
			this.Controls.Add(this.messageTypeLabel);
			this.Controls.Add(this.submitButton);
			this.Controls.Add(this.messageTypeComboBox);
			this.Controls.Add(this.customerComboBox);
			this.Name = "MainForm";
			this.Text = "Message Generator";
			this.Load += new System.EventHandler(this.MainForm_Load);
			this.ResumeLayout(false);
			this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ComboBox customerComboBox;
        private System.Windows.Forms.ComboBox messageTypeComboBox;
        private System.Windows.Forms.Button submitButton;
		private System.Windows.Forms.Label messageTypeLabel;
		private System.Windows.Forms.Label customerLabel;
	}
}

