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
            if (!Join.IsEnabled) { deck.Leave(name); }
            Environment.Exit(0);

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!Join.IsEnabled) { deck.Leave(name); }
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
        public void UpdateLobby(int numPlayers,List<Player> playerNames,string admin) {
           
            if (name != admin) {Play.Content = "Wait for Admin"; }
            else { Play.Content = "PLAY"; }
            Players.Content = numPlayers;
            lbNames.Items.Clear(); 
            foreach (Player player in playerNames)
            {
                if (player.Name == admin) { lbNames.Items.Add(admin + " (Admin)"); }
                else { lbNames.Items.Add(player.Name); }
            }
        }

        private void Help_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Crazy 8's is a card game that has many variations and has different rules based on where it would be first introduced.\n" +
                "This game is based on the rules that we learned when we were first introduced to this game." +
                "\n\n" +
                "At the beginning of the game, each player receives 5 cards in their hand, and after that a card is placed from the deck to the pile.\n" +
                "Each player takes turns adding a card from their hand to the pile (face up) if that card shares the same suit or rank as the card on the top of the pile.\n" +
                "If a player does not have a card that is able to be added to the pile, then they must draw 1 card. If the player wishes, they could add the card directly to the pile if it can be placed, or end thier turn without placing a card on the pile.\n\n" +
                "Extra rules:\n" +
                "If a player places a Two of any suit, the next player must draw 2 cards before starting their turn. If a Two is placed on the pile immediately after a Two has been placed, the next player must draw the same amount of cards that the previous player drew plus two more cards. This chain ends when anyone plays a card that is not a Two.\n\n" +
                "If a player places a Jack of any suit, the next player's turn is automatically skipped.\n\n" +
                "If a player has an Eight in their hand and have not placed a card on the pile yet, they have the option of placing down the card on the pile even if the card does not match the suit and rank. If a player does so, they are given the option to change the suit to whatever they wish.\n\n" +
                "A winner is declared when they do not have any remaining cards in their hand.\n\n" +
                "This game supports 2-4 players.", "Crazy 8 Manual", MessageBoxButton.OK,MessageBoxImage.Information);
        }
    }
}
