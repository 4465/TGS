using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace SC03
{
    public class SocketServer
    {
        TGS tgs = new TGS();
        private string _ip = string.Empty;
        private int _port = 0;
        private Socket _socket = null;
        private byte[] buffer = new byte[1024 * 1024 * 2];
       
        public SocketServer(string ip, int port)
        {
            this._ip = ip;
            this._port = port;
        }
        public SocketServer(int port)
        {
            this._ip = "0.0.0.0";
            this._port = port;
        }

        public SocketServer()
        {
            this._ip = "127.0.0.1";
            this._port = 10000;
        }

        public void StartListen()
        {
            Console.WriteLine("hello");
            tgs.TB_key.Text += "正在监听...";
            try
            {
                //1.0 实例化套接字(IP4寻找协议,流式协议,TCP协议)
                _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                //2.0 创建IP对象
                IPAddress address = IPAddress.Parse(_ip);
                //3.0 创建网络端口,包括ip和端口
                IPEndPoint endPoint = new IPEndPoint(address, _port);
                //4.0 绑定套接字
                _socket.Bind(endPoint);
                //5.0 设置最大连接数
                _socket.Listen(int.MaxValue);
                
                //Console.WriteLine("监听{0}消息成功", _socket.LocalEndPoint.ToString());
                //6.0 开始监听
                Thread thread = new Thread(ListenClientConnect);
                thread.Start();

            }
            catch (Exception ex)
            {

            }
        }
        
        /// 监听客户端连接
        
        private void ListenClientConnect()
        {
            try
            {
                while (true)
                {
                    //Socket创建的新连接
                    Socket clientSocket = _socket.Accept();
                    clientSocket.Send(Encoding.UTF8.GetBytes("服务端发送消息:"));
                    Thread thread = new Thread(ReceiveMessage);
                    thread.Start(clientSocket);
                }
            }
            catch (Exception)
            {
            }
        }

      
        /// 接收客户端消息
       
        private void ReceiveMessage(object socket)
        {
            Socket clientSocket = (Socket)socket;
            while (true)
            {
                try
                {
                    //获取从客户端发来的数据
                    int length = clientSocket.Receive(buffer);
                    Console.WriteLine("接收客户端{0},消息{1}", clientSocket.RemoteEndPoint.ToString(), Encoding.UTF8.GetString(buffer, 0, length));
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                    clientSocket.Shutdown(SocketShutdown.Both);
                    clientSocket.Close();
                    break;
                }
            }
        }
    }
}
