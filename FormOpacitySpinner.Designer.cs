namespace StickyNotes
{
    partial class FormOpacitySpinner
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
            this.opacitySpinner = new System.Windows.Forms.NumericUpDown();
            this.okBtn = new System.Windows.Forms.Button();
            this.cancelBtn = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.opacitySpinner)).BeginInit();
            this.SuspendLayout();
            // 
            // opacitySpinner
            // 
            this.opacitySpinner.Font = new System.Drawing.Font("MS UI Gothic", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(128)));
            this.opacitySpinner.Location = new System.Drawing.Point(0, 1);
            this.opacitySpinner.Minimum = new decimal(new int[] {
            20,
            0,
            0,
            0});
            this.opacitySpinner.Name = "opacitySpinner";
            this.opacitySpinner.Size = new System.Drawing.Size(90, 26);
            this.opacitySpinner.TabIndex = 0;
            this.opacitySpinner.TabStop = false;
            this.opacitySpinner.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.opacitySpinner.Value = new decimal(new int[] {
            20,
            0,
            0,
            0});
            // 
            // okBtn
            // 
            this.okBtn.Location = new System.Drawing.Point(0, 29);
            this.okBtn.Name = "okBtn";
            this.okBtn.Size = new System.Drawing.Size(45, 20);
            this.okBtn.TabIndex = 1;
            this.okBtn.Text = "設定";
            this.okBtn.UseVisualStyleBackColor = true;
            // 
            // cancelBtn
            // 
            this.cancelBtn.Location = new System.Drawing.Point(45, 29);
            this.cancelBtn.Name = "cancelBtn";
            this.cancelBtn.Size = new System.Drawing.Size(45, 20);
            this.cancelBtn.TabIndex = 2;
            this.cancelBtn.Text = "取消";
            this.cancelBtn.UseVisualStyleBackColor = true;
            // 
            // FormOpacitySpinner
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(90, 50);
            this.ControlBox = false;
            this.Controls.Add(this.cancelBtn);
            this.Controls.Add(this.okBtn);
            this.Controls.Add(this.opacitySpinner);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.None;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "FormOpacitySpinner";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "FormOpacitySpinner";
            ((System.ComponentModel.ISupportInitialize)(this.opacitySpinner)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.NumericUpDown opacitySpinner;
        private System.Windows.Forms.Button okBtn;
        private System.Windows.Forms.Button cancelBtn;
    }
}