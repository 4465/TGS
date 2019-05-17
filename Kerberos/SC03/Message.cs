using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text.RegularExpressions;
using System.Net;
using System.Net.Sockets;
using System.Windows;

namespace SC03
{
    class Message
    {
        
        public static string des_key_str;
        public Message()
        {

        }

        //加密
        public string Encrypt(string str, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Encoding.Default.GetBytes(str);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);// 密匙
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);// 初始化向量
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            var retB = Convert.ToBase64String(ms.ToArray());
            return retB;
        }

        //解密
        public string Decrypt(string pToDecrypt, string sKey)
        {
            DESCryptoServiceProvider des = new DESCryptoServiceProvider();
            byte[] inputByteArray = Convert.FromBase64String(pToDecrypt);
            des.Key = ASCIIEncoding.ASCII.GetBytes(sKey);
            des.IV = ASCIIEncoding.ASCII.GetBytes(sKey);
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            // 如果两次密匙不一样，这一步可能会引发异常
            cs.FlushFinalBlock();
            return System.Text.Encoding.Default.GetString(ms.ToArray());
        }

        //RSA产生密钥
        public string[] GenerateKeys()
        {
            string[] sKeys = new String[2];
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            sKeys[0] = rsa.ToXmlString(true);   //私钥
            sKeys[1] = rsa.ToXmlString(false);  //公钥
            return sKeys;
        }


        //RSA加密
        public string EncryptString(string sSource, string sPublicKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            string plaintext = sSource;                //明文
            rsa.FromXmlString(sPublicKey);             //公钥加密
            byte[] cipherbytes;                        //byte类型密文
            byte[] byteEn = rsa.Encrypt(Encoding.UTF8.GetBytes("a"), false);
            cipherbytes = rsa.Encrypt(Encoding.UTF8.GetBytes(plaintext), false);
            StringBuilder sbString = new StringBuilder();
            for (int i = 0; i < cipherbytes.Length; i++)
            {
                sbString.Append(cipherbytes[i] + ",");
            }
            return sbString.ToString();
        }

        //RSA解密
        public string DecryptString(String sSource, string sPrivateKey)
        {
            RSACryptoServiceProvider rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(sPrivateKey);                  //私钥解密
            byte[] byteEn = rsa.Encrypt(Encoding.UTF8.GetBytes("a"), false);
            string[] sBytes = sSource.Split(',');
            for (int j = 0; j < sBytes.Length; j++)
            {
                if (sBytes[j] != "")
                {
                    byteEn[j] = Byte.Parse(sBytes[j]);
                }
            }
            byte[] plaintbytes = rsa.Decrypt(byteEn, false);
            return Encoding.UTF8.GetString(plaintbytes);
        }

        //string转byte
        public byte[] StringtoBytes(string s)
        {
            byte[] b;
            b = Encoding.ASCII.GetBytes(s);
            return b;
        }

        public string GetHostIP()
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
                    Console.WriteLine(IpEntry.AddressList[i].ToString());
                    return IpEntry.AddressList[i].ToString();
                }
                else
                {
                    return "111";
                }
            }
            return "135";

        }

        public string[] DeMsgFromClient(string message)
        {
            //message = "300003SER12345678LWW1111192.168.43.1931111TGS2019/05/12 12:37:591000LWW1111192.168.11112019/05/12 12:48:26";
            //8 3 13 3 18 4   
            Console.WriteLine(message);
            //Console.WriteLine(message.Length);
            //消息头 300003
            string str_head = message.Substring(0, 11);
            string str_type = message.Substring(0, 2);  //1
            string str_pwd = message.Substring(2, 4);   //4
            string str_tag = message.Substring(6, 2);   //1  
            string str_IDs = message.Substring(8, 3);
            string tail = message.Substring(11, message.Length - 11);
            //ticket 12345678 LWW 1111 192.168.43.193 1111 TGS2019/05/11 16:37:591000
            string[] str_tkt = Regex.Split(tail, "####", RegexOptions.IgnoreCase);
            //AS生成，TGS与C共享的会话密钥，用来加密TGS发送给给C的整体报文
            string str_des_key = str_tkt[0].Substring(0, 8);  
            string str_IDc = str_tkt[0].Substring(8, 3);
            string str_ADC_tkt = str_tkt[1];
            string str_IDtgs = str_tkt[2].Substring(0, 3);  //3
            string str_TS2 = str_tkt[2].Substring(3, 19);
            string str_lifetime1 = str_tkt[2].Substring(22, 4);
            string tkt = str_des_key + str_IDc + str_ADC_tkt + str_IDtgs + str_TS2 + str_lifetime1;
            //300003SER12345678LWW1111192.168.43.1931111TGS2019/05/11 16:37:591000LWW1111192.168.1.10311112019/05/11 16:38:26
            //消息尾
            string str_Aut_IDc = str_tkt[2].Substring(26, 3);
            string str_ADc = str_tkt[3];
            string str_TS3 = str_tkt[4];
            string str_Aut = str_Aut_IDc + str_ADc + str_TS3;
            //打印消息头
            Console.WriteLine("Type:{0}", str_type);
            Console.WriteLine("pwd:{0}", str_pwd);
            Console.WriteLine("Tag:{0}", str_tag);
            Console.WriteLine("IDs:{0}", str_IDs);
            //打印ticket
            Console.WriteLine("访问TGS的票据:{0}", tkt);
            Console.WriteLine("C与TGS共享的会话密钥:{0}", str_des_key);
            Console.WriteLine("IDc:{0}", str_IDc);
            Console.WriteLine(str_ADC_tkt);
            Console.WriteLine("TGS的ID:{0}", str_IDtgs);
            Console.WriteLine("TS2:{0}", str_TS2);
            Console.WriteLine("生命周期:{0}", str_lifetime1);
            //打印消息尾
            Console.WriteLine("签名:{0}", str_Aut);
            Console.WriteLine("IDc:{0}", str_Aut_IDc);
            Console.WriteLine("ADc:{0}", str_ADc);
            Console.WriteLine("TS3:{0}", str_TS3);
            string[] data = new string[4];
            data[0] = "";
            data[1] = "";
            data[2] = "";
            data[3] = "";
            int lifetime1 = Convert.ToInt32(str_lifetime1);
            //int lifetime1;
            lifetime1 = 9999;
            DateTime TS2 = DateTime.Parse(str_TS2);
            if (DateTime.Compare(TS2.AddSeconds(lifetime1), DateTime.Now) > 0)
            {
                //TS2判断消息有效,解密签名，暂用str_Aut代表解密后的签名
                //string Authenticator = De_Authenticator()
                //完成TGS与Client同步
                DateTime TS3 = DateTime.Parse(str_TS3);
                if (DateTime.Compare(TS3.AddSeconds(lifetime1), DateTime.Now) > 0)
                {
                    //检查IDtgs是不是自己
                    if (str_IDtgs == "TGS")
                    {
                        //构造报文
                        Message msg = new Message();
                        string[] keys = msg.GenerateKeys();
                        DateTime TS4 = DateTime.Now;
                        long lifetime = 1000;
                        //string key = "asdf4568";
                        TGSMessage tgs = new TGSMessage(str_des_key, str_IDs, TS4, str_IDc, str_ADc, lifetime);
                        Console.WriteLine("确认成功");
                        Console.WriteLine("TGS发送给Client密文:{0}", Encoding.ASCII.GetString(tgs.msg4_tgs));
                        string message_tgs = Encoding.ASCII.GetString(tgs.msg4_tgs);
                        //用str_des_key解密
                        string De_msg4_tgs = msg.Decrypt(message_tgs.Substring(6, message_tgs.Length - 6), str_des_key);
                        Console.WriteLine("Client解密:{0}", De_msg4_tgs);
                        //Console.WriteLine(De_msg4_tgs.Length);
                        //票据明文
                        string m_tkt = De_msg4_tgs.Substring(32, De_msg4_tgs.Length - 32);
                        Console.WriteLine("票据密文:{0}", m_tkt);
                        //用TGS与V固定的密钥解密TGSyoSER
                        
                        //Console.WriteLine("票据明文:{0}", msg.Decrypt(m_tkt, "TGStoSER"));

                        string msg4 = De_msg4_tgs.Substring(0, 31) + m_tkt;
                        string str_msg4 = Encoding.ASCII.GetString(tgs.msg4_tgs);
                        data[1] = msg4;   //message4全明文
                        data[2] = str_msg4;    //message4密文
                        //
                        data[3] = tgs.key_des_CV;
                        return data;
                    }
                    else
                    {
                        //消息无效
                        Console.WriteLine("IDtgs不是自己");
                        data[0] = "101";
                        return data;
                    }
                }
                else
                {
                    //消息无效
                    Console.WriteLine("TS3失效");
                    data[0] = "102";
                    return data;
                }
            }
            else
            {
                //消息无效
                Console.WriteLine("TS2时间:{0}  现在时间:{1}", TS2.AddSeconds(lifetime1).ToString("yyyy/MM/dd HH:mm:ss"), DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss"));
                Console.WriteLine("TS2失效");
                data[0] = "103";
                return data;
            }
        }

        //参数为Authenticator报文、Client公钥
        public string[] Authenticator(Byte[] ss)
        {
            //string mess = Encoding.ASCII.GetString(ByteRec);
            string[] data = new string[4];
            Message Msg = new Message();
            string mess = Encoding.ASCII.GetString(ss);
            string message = mess.Substring(11, mess.Length - 11);
            Console.WriteLine(message);
            string[] msg = Regex.Split(message, "####", RegexOptions.IgnoreCase);
            Console.WriteLine("Tickt_tgs:{0}",msg[0]);  
            Console.WriteLine("Authenticator:{0}",msg[1]);
            string msg0 = Msg.Decrypt(msg[0], "ASandTGS");
            string str_des_key = msg0.Substring(0, 8);
            Console.WriteLine("密钥:{0}", str_des_key);
            //AS与TGS事先约定好的密钥
            
            Console.WriteLine("msg0:{0}", msg0);
            //int index = msg[1].IndexOf('\0');
            //int length = msg[1].Length;
            //str_des_key解密
            string msg1 = Msg.Decrypt(msg[1], str_des_key);
            Console.WriteLine("msg1:{0}", msg1);
            data[0] = "";
            //接收信息的全明文
            data[0] = mess.Substring(0, 11) + msg0 + msg1;
            Console.WriteLine(data[0]);
            //Console.WriteLine(msg0);
            //Console.WriteLine(msg1);
            //发送信息的全明文
            data[1] = Msg.DeMsgFromClient(data[0])[1];
            //发送信息的全秘文
            data[2] = Msg.DeMsgFromClient(data[0])[2];
            //data[2] = "";
            data[3] = Msg.DeMsgFromClient(data[0])[3];
            return data;
        }
    }
}
