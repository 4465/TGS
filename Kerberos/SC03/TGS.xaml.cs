﻿using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
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
        Message Msg = new Message();
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

        //public string get_remote_ip()
        //{
        //    connection = listener.AcceptSocket();
        //    //在新线程中启动新的socket连接，每个socket等待，并保持连接
        //    IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
        //    return iprm.Address.ToString();
        //}

        //侦听客户连接请求
        public void runTGS()
        {
            while (true)
            {
                this.Dispatcher.Invoke(new Action(() => { TB_log.AppendText("正在监听..."); }));
                connection = listener.AcceptSocket();
                //在新线程中启动新的socket连接，每个socket等待，并保持连接
                IPEndPoint iprm = (IPEndPoint)connection.RemoteEndPoint;
                this.Dispatcher.Invoke(new Action(() => { TB_log.AppendText(DateTime.Now.ToString() + "远程主机:" + iprm.Address.ToString() + ":" + iprm.Port.ToString() + "连接上本机\r\n"); }));
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
                    Console.WriteLine("result：{0}", Encoding.ASCII.GetString(result));
                    //System.Windows.MessageBox.Show(Encoding.ASCII.GetString(result));
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

        //public string[] dealAut(string mm)
        //{
        //    string[] send = new string[3];
        //    send[0] = " ";
        //    send[1] = " ";
        //    send[2] = " ";
        //    string mess = Encoding.ASCII.GetString(result);
        //    string message = mess.Substring(11, mess.Length - 11);
        //    Console.WriteLine(message);
        //    string[] msg = Regex.Split(message, "####", RegexOptions.IgnoreCase);
        //    Console.WriteLine("Tickt_tgs:{0}", msg[0]);
        //    Console.WriteLine("Authenticator:{0}", msg[1]);
        //    string msg0 = Msg.Decrypt(msg[0], "ASandTGS");
        //    string str_des_key = msg0.Substring(0, 8);
        //    Console.WriteLine("密钥:{0}", str_des_key);
        //    string Aut = Msg.Decrypt(msg[1], str_des_key);
        //    string IDc = Aut.Substring(0, 3);
        //    send[0] = IDc;
        //    send[1] = str_des_key;
        //    send[2] = Encoding.ASCII.GetString(result);
            
        //    return send;
        //}

        public void SendMessage(object clientSocket)
        {
            //TGS tgs = new TGS();
            //string[] send = new string[2];
            //send = this.dealAut(Encoding.ASCII.GetString(result));
            //Console.WriteLine("S数组:{0}\n{1}\n{2}\n", send[0], send[1], send[2]);
            string mess = Encoding.ASCII.GetString(result);
            string message = mess.Substring(11, mess.Length - 11);
            Console.WriteLine(message);
            string[] msg = Regex.Split(message, "####", RegexOptions.IgnoreCase);
            Console.WriteLine("Tickt_tgs:{0}", msg[0]);
            Console.WriteLine("Authenticator:{0}", msg[1]);
            string msg0 = Msg.Decrypt(msg[0], "ASandTGS");
            string str_des_key = msg0.Substring(0, 8);  
            Console.WriteLine("密钥:{0}", str_des_key);
            string Aut = Msg.Decrypt(msg[1], str_des_key);
            string IDc = Aut.Substring(0, 3);
            //string[] tail = Regex.Split(Aut, "####", RegexOptions.IgnoreCase);
            //string ADc = tail[1];
            if (!Dic.myDictionary.ContainsKey(IDc))
            {
                Dic.myDictionary.Add(IDc, str_des_key);
                Console.WriteLine("IDC:{0}",IDc);
                Console.WriteLine("str_des_key:{0}",str_des_key);
            }
            else
            {
                Dic.myDictionary[IDc] = str_des_key;
                
                Console.WriteLine("IDC:{0}", IDc);
                Console.WriteLine("str_des_key:{0}", str_des_key);
            }
            string[] Str = Msg.Authenticator(IDc,result);
            Socket myClientSocket = (Socket)clientSocket;
            
            //this.Dispatcher.Invoke(new Action(() => { TB_send_1.AppendText(ssmg); }));
            //string sendStr = "40000WXSCd/8k4O7b8v2WbUJ+RGOuO/n4TD2S4adxWheNocrnQfkEfEtOkvlRochjKvOgVG7vILx0bKQEDTaDylPHTEioKwxy4oX2lsswZKKoy4aBOWEcepwPn9itkq7l0OE4VxXPH1PsMsjB8uxn2F9MXg == ";
            
            string sendStr = Str[2];       //发送消息全秘文
            byte[] sendByte = Encoding.ASCII.GetBytes(sendStr);
            this.Dispatcher.Invoke(new Action(() => { TB_send_2.AppendText(sendStr); }));
            this.Dispatcher.Invoke(new Action(() => { TB_send_1.AppendText(Str[1]); }));  //发送信息的全明文
            this.Dispatcher.Invoke(new Action(() => { TB_recv_1.AppendText(Str[0]); }));  //接收信息的全明文
            this.Dispatcher.Invoke(new Action(() => { TB_key.AppendText(Str[3]+" "); }));
            myClientSocket.Send(sendByte, sendByte.Length, 0);
            
            //Thread receiveThread = new Thread(ReceiveMessage);
            //receiveThread.Start(connection);
            myClientSocket.Close();
        }


        private void Init(object sender, RoutedEventArgs e)
        {
            listener = getListener(ip, port);
            Thread thread = new Thread(new ThreadStart(runTGS));
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
