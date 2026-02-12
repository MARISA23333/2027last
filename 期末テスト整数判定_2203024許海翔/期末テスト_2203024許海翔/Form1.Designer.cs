namespace 期末テスト_2203024許海翔
{
    partial class Form1
    {
        
        private System.ComponentModel.IContainer components = null;

        private System.Windows.Forms.TextBox textBoxInput;
        private System.Windows.Forms.Button buttonConfirm;
        private System.Windows.Forms.Label labelResult;

        
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows フォーム デザイナーで生成されたコード

        
        private void InitializeComponent()
        {
            this.textBoxInput = new System.Windows.Forms.TextBox();
            this.buttonConfirm = new System.Windows.Forms.Button();
            this.labelResult = new System.Windows.Forms.Label();
            this.SuspendLayout();
            
            this.textBoxInput.Location = new System.Drawing.Point(24, 24);
            this.textBoxInput.Name = "textBoxInput";
            this.textBoxInput.Size = new System.Drawing.Size(240, 19);
            this.textBoxInput.TabIndex = 0;
            
            this.buttonConfirm.Location = new System.Drawing.Point(280, 22);
            this.buttonConfirm.Name = "buttonConfirm";
            this.buttonConfirm.Size = new System.Drawing.Size(96, 23);
            this.buttonConfirm.TabIndex = 1;
            this.buttonConfirm.Text = "確認";
            this.buttonConfirm.UseVisualStyleBackColor = true;
            this.buttonConfirm.Click += new System.EventHandler(this.buttonConfirm_Click);
            
            this.labelResult.AutoSize = true;
            this.labelResult.Location = new System.Drawing.Point(24, 64);
            this.labelResult.Name = "labelResult";
            this.labelResult.Size = new System.Drawing.Size(0, 12);
            this.labelResult.TabIndex = 2;
            
            // Form1
            
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(420, 120);
            this.Controls.Add(this.labelResult);
            this.Controls.Add(this.buttonConfirm);
            this.Controls.Add(this.textBoxInput);
            this.Name = "Form1";
            this.Text = "整数判定";
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion
    }
}

