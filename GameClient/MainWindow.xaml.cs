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
using System.ServiceModel;
using GameLibrary;

namespace GameClient
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        IMe me = null;
        public MainWindow()
        {
            InitializeComponent();

            try
            {
                // Connect to the WCF service endpoint called "ShoeService" 
                //ChannelFactory<IShoe> channel = new ChannelFactory<IShoe>(new NetTcpBinding(), 
                //    new EndpointAddress("net.tcp://localhost:13200/CardsLibrary/ShoeService"));
                ChannelFactory<IMe> channel = new ChannelFactory<IMe>("ShoeEndpoint");
                me = channel.CreateChannel();

                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }



        

        private void Btn_Click(object sender, RoutedEventArgs e)
        {
            me=new Me();
            
            try
            {
                me.Name_ = tBox.Text;
                l.Items.Insert(0,me.Name_);


            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }
    }
}
