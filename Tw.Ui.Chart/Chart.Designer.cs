namespace Tw.Ui.Chart
{
    partial class Chart
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
            this.financialChartType1 = new WinFormsChartSamples.FinancialChartType();
            this.SuspendLayout();
            // 
            // financialChartType1
            // 
            this.financialChartType1.BackColor = System.Drawing.Color.White;
            this.financialChartType1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.financialChartType1.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.financialChartType1.Location = new System.Drawing.Point(0, 0);
            this.financialChartType1.Name = "financialChartType1";
            this.financialChartType1.Size = new System.Drawing.Size(800, 513);
            this.financialChartType1.TabIndex = 0;
            // 
            // Chart
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 513);
            this.Controls.Add(this.financialChartType1);
            this.Name = "Chart";
            this.Text = "Form1";
            this.ResumeLayout(false);

        }

        #endregion

        private WinFormsChartSamples.FinancialChartType financialChartType1;

    }
}

