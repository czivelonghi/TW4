using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows.Forms;
using System.IO;
using System.Windows.Forms.DataVisualization.Charting;
using Tw.Model.Entity;
using Tw.Model.View;
using Tw.Service.Helper;
using System.Collections.Generic;

namespace WinFormsChartSamples
{
	/// <summary>
	/// Summary description for FinancialChartType.
	/// </summary>
	public class FinancialChartType : System.Windows.Forms.UserControl
	{
		private MemoryStream defaultViewStyleStream = new MemoryStream();

        private System.Windows.Forms.DataVisualization.Charting.Chart chart1;
		private System.Windows.Forms.Panel panel1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.Label label2;
		private System.Windows.Forms.ComboBox comboBoxMarks;
		private System.Windows.Forms.CheckBox checkBoxCloseOnly;
        private System.Windows.Forms.ComboBox comboBoxChartType;
        
		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public FinancialChartType()
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();
            // TODO: Add any initialization after the InitForm call
            //this.chart1.KeyDown += new System.Windows.Forms.KeyEventHandler(this.chart1_KeyUp);
            //this.chart1.Click += new System.EventHandler(this.chart1_Click);
		}

        MemoryMapView MMV;
        public void SetMemoryMap(MemoryMapView mmv)
        {
            MMV = mmv;
            this.ParentForm.Text = mmv.Company.Name + " (" + mmv.Company.Symbol + ")   " + mmv.StartDate.ToString() + "-" + mmv.EndDate.ToString();
        }

