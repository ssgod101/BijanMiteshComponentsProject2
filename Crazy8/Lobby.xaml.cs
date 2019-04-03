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
using System.Windows.Shapes;
using System.ServiceModel;
using Crazy8Library;

namespace Crazy8
{
    /// <summary>
    /// Interaction logic for Lobby.xaml
    /// </summary>
    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class Lobby : Window,ICallBack
    {
        private IDeck deck = null;
        private string name = "";
        public Lobby()
        {
            InitializeComponent();
         

            try
            {
                // Connect to the WCF service endpoint called "ShoeService" 
                DuplexChannelFactory<IDeck> channel = new DuplexChannelFactory<IDeck>(this, "DeckEndpoint");
                deck = channel.CreateChannel();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }

        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            if (deck.Join(tbName.Text))
            {
                Play.IsEnabled = true;
                name = tbName.Text;
               
                Join.IsEnabled = false;
            }
            else { MessageBox.Show(name + " was not able to join game.","",MessageBoxButton.OK,MessageBoxImage.Error); }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            deck.Leave(name);
            Environment.Exit(0);

        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            this.Close();

        }
        // Implement ICallback contract
        private delegate void ClientUpdateDelegate(CallbackInfo info);

        public void UpdateGui(CallbackInfo info)
        {
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                Players.Content = info.numPlayers;
                lbNames.Items.Clear();
                foreach (string s in info.playerNames) {
                    lbNames.Items.Add(s);
                }
            }
            else
            {
                // Only the main (dispatcher) thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
            }
        }
    }
}
