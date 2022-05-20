using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Network_Monitor_wpf1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window


{ 
      
        public MainWindow()
        {
            InitializeComponent();
            Getnetwork();
        }

  
        public async void Getnetwork()
        {
            if (!NetworkInterface.GetIsNetworkAvailable())
                return;

            NetworkInterface[] interfaces = NetworkInterface.GetAllNetworkInterfaces();


            foreach (NetworkInterface ni in interfaces)
            {
                string status = ni.OperationalStatus.ToString();
                string bytesent = ni.GetIPv4Statistics().BytesSent.ToString();
                if (status != "Down" && bytesent != "0")
                {
                    var firstbr = ni.GetIPv4Statistics().BytesReceived;
                    var firsts = ni.GetIPv4Statistics().BytesSent;
                    var sw = new Stopwatch();
                    var readx = Enumerable.Empty<double>();
                    int x = 0;
                    while (true)
                    {
                        x++;
                        sw.Restart();
                        await Task.Delay(100);
                        var elapsed = sw.Elapsed.TotalSeconds;
                        var secondbr = ni.GetIPv4Statistics().BytesReceived;
                        var seconds = ni.GetIPv4Statistics().BytesSent;
                        var locals = (seconds - firsts) / elapsed;
                        firsts = seconds;
                        var localbr = (secondbr - firstbr) / elapsed;
                        firstbr = secondbr;
                        // Keep last 20, ~2 seconds
                        readx = new[] { localbr }.Concat(readx).Take(20);
                        readx = new[] { locals }.Concat(readx).Take(20);
                        if (x % 10 == 0)
                        { // ~1 second
                            var sSec = readx.Sum() / readx.Count();
                            var sSeckbps = (sSec * 8) / 1024;                       
                            var brsec = readx.Sum() / readx.Count();
                            var brseckbps = (brsec * 8) / 1024;                          
                            txbdownload.Text = "Upload ~ " + Math.Round(sSeckbps, 2) + " kbps";
                            txbupload.Text = "Download ~ " + Math.Round(brseckbps, 2) + " kbps";                        
                        }
                    }
                }
            }
        }
    }
}
