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
   // [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class Lobby : Window//,ICallBack
    {
        private IDeck deck = null;
        private string name = "";
        public Lobby(ref IDeck d)
        {
            InitializeComponent();
            deck = d;

        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            name = tbName.Text;
            if (deck.Join(name))
            {
                Play.IsEnabled = true;
                Join.IsEnabled = false;
            }
            else { MessageBox.Show(name + " was not able to join game.","",MessageBoxButton.OK,MessageBoxImage.Error); }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            deck.Leave(name);
            Environment.Exit(0);

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
           
            Environment.Exit(0);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();

        }
        public void UpdateLobby(int numPlayers,string[] playerNames) {
            Players.Content = numPlayers;
            lbNames.Items.Clear();
            foreach (string s in playerNames)
            {
                lbNames.Items.Add(s);
            }
        }
        // Implement ICallback contract
       // private delegate void ClientUpdateDelegate(CallbackInfo info);

        //public void UpdateGui(CallbackInfo info)
        //{
        //    if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
        //    {
        //        Players.Content = info.numPlayers;
        //        lbNames.Items.Clear();
        //        foreach (string s in info.playerNames) {
        //            lbNames.Items.Add(s);
        //        }
        //    }
        //    else
        //    {
        //        // Only the main (dispatcher) thread can change the GUI
        //        this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
        //    }
        //}
    }
}
