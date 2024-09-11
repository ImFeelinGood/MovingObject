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
        // Pen, Rectangle, dan variabel untuk pergerakan
        Pen red = new Pen(Color.Red);
        Rectangle rect = new Rectangle(20, 20, 30, 30);
        SolidBrush fillBlue = new SolidBrush(Color.Blue);
        int slide = 10;

        // Socket server untuk komunikasi
        private Socket serverSocket;
        private List<Socket> clientSockets = new List<Socket>();
        private byte[] buffer = new byte[1024];

        public ServerForm()
        {
            InitializeComponent();
            timer1.Interval = 50;
            timer1.Enabled = true;

            // Memulai server socket
            StartServer();
        }

        // Memulai server untuk mendengarkan koneksi klien
        private void StartServer()
        {
            serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            serverSocket.Bind(new IPEndPoint(IPAddress.Any, 3333));
            serverSocket.Listen(5);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        // Menerima koneksi dari klien
        private void AcceptCallback(IAsyncResult ar)
        {
            Socket clientSocket = serverSocket.EndAccept(ar);
            clientSockets.Add(clientSocket);
            serverSocket.BeginAccept(AcceptCallback, null);
        }

        // Mengirimkan data posisi ke klien
        private void SendDataToClients()
        {
            string positionData = $"{rect.X},{rect.Y}";  // Kirim posisi X dan Y
            byte[] data = Encoding.ASCII.GetBytes(positionData);

            foreach (var client in clientSockets)
            {
                try
                {
                    client.Send(data);
                }
                catch (SocketException)
                {
                    // Jika terjadi kesalahan, tutup socket dan keluarkan dari list
                    client.Close();
                    clientSockets.Remove(client);
                    break;
                }
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            // Menggerakkan kubus
            rect.X += slide;
            back();

            // Kirim data posisi ke semua klien
            SendDataToClients();

            // Menggambar ulang di server
            Invalidate();
        }

        private void back()
        {
            if (rect.X >= this.Width - rect.Width * 2)
                slide = -10;
            else if (rect.X <= rect.Width / 2)
                slide = 10;
        }

        // Menggambar objek di form
        private void Form1_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.DrawRectangle(red, rect);
            g.FillRectangle(fillBlue, rect);
        }
    }
}
