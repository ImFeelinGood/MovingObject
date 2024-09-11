using System;
using System.Drawing;
using System.Net.Sockets;
using System.Text;
using System.Windows.Forms;

namespace MovingObjectClient
{
    public partial class ClientForm : Form
    {
        private Rectangle rect = new Rectangle(20, 20, 30, 30);
        private Pen red = new Pen(Color.Red);
        private SolidBrush fillBlue = new SolidBrush(Color.Blue);
        private Socket clientSocket;
        private byte[] buffer;

        public ClientForm()
        {
            InitializeComponent();
            ConnectToServer();
        }

        private void ConnectToServer()
        {
            try
            {
                clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                clientSocket.BeginConnect("127.0.0.1", 3333, ConnectCallback, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ConnectCallback(IAsyncResult AR)
        {
            try
            {
                clientSocket.EndConnect(AR);
                buffer = new byte[clientSocket.ReceiveBufferSize];
                clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ReceiveCallback(IAsyncResult AR)
        {
            try
            {
                int received = clientSocket.EndReceive(AR);
                if (received > 0)
                {
                    string data = Encoding.ASCII.GetString(buffer, 0, received);
                    string[] positionData = data.Split(',');

                    if (positionData.Length == 2)
                    {
                        int newX, newY;

                        if (int.TryParse(positionData[0], out newX) && int.TryParse(positionData[1], out newY))
                        {
                            rect.X = newX;
                            rect.Y = newY;
                            Invalidate();
                        }
                    }

                    clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, ReceiveCallback, null);
                }
            }
            catch (SocketException ex)
            {
                MessageBox.Show(ex.Message, "Connection Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void ClientForm_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }

        private void ClientForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (clientSocket != null && clientSocket.Connected)
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}