		/// <summary> 
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			defaultViewStyleStream.Close();

			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea3 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.ChartArea chartArea4 = new System.Windows.Forms.DataVisualization.Charting.ChartArea();
            System.Windows.Forms.DataVisualization.Charting.Legend legend2 = new System.Windows.Forms.DataVisualization.Charting.Legend();
            System.Windows.Forms.DataVisualization.Charting.Series series3 = new System.Windows.Forms.DataVisualization.Charting.Series();
            System.Windows.Forms.DataVisualization.Charting.Series series4 = new System.Windows.Forms.DataVisualization.Charting.Series();
            this.chart1 = new System.Windows.Forms.DataVisualization.Charting.Chart();
            this.panel1 = new System.Windows.Forms.Panel();
            this.checkBoxCloseOnly = new System.Windows.Forms.CheckBox();
            this.comboBoxMarks = new System.Windows.Forms.ComboBox();
            this.label2 = new System.Windows.Forms.Label();
            this.comboBoxChartType = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).BeginInit();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // chart1
            // 
            this.chart1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.chart1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(211)))), ((int)(((byte)(223)))), ((int)(((byte)(240)))));
            this.chart1.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            this.chart1.BackSecondaryColor = System.Drawing.Color.White;
            this.chart1.BorderlineColor = System.Drawing.Color.FromArgb(((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            this.chart1.BorderlineDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            this.chart1.BorderlineWidth = 2;
            this.chart1.BorderSkin.SkinStyle = System.Windows.Forms.DataVisualization.Charting.BorderSkinStyle.Emboss;
            chartArea3.Area3DStyle.Inclination = 15;
            chartArea3.Area3DStyle.IsClustered = true;
            chartArea3.Area3DStyle.IsRightAngleAxes = false;
            chartArea3.Area3DStyle.Perspective = 10;
            chartArea3.Area3DStyle.Rotation = 10;
            chartArea3.Area3DStyle.WallWidth = 0;
            chartArea3.AxisX.IsLabelAutoFit = false;
            chartArea3.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea3.AxisX.LabelStyle.IsEndLabelVisible = false;
            chartArea3.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisY.IsLabelAutoFit = false;
            chartArea3.AxisY.IsStartedFromZero = false;
            chartArea3.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea3.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(165)))), ((int)(((byte)(191)))), ((int)(((byte)(228)))));
            chartArea3.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea3.BackSecondaryColor = System.Drawing.Color.White;
            chartArea3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea3.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea3.CursorX.IsUserEnabled = true;
            chartArea3.CursorX.IsUserSelectionEnabled = true;
            chartArea3.Name = "Price";
            chartArea3.Position.Auto = false;
            chartArea3.Position.Height = 42F;
            chartArea3.Position.Width = 88F;
            chartArea3.Position.X = 3F;
            chartArea3.Position.Y = 10F;
            chartArea3.ShadowColor = System.Drawing.Color.Transparent;
            chartArea4.AlignWithChartArea = "Price";
            chartArea4.Area3DStyle.Inclination = 15;
            chartArea4.Area3DStyle.IsClustered = true;
            chartArea4.Area3DStyle.IsRightAngleAxes = false;
            chartArea4.Area3DStyle.Perspective = 10;
            chartArea4.Area3DStyle.Rotation = 10;
            chartArea4.Area3DStyle.WallWidth = 0;
            chartArea4.AxisX.IsLabelAutoFit = false;
            chartArea4.AxisX.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea4.AxisX.LabelStyle.IsEndLabelVisible = false;
            chartArea4.AxisX.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea4.AxisX.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea4.AxisY.IsLabelAutoFit = false;
            chartArea4.AxisY.IsStartedFromZero = false;
            chartArea4.AxisY.LabelStyle.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            chartArea4.AxisY.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea4.AxisY.MajorGrid.LineColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea4.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(165)))), ((int)(((byte)(191)))), ((int)(((byte)(228)))));
            chartArea4.BackGradientStyle = System.Windows.Forms.DataVisualization.Charting.GradientStyle.TopBottom;
            chartArea4.BackSecondaryColor = System.Drawing.Color.White;
            chartArea4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            chartArea4.BorderDashStyle = System.Windows.Forms.DataVisualization.Charting.ChartDashStyle.Solid;
            chartArea4.Name = "Volume";
            chartArea4.Position.Auto = false;
            chartArea4.Position.Height = 42F;
            chartArea4.Position.Width = 88F;
            chartArea4.Position.X = 3F;
            chartArea4.Position.Y = 51.84195F;
            chartArea4.ShadowColor = System.Drawing.Color.Transparent;
            this.chart1.ChartAreas.Add(chartArea3);
            this.chart1.ChartAreas.Add(chartArea4);
            legend2.Alignment = System.Drawing.StringAlignment.Far;
            legend2.BackColor = System.Drawing.Color.Transparent;
            legend2.DockedToChartArea = "Price";
            legend2.Docking = System.Windows.Forms.DataVisualization.Charting.Docking.Top;
            legend2.Font = new System.Drawing.Font("Trebuchet MS", 8.25F, System.Drawing.FontStyle.Bold);
            legend2.IsDockedInsideChartArea = false;
            legend2.IsTextAutoFit = false;
            legend2.LegendStyle = System.Windows.Forms.DataVisualization.Charting.LegendStyle.Row;
            legend2.Name = "Default";
            legend2.Position.Auto = false;
            legend2.Position.Height = 7.127659F;
            legend2.Position.Width = 38.19123F;
            legend2.Position.X = 55F;
            legend2.Position.Y = 5F;
            this.chart1.Legends.Add(legend2);
            this.chart1.Location = new System.Drawing.Point(3, 45);
            this.chart1.Name = "chart1";
            series3.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            series3.ChartArea = "Price";
            series3.ChartType = System.Windows.Forms.DataVisualization.Charting.SeriesChartType.Candlestick;
            series3.IsVisibleInLegend = false;
            series3.Legend = "Default";
            series3.Name = "Price";
            series3.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            series3.YValuesPerPoint = 4;
            series4.BorderColor = System.Drawing.Color.FromArgb(((int)(((byte)(180)))), ((int)(((byte)(26)))), ((int)(((byte)(59)))), ((int)(((byte)(105)))));
            series4.ChartArea = "Volume";
            series4.Color = System.Drawing.Color.FromArgb(((int)(((byte)(224)))), ((int)(((byte)(64)))), ((int)(((byte)(10)))));
            series4.IsVisibleInLegend = false;
            series4.Legend = "Default";
            series4.Name = "Volume";
            series4.XValueType = System.Windows.Forms.DataVisualization.Charting.ChartValueType.DateTime;
            this.chart1.Series.Add(series3);
            this.chart1.Series.Add(series4);
            this.chart1.Size = new System.Drawing.Size(725, 387);
            this.chart1.TabIndex = 1;
            this.chart1.MouseDown += new System.Windows.Forms.MouseEventHandler(this.button1_MouseDown);
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.checkBoxCloseOnly);
            this.panel1.Controls.Add(this.comboBoxMarks);
            this.panel1.Controls.Add(this.label2);
            this.panel1.Controls.Add(this.comboBoxChartType);
            this.panel1.Controls.Add(this.label1);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Top;
            this.panel1.Location = new System.Drawing.Point(0, 0);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(728, 39);
            this.panel1.TabIndex = 2;
            // 
            // checkBoxCloseOnly
            // 
            this.checkBoxCloseOnly.CheckAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxCloseOnly.Location = new System.Drawing.Point(568, 8);
            this.checkBoxCloseOnly.Name = "checkBoxCloseOnly";
            this.checkBoxCloseOnly.Size = new System.Drawing.Size(143, 24);
            this.checkBoxCloseOnly.TabIndex = 4;
            this.checkBoxCloseOnly.Text = "&Close Price Only:";
            this.checkBoxCloseOnly.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            this.checkBoxCloseOnly.CheckedChanged += new System.EventHandler(this.checkBox1_CheckedChanged);
            // 
            // comboBoxMarks
            // 
            this.comboBoxMarks.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxMarks.Items.AddRange(new object[] {
            "Line",
            "Triangle"});
            this.comboBoxMarks.Location = new System.Drawing.Point(416, 8);
            this.comboBoxMarks.Name = "comboBoxMarks";
            this.comboBoxMarks.Size = new System.Drawing.Size(112, 22);
            this.comboBoxMarks.TabIndex = 3;
            this.comboBoxMarks.SelectedIndexChanged += new System.EventHandler(this.comboBoxMarks_SelectedIndexChanged);
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(282, 8);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(128, 23);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Open Close Marks:";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // comboBoxChartType
            // 
            this.comboBoxChartType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.comboBoxChartType.Items.AddRange(new object[] {
            "CandleStick",
            "Stock"});
            this.comboBoxChartType.Location = new System.Drawing.Point(101, 8);
            this.comboBoxChartType.Name = "comboBoxChartType";
            this.comboBoxChartType.Size = new System.Drawing.Size(112, 22);
            this.comboBoxChartType.TabIndex = 1;
            this.comboBoxChartType.SelectedIndexChanged += new System.EventHandler(this.comboBoxMarks_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(3, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(92, 23);
            this.label1.TabIndex = 0;
            this.label1.Text = "Chart &Type:";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleRight;
            // 
            // FinancialChartType
            // 
            this.BackColor = System.Drawing.Color.White;
            this.Controls.Add(this.panel1);
            this.Controls.Add(this.chart1);
            this.Font = new System.Drawing.Font("Verdana", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "FinancialChartType";
            this.Size = new System.Drawing.Size(728, 432);
            this.Load += new System.EventHandler(this.FinancialChartType_Load);
            ((System.ComponentModel.ISupportInitialize)(this.chart1)).EndInit();
            this.panel1.ResumeLayout(false);
            this.ResumeLayout(false);

		}
		#endregion

		private void UpdateChartSettings()
		{
			chart1.BeginInit();

			comboBoxChartType.Enabled = true;
			comboBoxMarks.Enabled = true;
			checkBoxCloseOnly.Enabled = true;

			// Load default settings except of the chart's size
			defaultViewStyleStream.Seek(0, SeekOrigin.Begin);
			chart1.Serializer.SerializableContent = "*.*";
			chart1.Serializer.NonSerializableContent = "*.Size";
			chart1.Serializer.Load(defaultViewStyleStream);

			// Set series chart type
			chart1.Series["Price"].ChartType = (SeriesChartType) Enum.Parse( typeof(SeriesChartType), comboBoxChartType.Text, true );

			// Set stock chart attributes
			if(comboBoxChartType.Text == "Stock")
			{
				comboBoxMarks.Enabled = true;
				checkBoxCloseOnly.Enabled = true;
				chart1.Series["Price"]["OpenCloseStyle"] = comboBoxMarks.Text;
				if(checkBoxCloseOnly.Checked)
				{
					chart1.Series["Price"]["ShowOpenClose"] = "Close";
				}

				chart1.Series["Price"]["PointWidth"] = "1.0";
			}
			else
			{
				chart1.Series["Price"]["PointWidth"] = "0.8";
				comboBoxMarks.Enabled = false;
				checkBoxCloseOnly.Enabled = false;
			}

			chart1.Series["Volume"]["PointWidth"] = "0.5";

			chart1.EndInit();
		}

		private void FinancialChartType_Load(object sender, System.EventArgs e)
		{
			// Get image path
            //System.Windows.Forms.DataVisualization.Charting.Utilities.SampleMain.MainForm mainForm = (System.Windows.Forms.DataVisualization.Charting.Utilities.SampleMain.MainForm)this.ParentForm;
            //string imagePath = mainForm.CurrentSamplePath; 
            chart1.ChartAreas["Price"].CursorX.IsUserEnabled = true;
            chart1.ChartAreas["Price"].CursorX.IsUserSelectionEnabled = true;
            chart1.ChartAreas["Price"].CursorY.IsUserEnabled = true;
            chart1.ChartAreas["Price"].CursorY.IsUserSelectionEnabled = true;
            chart1.ChartAreas["Price"].AxisX.ScaleView.Zoomable = true;
            chart1.ChartAreas["Price"].AxisY.ScaleView.Zoomable = true;
            chart1.ChartAreas["Price"].CursorX.AutoScroll = true;
            chart1.ChartAreas["Price"].CursorY.AutoScroll = true;
            
			var imagePath = @"C:\Development\ClientServer\TW4\Tw.Ui.Chart\Images\";

			// Add custom legend items
			LegendItem legendItem = new LegendItem(); 
			legendItem.Name = "Dividend";
			legendItem.ImageStyle = LegendImageStyle.Marker;
			legendItem.MarkerImageTransparentColor = Color.White;
			legendItem.MarkerImage = imagePath + "DividentLegend.bmp";
			chart1.Legends[0].CustomItems.Add(legendItem);

			legendItem = new LegendItem(); 
			legendItem.Name = "Split";
			legendItem.ImageStyle = LegendImageStyle.Marker;
			legendItem.MarkerImageTransparentColor = Color.White;
			legendItem.MarkerImage = imagePath + "SplitLegend.bmp";
            chart1.Legends[0].CustomItems.Add(legendItem);

            if (MMV.Caller == "backtest")
            {
           
                legendItem = new LegendItem();
                legendItem.Name = "Entry";
                legendItem.ImageStyle = LegendImageStyle.Marker;
                legendItem.MarkerImageTransparentColor = Color.White;
                legendItem.MarkerImage = imagePath + "EntryLegend.bmp";
                chart1.Legends[0].CustomItems.Add(legendItem);

                legendItem = new LegendItem();
                legendItem.Name = "Exit";
                legendItem.ImageStyle = LegendImageStyle.Marker;
                legendItem.MarkerImageTransparentColor = Color.White;
                legendItem.MarkerImage = imagePath + "ExitLegend.bmp";
                chart1.Legends[0].CustomItems.Add(legendItem);
            }

			// Populate series data
			FillData();
			
			// Save default appearance
			chart1.Serializer.Save(defaultViewStyleStream);

			comboBoxChartType.SelectedIndex = 0;
			comboBoxMarks.SelectedIndex = 0;
		}

		private void checkBox1_CheckedChanged(object sender, System.EventArgs e)
		{
			UpdateChartSettings();
		}

		private void radioButtonCGI_CheckedChanged(object sender, System.EventArgs e)
		{
			UpdateChartSettings();
		}

		private void comboBoxMarks_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			UpdateChartSettings();
		}

		/// <summary>
		/// Random Stock Data Generator
		/// </summary>
        /// 
        private List<Price> GetPrices()
        {
            var v = new Tw.Service.Helper.ViewHelper();
            List<Price> prices = v.PriceByCompany(MMV.Company, MMV.StartDate, MMV.EndDate, MMV.Interval);
            prices.Reverse();
            return prices;
        }

        
        private void ResetZoom()
        {
            chart1.ChartAreas["Price"].AxisX.ScaleView.ZoomReset(0);
            chart1.ChartAreas["Price"].AxisY.ScaleView.ZoomReset(0);
        }

        private void button1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                //do something
            }
            if (e.Button == MouseButtons.Right)
            {
                ResetZoom();
            }
        }

        string MarkerTip(Trade t,bool entry)
        {
            var total = 0.0M;

            if (!entry)
                total = ((t.Shares * t.Price) - t.Commission);
            else
                total = ((t.Shares * t.Price) + t.Commission);

            return t.Date.ToString() + " - " +
                   t.Description + ": " + 
                   t.TradeType + " " + 
                   t.PositionType + " " + 
                   total + "(" + t.Shares + "@" + t.Price + "+/-" +  t.Commission + ")";
        }

		private void FillData()
		{
            var imagePath = @"C:\Development\ClientServer\TW4\Tw.Ui.Chart\Images\";

            List<Price> prices = GetPrices();

            if(prices.Count==0) return;

            var p = prices[0];
            var d = Tw.Service.Common.Utility.Date.ToDate(p.Date);
            chart1.Series["Volume"].Points.AddXY(d, p.Volume);
            chart1.Series["Price"].Points.AddXY(d, (double)p.High);
            chart1.Series["Price"].Points[0].YValues[1] = (double)p.Low;
            chart1.Series["Price"].Points[0].YValues[2] = (double)p.Open;
            chart1.Series["Price"].Points[0].YValues[3] = (double)p.Close;

            for (int i = 1; i < prices.Count; i++ )
            {
                p = prices[i];
                d = Tw.Service.Common.Utility.Date.ToDate(p.Date);
                chart1.Series["Price"].Points.AddXY(d, (double)p.High);
                chart1.Series["Price"].Points[i].XValue = chart1.Series["Price"].Points[i - 1].XValue + 1;
                chart1.Series["Price"].Points[i].YValues[1] = (double)p.Low;
                chart1.Series["Price"].Points[i].YValues[2] = (double)p.Open;
                chart1.Series["Price"].Points[i].YValues[3] = (double)p.Close;
                chart1.Series["Volume"].Points.AddXY(d, p.Volume);
                chart1.Series["Volume"].Points[i].XValue = chart1.Series["Volume"].Points[i - 1].XValue + 1;
                //set trade indicators
                var t = MMV.Trade.Find(x => x.Date == p.Date);
                if (t != null)
                {
                    if (((t.PositionType == "long") && (t.TradeType == "buy")) || ((t.PositionType == "short") && (t.TradeType == "sell")))
                    {
                        chart1.Series["Price"].Points[i].MarkerImage = imagePath + "EntryMarker.bmp";
                        chart1.Series["Price"].Points[i].MarkerImageTransparentColor = Color.White;
                        chart1.Series["Price"].Points[i].ToolTip = MarkerTip(t, true);
                        //"#VALX{D}\n0.15 - dividend per share";
                    }
                    else
                    {
                        chart1.Series["Price"].Points[i].MarkerImage = imagePath + "ExitMarker.bmp";
                        chart1.Series["Price"].Points[i].MarkerImageTransparentColor = Color.White;
                        chart1.Series["Price"].Points[i].ToolTip = MarkerTip(t, false);
                            //"#VALX{D}\n0.15 - dividend per share";
                    }
                    
                }
            }

		}

	}
}
