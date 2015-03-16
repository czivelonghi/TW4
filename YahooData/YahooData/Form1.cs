using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Newtonsoft.Json;

namespace YahooData
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            txtUrl.Text = Url();
        }

        private string Url()
        {
            return "http://query.yahooapis.com/v1/public/yql?q=SELECT%20*%20FROM%20yahoo.finance.options%20WHERE%20symbol%3D%22" + txtSymbol.Text
                    + "%22AND%20expiration%3D%22" + txtExpire.Text + "%22&format=json&env=store%3A%2F%2Fdatatables.org%2Falltableswithkeys&callback=";
        
        }

        //http://json.codeplex.com/
        private void btnGo_Click(object sender, EventArgs e)
        {
            txtResult.Text = "";
            
            using (var webClient = new System.Net.WebClient())
            {
                var json = webClient.DownloadString(Url());
                YahooData.RootObject obj = JsonConvert.DeserializeObject<YahooData.RootObject>(json);

                if (obj.query.results.optionsChain.option != null)
                {
                    var oc = obj.query.results.optionsChain;
                    var sb = new System.Text.StringBuilder();
                    sb.AppendLine("SYMBOL: " + oc.symbol.PadRight(15) + "EXPIRES: " + oc.expiration);
                    sb.AppendLine("-----------------------------------------------------------------");
                    foreach (var o in oc.option)
                    {
                        sb.AppendLine("Symbol: " + o.symbol.PadRight(30) +
                            "Type: " + (o.type == "C" ? "call" : "put").PadRight(10) +
                                        "Strike: " + o.strikePrice.PadRight(10) +
                                        "Ask/Bid: " + o.ask + "/" + o.bid.PadRight(15) +
                                        "Vol: " + o.vol.PadRight(10) +
                                        "Open Int: " + o.openInt.PadRight(10));
                    }
                    txtResult.Text = sb.ToString();
                }
                else
                    txtResult.Text = "No results found";
            }
        }
    }
}
