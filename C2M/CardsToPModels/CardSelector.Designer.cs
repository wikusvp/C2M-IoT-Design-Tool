
using System;
using System.Windows.Forms;

namespace CardsToPModels
{
    partial class CardSelector
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
            this.TypeOfCard = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.panel1 = new System.Windows.Forms.Panel();
            this.pictureBox = new System.Windows.Forms.PictureBox();
            this.selectedCard = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.changeCategoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).BeginInit();
            this.menuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // TypeOfCard
            // 
            this.TypeOfCard.Font = new System.Drawing.Font("Calibri", 15.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.TypeOfCard.Location = new System.Drawing.Point(-1, 23);
            this.TypeOfCard.Name = "TypeOfCard";
            this.TypeOfCard.Size = new System.Drawing.Size(886, 23);
            this.TypeOfCard.TabIndex = 0;
            this.TypeOfCard.Text = "label1";
            this.TypeOfCard.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Calibri", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(308, 59);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(278, 15);
            this.label1.TabIndex = 2;
            this.label1.Text = "Click on a card to select and add it to your design";
            // 
            // panel1
            // 
            this.panel1.AutoScroll = true;
            this.panel1.Controls.Add(this.pictureBox);
            this.panel1.Location = new System.Drawing.Point(12, 87);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(860, 396);
            this.panel1.TabIndex = 3;
            this.panel1.Scroll += new System.Windows.Forms.ScrollEventHandler(this.panel1_Scroll);
            this.panel1.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.panel1_MouseWheel);
            // 
            // pictureBox
            // 
            this.pictureBox.Location = new System.Drawing.Point(32, 3);
            this.pictureBox.Name = "pictureBox";
            this.pictureBox.Size = new System.Drawing.Size(803, 310);
            this.pictureBox.TabIndex = 0;
            this.pictureBox.TabStop = false;
            this.pictureBox.Paint += new System.Windows.Forms.PaintEventHandler(this.pictureBox_Paint);
            this.pictureBox.MouseClick += new System.Windows.Forms.MouseEventHandler(this.pictureBox_MouseClick);
            // 
            // selectedCard
            // 
            this.selectedCard.AutoSize = true;
            this.selectedCard.Font = new System.Drawing.Font("Calibri", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.selectedCard.ForeColor = System.Drawing.SystemColors.ControlDarkDark;
            this.selectedCard.Location = new System.Drawing.Point(12, 503);
            this.selectedCard.Name = "selectedCard";
            this.selectedCard.Size = new System.Drawing.Size(98, 14);
            this.selectedCard.TabIndex = 5;
            this.selectedCard.Text = "No card selected";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.changeCategoryToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(884, 24);
            this.menuStrip1.TabIndex = 6;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // changeCategoryToolStripMenuItem
            // 
            this.changeCategoryToolStripMenuItem.Name = "changeCategoryToolStripMenuItem";
            this.changeCategoryToolStripMenuItem.Size = new System.Drawing.Size(111, 20);
            this.changeCategoryToolStripMenuItem.Text = "Change Category";
            // 
            // CardSelector
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(884, 524);
            this.Controls.Add(this.selectedCard);
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.TypeOfCard);
            this.Controls.Add(this.menuStrip1);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MainMenuStrip = this.menuStrip1;
            this.MaximizeBox = false;
            this.Name = "CardSelector";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "CardSelector";
            this.Load += new System.EventHandler(this.CardSelector_Load);
            this.panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }


        #endregion

        private System.Windows.Forms.Label TypeOfCard;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Panel panel1;
        private System.Windows.Forms.PictureBox pictureBox;
        private System.Windows.Forms.Label selectedCard;
        private MenuStrip menuStrip1;
        private ToolStripMenuItem changeCategoryToolStripMenuItem;
    }
}