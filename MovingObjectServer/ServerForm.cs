using System;
using System.Collections.Generic;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MovingObject
{
    public partial class ServerForm : Form
    {
        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slide = 10;

        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();
        private byte[] buffer = new byte[1024];

        public ServerForm()
        {
            InitializeComponent();
            timer1.Interval = 50;
            timer1.Enabled = true;

            StartServer();
        }

        private void StartServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3333));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void AcceptCallback(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            clientSockets.Add(clientSocket);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        private void SendDataToClients()
        {
            string positionData = $"{rect.X},{rect.Y}";
            byte[] data = Encoding.ASCII.GetBytes(positionData);

            foreach (var client in clientSockets)
            {
                try
                {
                    client.Send(data);
                }
                catch (SocketException)
                {
                    client.Close();
                    clientSockets.Remove(client);
                    break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            rect.X += slide;
            back();

            SendDataToClients();

            Invalidate();
        }

        private void back()
        {
            if (rect.X >= this.Width - rect.Width * 2)
                slide = -10;
            else if (rect.X <= rect.Width / 2)
                slide = 10;
        }

        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}
