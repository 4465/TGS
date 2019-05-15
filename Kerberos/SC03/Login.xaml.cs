using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Net;
using System.Net.Sockets;

namespace SC03
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        Message msg = new Message();
        public MainWindow()
        {
            InitializeComponent();
            LB.Items.Add("斗破苍穹");
            LB.Items.Add("55575");
            LB.Items.Add("1213");
        }

        public void Print()
        {

        }

        private void Btn_login_Click(object sender, RoutedEventArgs e)
        {
            byte[] byteMsg = new byte[2];
            byteMsg[0] = 0x01;
            byteMsg[1] = 0x02;
            TB_username.Text = Encoding.ASCII.GetString(byteMsg, 0, 2);
            //TB_passwd.Text = byteMsg[1];
            Window tgs = new TGS();
            tgs.Show();

        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { TB_username.Text=LB.SelectedItem.ToString(); }));
        }
    }
}
