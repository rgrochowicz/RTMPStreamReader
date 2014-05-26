using System;
using System.IO;
using System.Windows.Forms;
using RTMPStreamReader.RTMP;

namespace RTMPStreamReader
{
    public partial class MainForm : Form
    {
        private readonly string _fileName;

        private Client _currentClient;
        private Stream _inputStream;
        private Stream _outputStream;

        public MainForm(string fileName)
        {
            _fileName = fileName;
            InitializeComponent();
        }

        public void Go()
        {
            _inputStream = File.OpenRead(_fileName);
            _outputStream = new BufferedStream(File.OpenWrite("stream.flv"), 200000000);

            _currentClient = new Client(_outputStream);

            pbRead.Maximum = (int) _inputStream.Length;

            _currentClient.OnPacketReceived += currentClient_OnPacketReceived;
            _currentClient.Connect(_inputStream);

            timerUpdate.Start();
        }

        private void currentClient_OnPacketReceived(object sender, EventArgs e)
        {
            if (_currentClient.Stream.Length != _currentClient.Stream.Position)
                return;
            timerUpdate.Stop();

            _outputStream.Flush();

            _outputStream.Close();
            _inputStream.Close();
            Application.Exit();

            _currentClient.ForceStop();
        }

        private void timerUpdate_Tick(object sender, EventArgs e)
        {
            if (_currentClient == null)
                return;
            pbRead.Value = (int) _currentClient.Stream.Position;
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            Go();
        }
    }
}