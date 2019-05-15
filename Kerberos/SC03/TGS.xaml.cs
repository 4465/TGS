using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows;

namespace SC03
{
    /// <summary>
    /// AS.xaml 的交互逻辑
    /// </summary>
    /// <summary>
    public partial class TGS : Window
    {
        private Socket connection;
        private TcpListener listener;
        private IPAddress ip;
        private Int32 port;
        //public string Time = DateTime.Now.ToString("yyyy/MM/dd HH：mm：ss");
        private static byte[] result = new byte[1024]; 
        Message msg = new Message();
        public TGS()
        {
            string host = GetLocalIP();
            IPAddress ip = IPAddress.Parse(host);
            InitializeComponent();
            this.ip = ip;
            this.port = 9000;
        }

        public string get_remote_ip()
        {
            connection = listener.AcceptSocket();
            //在新线程中启动新的socket连接，每个socket等待，并保持连接
            IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
            return iprm.Address.ToString();
        }

        //侦听客户连接请求
        public void runAs()
        {
            while (true)
            {
                this.Dispatcher.Invoke(new Action(() => { TB_log.AppendText("正在监听..."); }));
                connection = listener.AcceptSocket();
                //在新线程中启动新的socket连接，每个socket等待，并保持连接
                IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
                this.Dispatcher.Invoke(new Action(() => { TB_log.AppendText("远程主机:" + iprm.Address.ToString() + ":" + iprm.Port.ToString() + "连接上本机\r\n"); }));

                Thread thread = new Thread(new ThreadStart(dealClient));
                Thread myThread = new Thread(dealClient);
                thread.Start();
            }
        }

        //和客户端对话
        private void dealClient()
        {
            Socket connection = this.connection;
            IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
            this.Dispatcher.Invoke(new Action(() => { TB_log.AppendText("准备接受消息！\n"); }));
            Thread receiveThread = new Thread(ReceiveMessage);
            receiveThread.Start(connection);
        }


        public void ReceiveMessage(object ClientSocket)
        {
            Socket myClientSocket = (Socket)ClientSocket;
            IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
            while (true)
            {
                try
                {
                    //通过clientsocket接收数据
                    int num = myClientSocket.Receive(result);
                    this.Dispatcher.Invoke(new Action(() => { TB_recv_2.AppendText(Encoding.ASCII.GetString(result, 0, num)); }));
                    Thread sendThread = new Thread(SendMessage);
                    sendThread.Start(myClientSocket);
                    break;
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    //myClientSocket.Shutdown(SocketShutdown.Both);
                    //myClientSocket.Close();
                    break;
                }
            }
        }
        public void SendMessage(object clientSocket)
        {

            Socket myClientSocket = (Socket)clientSocket;
            IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
            //this.Dispatcher.Invoke(new Action(() => { TB_send_1.AppendText(ssmg); }));
            //string sendStr = "40000WXSCd/8k4O7b8v2WbUJ+RGOuO/n4TD2S4adxWheNocrnQfkEfEtOkvlRochjKvOgVG7vILx0bKQEDTaDylPHTEioKwxy4oX2lsswZKKoy4aBOWEcepwPn9itkq7l0OE4VxXPH1PsMsjB8uxn2F9MXg == ";
            string[] Str = msg.Authenticator(result);
            string sendStr = Str[2];       //发送消息全秘闻
            byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
            this.Dispatcher.Invoke(new Action(() => { TB_send_2.AppendText(sendStr); }));
            this.Dispatcher.Invoke(new Action(() => { TB_send_1.AppendText(Str[1]); }));  //发送信息的全明文
            this.Dispatcher.Invoke(new Action(() => { TB_recv_1.AppendText(Str[0]); }));  //接收信息的全明文
            myClientSocket.Send(sendByte, sendByte.Length, 0);
            //Thread receiveThread = new Thread(ReceiveMessage);
            //receiveThread.Start(connection);
            //myClientSocket.Close();
        }


        private void Init(object sender, RoutedEventArgs e)
        {
            listener = getListener(ip, port);
            Thread thread = new Thread(new ThreadStart(runAs));
            thread.Start();
        }

        public static TcpListener getListener(IPAddress address, Int32 port)
        {
            try
            {
                TcpListener listener = new TcpListener(address, port);
                listener.Start();
                return listener;
            }
            catch (Exception error)
            {
                MessageBox.Show(error.ToString());
            }
            return null;
        }

        //获取自身IP
        public static string GetLocalIP()
        {
            try
            {

                string HostName = Dns.GetHostName(); //得到主机名
                IPHostEntry IpEntry = Dns.GetHostEntry(HostName);
                for (int i = 0; i < IpEntry.AddressList.Length; i++)
                {
                    //从IP地址列表中筛选出IPv4类型的IP地址
                    //AddressFamily.InterNetwork表示此IP为IPv4,
                    //AddressFamily.InterNetworkV6表示此地址为IPv6类型
                    if (IpEntry.AddressList[i].AddressFamily == AddressFamily.InterNetwork)
                    {
                        string ip = "";
                        ip = IpEntry.AddressList[i].ToString();
                        return IpEntry.AddressList[i].ToString();
                    }
                }
                return "";
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
    }
}
