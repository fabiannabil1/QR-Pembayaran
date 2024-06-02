using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Net.Mime.MediaTypeNames;
using QRCoder;


namespace WinFormsApp2
{
    public partial class Form1 : Form
    {
        string kode_qr = "https://steel-hans-porter-voters.trycloudflare.com";
        private static readonly HttpClient client = new HttpClient();
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public Form1()
        {
            InitializeComponent();
            Task.Run(() => MonitorCloseFlag(cancellationTokenSource.Token));
        }
        private async Task MonitorCloseFlag(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                try
                {
                    var response = await client.GetStringAsync(kode_qr + "/check");
                    dynamic json = Newtonsoft.Json.JsonConvert.DeserializeObject(response);
                    bool closeFlag = json.close_flag;

                    if (closeFlag)
                    {
                        MessageBox.Show("Pembayaran Berhasil ;)");
                        Thread.Sleep(4000);
                        Invoke(new Action(() => this.Close()));
                        break;
                    }
                }
                catch (Exception ex)
                {
                    // Handle exceptions (e.g., network issues)
                    Console.WriteLine(ex.Message);
                }

                // Wait for a while before checking again
                await Task.Delay(1000, cancellationToken);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Cancel the monitoring task
            cancellationTokenSource.Cancel();
            base.OnFormClosing(e);
        }
        private void Form1_Load(object sender, EventArgs e)
        {
            string url_trigger = kode_qr + "/set";
            QRCoder.QRCodeGenerator qr_baru = new QRCoder.QRCodeGenerator();
            var Data_qr = qr_baru.CreateQrCode(url_trigger, QRCodeGenerator.ECCLevel.H);
            var QR = new QRCoder.QRCode(Data_qr);
            qr_box.Image = QR.GetGraphic(100);

        }

        private void qr_box_Click(object sender, EventArgs e)
        {

        }

        private void pictureBox1_Click(object sender, EventArgs e)
        {
        }
    }
}
