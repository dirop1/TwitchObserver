using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TwitchObserver
{
    public partial class Form1 : Form
    {
        ApiHandler api = new ApiHandler();
        List<String> viewers;
        ApiHandler.Stream stream;
        Boolean isRunning = false;
        String channelURL;

        public Form1()
        {
            InitializeComponent();
            progressBar1.Maximum = 4;
            progressBar1.Step = 1;
            txtChannelName.Text = Properties.Settings.Default.channelName;
            numericUpdateInterval.Value = Properties.Settings.Default.updateInterVal;
            setEnables(false);
        }

        private void label1_Click(object sender, EventArgs e)
        {
            
        }
        private void buttonStop_Click(object sender, EventArgs e)
        {
            setEnables(false);
        }

        public string getInfoString(ApiHandler.Stream s)
        {
            DateTime dt = DateTime.Now;
            string data = dt.ToString("HH:mm  dd/MM/yy");
            channelURL = s.channel.url;
            return String.Format("Game: {0}\nViewers: {1}\nStatus: {2}\nURL: {3}\nfollowers: {4}\n\nLast Updated: {5}", s.game,s.viewers,s.channel.status,channelURL,s.channel.followers,data);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (String.IsNullOrEmpty(txtChannelName.Text))
            {
                MessageBox.Show("Insert a channel name", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            setEnables(true);
            Properties.Settings.Default.channelName = txtChannelName.Text;
            Properties.Settings.Default.updateInterVal = Convert.ToInt32(numericUpdateInterval.Value);
            Properties.Settings.Default.Save();
            progressBar1.Value = 0;
            backgroundWorker1.RunWorkerAsync();          
        }

        private void setEnables(bool running)
        {
            isRunning = running;
            timerObserver.Interval = Convert.ToInt32(numericUpdateInterval.Value * 60 * 1000);
            if (running) timerObserver.Start(); else timerObserver.Stop();            
            numericUpdateInterval.Enabled = !running;
            txtChannelName.Enabled = !running;
            buttonStop.Enabled = running;
            buttonStart.Enabled = !running;
            goToTwitchButton.Enabled = running;
            openChat.Enabled = running;
        }

        private void backgroundWorker1_DoWork(object sender, DoWorkEventArgs e)
        {

            try
            {
                stream = api.getStreamInfo(txtChannelName.Text);
                backgroundWorker1.ReportProgress(1);
                viewers = api.getChatInfo(txtChannelName.Text).chatters.viewers;
                backgroundWorker1.ReportProgress(2);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void backgroundWorker1_ProgressChanged(object sender,
            ProgressChangedEventArgs e)
        {
            // Change the value of the ProgressBar to the BackgroundWorker progress.
            progressBar1.Value = e.ProgressPercentage;           
        }

       

        private void backgroundWorker1_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (stream == null)
            {
                infoLabel.Text = "Channel is offline :("; progressBar1.Value = 4;
            }
            else
            {
                pictureBox1.LoadAsync(stream.preview.medium);
                progressBar1.Value = 3;
               pictureBox1.SizeMode = PictureBoxSizeMode.StretchImage;
                infoLabel.Text = getInfoString(stream);
                listBox1.Items.Clear();
                listBox1.Items.AddRange(viewers.ToArray());
                progressBar1.Value = 4;
            }
        }

        private void timerObserver_Tick(object sender, EventArgs e)
        {
            ApiHandler.d(String.Format("Timer ticked! Min: {0},isRunning: {1}",timerObserver.Interval/1000/60,isRunning));
            if (isRunning)
            {
                progressBar1.Value = 0;
                backgroundWorker1.RunWorkerAsync();
            }
        }

        private void goToTwitchButton_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start(channelURL);
        }

        private void openChat_Click(object sender, EventArgs e)
        {
            System.Diagnostics.Process.Start("http://www.twitch.tv/" + txtChannelName.Text + "/chat");
        }
    }
}
