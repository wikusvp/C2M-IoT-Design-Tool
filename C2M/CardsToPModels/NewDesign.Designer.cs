
namespace CardsToPModels
{
    partial class NewDesign
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
            this.iotCards = new System.Windows.Forms.Button();
            this.mobileCards = new System.Windows.Forms.Button();
            this.conceptualCards = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // iotCards
            // 
            this.iotCards.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.iotCards.Location = new System.Drawing.Point(23, 20);
            this.iotCards.Margin = new System.Windows.Forms.Padding(1);
            this.iotCards.Name = "iotCards";
            this.iotCards.Size = new System.Drawing.Size(329, 74);
            this.iotCards.TabIndex = 0;
            this.iotCards.Text = "IoT Cards";
            this.iotCards.UseVisualStyleBackColor = true;
            this.iotCards.Click += new System.EventHandler(this.iotCards_Click);
            // 
            // mobileCards
            // 
            this.mobileCards.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.mobileCards.Location = new System.Drawing.Point(23, 112);
            this.mobileCards.Margin = new System.Windows.Forms.Padding(1);
            this.mobileCards.Name = "mobileCards";
            this.mobileCards.Size = new System.Drawing.Size(329, 74);
            this.mobileCards.TabIndex = 1;
            this.mobileCards.Text = "Mobile Application Cards";
            this.mobileCards.UseVisualStyleBackColor = true;
            this.mobileCards.Click += new System.EventHandler(this.mobileCards_Click);
            // 
            // conceptualCards
            // 
            this.conceptualCards.Font = new System.Drawing.Font("Microsoft Sans Serif", 14F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.conceptualCards.Location = new System.Drawing.Point(23, 203);
            this.conceptualCards.Margin = new System.Windows.Forms.Padding(1);
            this.conceptualCards.Name = "conceptualCards";
            this.conceptualCards.Size = new System.Drawing.Size(329, 74);
            this.conceptualCards.TabIndex = 2;
            this.conceptualCards.Text = "Conceptual Cards";
            this.conceptualCards.UseVisualStyleBackColor = true;
            this.conceptualCards.Click += new System.EventHandler(this.conceptualCards_Click);
            // 
            // NewDesign
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(381, 300);
            this.Controls.Add(this.conceptualCards);
            this.Controls.Add(this.mobileCards);
            this.Controls.Add(this.iotCards);
            this.Margin = new System.Windows.Forms.Padding(1);
            this.Name = "NewDesign";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select the card deck to use";
            this.Load += new System.EventHandler(this.NewDesign_Load);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Button iotCards;
        private System.Windows.Forms.Button mobileCards;
        private System.Windows.Forms.Button conceptualCards;
    }
}