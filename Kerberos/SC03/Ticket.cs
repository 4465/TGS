using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SC03
{
    class TGSMessage
    {

        public string type;
        public byte[] msg4_IDs;
        public byte[] msg4_TS4;
        public byte[] msg4_tkt;
        public string Ramdon_key_CV;
        public string msg4_password;
        public string data_tag;
        public byte[] msg4_tgs;
        Message msg = new Message();
        //生成报文，Key_tgs解析报文所得，是C与TGS共享的会话密钥，加密整个报文
        public TGSMessage(string Key_tgs, string IDs, DateTime TS4, string IDc, string ADc, long lifetime2)
        {
            this.type = "04";
            this.msg4_password = "0000";
            List<byte> tgsMessage = new List<byte>();
            byte[] message;
            message = Message_4(Key_tgs, IDs, TS4, IDc, ADc, lifetime2);
            tgsMessage.AddRange(Encoding.ASCII.GetBytes(this.type.ToString()));
            tgsMessage.AddRange(Encoding.ASCII.GetBytes(this.msg4_password.ToString()));
            tgsMessage.AddRange(message);
            byte[] Tk = tgsMessage.ToArray();
            this.msg4_tgs = Tk;
        }
        //生成报文数据段，Key_C_TGS解析报文所得，有Ticket()传参所得，是C与TGS共享的会话密钥，加密整个报文
        public byte[] Message_4(string Key_C_TGS, string IDs, DateTime TS4, string IDc, string ADc, long lifetime2)
        {
            this.data_tag = "04";
            this.msg4_IDs = msg.StringtoBytes(IDs);
            this.msg4_TS4 = msg.StringtoBytes(TS4.ToString("yyyy/MM/dd HH:mm:ss"));
            //TGS生成，C与V共享的会话密钥
            string str_des_CV = "12345678";
            this.Ramdon_key_CV = GetRandomString(8);
            this.msg4_tkt = Ticket(IDs, str_des_CV, IDc, ADc, lifetime2);
            List<byte> tgsMessage = new List<byte>();
            tgsMessage.AddRange(Encoding.ASCII.GetBytes(this.data_tag.ToString()));
            tgsMessage.AddRange(Encoding.ASCII.GetBytes(str_des_CV));
            tgsMessage.AddRange(msg4_IDs);
            tgsMessage.AddRange(msg4_TS4);
            tgsMessage.AddRange(msg4_tkt);
            Console.WriteLine("Ticket_v:{0}", Encoding.ASCII.GetString(msg4_tkt));
            //Console.WriteLine("票据解密:{0}", msg.Decrypt(Encoding.ASCII.GetString(msg4_tkt), "TGSToSER"));
            byte[] Tk = tgsMessage.ToArray();
            //client与TGS加解密
            string str_Tk = Encoding.ASCII.GetString(Tk);
            //整个报文加密，key=12345678这是AS生成的，C和TGS之间共享的会话密钥 通过解析Ticket_tgs票据获得
            string En_str_Tk = msg.Encrypt(str_Tk, "12345678");
            Tk = Encoding.ASCII.GetBytes(En_str_Tk);
            return Tk;
        }

        //服务器密码加密  key = TGSandS_
        public byte[] Ticket(string IDs, string Key_C_S, string IDc, string ADc, long lifetime2)
        {
            //Console.WriteLine("IDc:{0}", IDc);
            List<byte> ticket = new List<byte>();
            ticket.AddRange(msg.StringtoBytes(Key_C_S));
            ticket.AddRange(Encoding.ASCII.GetBytes(IDc));
            ticket.AddRange(Encoding.ASCII.GetBytes("####"));
            ticket.AddRange(Encoding.ASCII.GetBytes(ADc));
            ticket.AddRange(Encoding.ASCII.GetBytes("####"));
            ticket.AddRange(Encoding.ASCII.GetBytes(IDs));
            ticket.AddRange(Encoding.ASCII.GetBytes(DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")));
            ticket.AddRange(msg.StringtoBytes(lifetime2.ToString()));
            byte[] Tk = ticket.ToArray();
            string str_Tk = Encoding.ASCII.GetString(Tk);
            //对票据进行加密  key = 12345678这是c和v之间约定的密钥
            string En_str_Tk = msg.Encrypt(str_Tk, "12345678");
            Tk = Encoding.ASCII.GetBytes(En_str_Tk);
            return Tk;
        }

        private static string GetRandomString(int length)
        {
            const string key = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
            if (length < 1)
                return string.Empty;
            Random rnd = new Random();
            byte[] buffer = new byte[8];
            ulong bit = 31;
            ulong result = 0;
            int index = 0;
            StringBuilder sb = new StringBuilder((length / 5 + 1) * 5);
            while (sb.Length < length)
            {
                rnd.NextBytes(buffer);
                buffer[5] = buffer[6] = buffer[7] = 0x00;
                result = BitConverter.ToUInt64(buffer, 0);
                while (result > 0 && sb.Length < length)
                {
                    index = (int)(bit & result);
                    sb.Append(key[index]);
                    result = result >> 5;
                }
            }
            return sb.ToString();
        }
    }
}
