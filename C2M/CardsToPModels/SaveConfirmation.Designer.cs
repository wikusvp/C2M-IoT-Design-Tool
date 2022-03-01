
namespace CardsToPModels
{
    partial class SaveConfirmation
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
            this.save = new System.Windows.Forms.Button();
            this.dontSave = new System.Windows.Forms.Button();
            this.cancel = new System.Windows.Forms.Button();
            this.lblConfirmation = new System.Windows.Forms.Label();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // save
            // 
            this.save.Location = new System.Drawing.Point(242, 93);
            this.save.Name = "save";
            this.save.Size = new System.Drawing.Size(75, 23);
            this.save.TabIndex = 0;
            this.save.Text = "Save";
            this.save.UseVisualStyleBackColor = true;
            this.save.Click += new System.EventHandler(this.save_Click);
            // 
            // dontSave
            // 
            this.dontSave.Location = new System.Drawing.Point(323, 93);
            this.dontSave.Name = "dontSave";
            this.dontSave.Size = new System.Drawing.Size(75, 23);
            this.dontSave.TabIndex = 1;
            this.dontSave.Text = "Don\'t save";
            this.dontSave.UseVisualStyleBackColor = true;
            this.dontSave.Click += new System.EventHandler(this.dontSave_Click);
            // 
            // cancel
            // 
            this.cancel.Location = new System.Drawing.Point(404, 93);
            this.cancel.Name = "cancel";
            this.cancel.Size = new System.Drawing.Size(75, 23);
            this.cancel.TabIndex = 2;
            this.cancel.Text = "Cancel";
            this.cancel.UseVisualStyleBackColor = true;
            this.cancel.Click += new System.EventHandler(this.cancel_Click);
            // 
            // lblConfirmation
            // 
            this.lblConfirmation.AutoSize = true;
            this.lblConfirmation.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lblConfirmation.ForeColor = System.Drawing.Color.DarkSlateBlue;
            this.lblConfirmation.Location = new System.Drawing.Point(12, 23);
            this.lblConfirmation.Name = "lblConfirmation";
            this.lblConfirmation.Size = new System.Drawing.Size(281, 24);
            this.lblConfirmation.TabIndex = 3;
            this.lblConfirmation.Text = "Do you want to save changes to ";
            // 
            // pictureBox1
            // 
            this.pictureBox1.BackColor = System.Drawing.SystemColors.ControlLight;
            this.pictureBox1.Location = new System.Drawing.Point(-3, 84);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(495, 46);
            this.pictureBox1.TabIndex = 4;
            this.pictureBox1.TabStop = false;
            // 
            // SaveConfirmation
            // 
            this.AcceptButton = this.save;
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(490, 124);
            this.Controls.Add(this.lblConfirmation);
            this.Controls.Add(this.cancel);
            this.Controls.Add(this.dontSave);
            this.Controls.Add(this.save);
            this.Controls.Add(this.pictureBox1);
            this.Name = "SaveConfirmation";
            this.Text = "C2M";
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button save;
        private System.Windows.Forms.Button dontSave;
        private System.Windows.Forms.Button cancel;
        private System.Windows.Forms.Label lblConfirmation;
        private System.Windows.Forms.PictureBox pictureBox1;
    }
}