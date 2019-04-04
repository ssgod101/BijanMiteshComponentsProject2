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

    public partial class Lobby : Window//,ICallBack
    {
        private IDeck deck = null;
        public string name = "";
       
        public Lobby(ref IDeck d)
        {
            InitializeComponent();
            deck = d;

        }

        private void Join_Click(object sender, RoutedEventArgs e)
        {
            name = tbName.Text.ToUpper();
            if (name!=""&& deck.Join(name))
            {
                Play.IsEnabled = true;
                Join.IsEnabled = false;
            }
            else { MessageBox.Show(name + " Player was not able to join game.","",MessageBoxButton.OK,MessageBoxImage.Error); }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            deck.Leave(name);
            Environment.Exit(0);

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            deck.Leave(name);
            Environment.Exit(0);
        }

        private void Play_Click(object sender, RoutedEventArgs e)
        {
            if (Play.Content.ToString() != "PLAY") { MessageBox.Show("Wait for Admin to start the game."); }
            else
            {
                if (int.Parse(Players.Content.ToString()) < 2) { MessageBox.Show("Wait for another player"); }
                else
                {
                    deck.NewGame(name);
                }
            }
        }
        public void UpdateLobby(int numPlayers,string[] playerNames,string admin) {
           
            if (name != admin) {Play.Content = "Wait for Admin"; }
            Players.Content = numPlayers;
            lbNames.Items.Clear(); 
            foreach (string s in playerNames)
            {
                if (s == admin) { lbNames.Items.Add(admin + " (Admin)"); }
                else { lbNames.Items.Add(s); }
                
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Help instructions.......", "Crazy 8 Manual", MessageBoxButton.OK);
        }
    }
}
