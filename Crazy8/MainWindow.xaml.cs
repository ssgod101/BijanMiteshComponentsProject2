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
using Crazy8Library;

namespace Crazy8
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>

    [CallbackBehavior(ConcurrencyMode = ConcurrencyMode.Reentrant, UseSynchronizationContext = false)]
    public partial class MainWindow : Window ,ICallBack
    {
        private IDeck deck = null;
        private Lobby lobby = null;
        private List<Card> PlayerHand;
        private string PlayerName="";
        public MainWindow()
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
             lobby = new Lobby(ref deck);
            lobby.ShowDialog();
            PlayerName = lobby.name;
            NewGame();

        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {

            Environment.Exit(0);
        }
        private void NewGame() {
            deck.NewGame(PlayerName);
            PlayerHand = new List<Card>();
            for (int i = 0; i < 5; i++)
            {
                PlayerHand.Add(deck.DrawSingle(PlayerName));
            }
            MakeBtnCardOnScreen(PlayerHand);
            PlayerBotton.Content = PlayerName;
            
        }
        private void UpdateOtherPlayersCard(List<Player> AllPlayers) {
            AllPlayers.Remove(AllPlayers.Find(a => a.Name==PlayerName));
            if (AllPlayers.Count == 1) { PlayerTop.Content = AllPlayers.ElementAt(0).Name + ": " + AllPlayers.ElementAt(0).CardsInHand; }
            else if (AllPlayers.Count == 2)
            {
                PlayerTop.Content = AllPlayers.ElementAt(0).Name + ": " + AllPlayers.ElementAt(0).CardsInHand;
                PlayerRight.Content = AllPlayers.ElementAt(1).Name + ": " + AllPlayers.ElementAt(1).CardsInHand;
            }
            else
            {
                PlayerTop.Content = AllPlayers.ElementAt(0).Name + ": " + AllPlayers.ElementAt(0).CardsInHand;
                PlayerRight.Content = AllPlayers.ElementAt(1).Name + ": " + AllPlayers.ElementAt(1).CardsInHand;
                PlayerLeft.Content = AllPlayers.ElementAt(2).Name + ": " + AllPlayers.ElementAt(2).CardsInHand;
            }
            

              
        }
        private delegate void ClientUpdateDelegate(CallbackInfo info);
        public void UpdateGui(CallbackInfo info)
        {
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                if (info.StartGame) { lobby.Hide();}
                if (PlayerName != "") { UpdateOtherPlayersCard(info.AllPlayers); }
                lobby.UpdateLobby(info.numPlayers,info.AllPlayers,info.Administrator);
            }
            else
            {
                // Only the main (dispatcher) thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
            }
        }

        private void BtnCurrentCard_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnEndTurn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnDeck_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MakeBtnCardOnScreen(List<Card> CardsOnHand) {
            
            foreach (Card card in CardsOnHand)
            {
                Button button = new Button
                {
                    Width = 100

                };
                Image image = new Image
                {
                    Source = new BitmapImage
                    {
                        UriSource = new Uri("./Cards/backCard.png", UriKind.Relative)
                    }

                };
                button.Content = image;
                StackPanel.Children.Add(image);
            } 
        }
    }
}
