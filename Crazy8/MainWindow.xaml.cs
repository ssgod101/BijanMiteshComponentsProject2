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
        private string Administrator = "";
        private string CurrentTurn = "";
        private bool pickedOne = false;
        private bool gotNumberTwo = false;
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
            deck.Leave(PlayerName);
            Environment.Exit(0);
        }
        private void NewGame() {
            deck.NewGame(PlayerName);
            PlayerHand = new List<Card>();
            for (int i = 0; i < 5; i++)
            {
                PlayerHand.Add(deck.DrawSingle(PlayerName));
            }
            MakeBtnCardOnScreen();
            PlayerBotton.Content = PlayerName;
            
        }
      
        private delegate void ClientUpdateDelegate(CallbackInfo info);
        public void UpdateGui(CallbackInfo info)
        {
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                if (info.StartGame) { lobby.Hide();}
                if (PlayerName != "") { UpdateOtherPlayersCard(info.AllPlayers);
                    lCurrentTurn.Content = "Player turn: "+info.CurrentTurn;
                    Image image = new Image();
                    string temp = "./Cards/" + info.CurrentCard.ToString() + ".png";
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(temp, UriKind.RelativeOrAbsolute);
                    bitmapImage.EndInit();
                    image.Source = bitmapImage;
                    btnCurrentCard.Content = image;
                   
                }
                lobby.UpdateLobby(info.numPlayers,info.AllPlayers,info.Administrator);
                Administrator = info.Administrator;
                if(info.Winner.Length > 0)
                {
                    MessageBox.Show(info.Winner+" wins!");
                    this.Hide();
                    deck.Leave(PlayerName);
                    lobby.Join.IsEnabled = true;
                    lobby.ShowDialog();
                }
                CurrentTurn = info.CurrentTurn;
                
                if(CurrentTurn == PlayerName && info.PickUpCards > 0 && !gotNumberTwo)
                {
                    for(int i = 0; i < info.PickUpCards; i++)
                    {
                        PlayerHand.Add(deck.DrawSingle(PlayerName));
                    }
                    MakeBtnCardOnScreen();
                    gotNumberTwo = true;
                }
                
            }
            else
            {
                // Only the main (dispatcher) thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
            }
        }

        private void Card_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTurn == PlayerName)
            {
                Button button = sender as Button;
                string temp = button.Name;
                string[] splitResult = temp.Split('_');
                Enum.TryParse(splitResult[0], out Card.RankID rank);
                Enum.TryParse(splitResult[1], out Card.SuitID suit);
                if (rank == Card.RankID.Eight)
                {

                    PickSuit();
                    PlayerHand.Remove(PlayerHand.Find(c => c.Rank == rank && c.Suit == suit));
                    MakeBtnCardOnScreen();
                    
                }
                else
                {

                    if (!deck.PlaceDown(PlayerName, new Card(suit, rank)))
                    {
                        MessageBox.Show("suit or rank must be same as top card.");
                    }
                    else
                    {

                        PlayerHand.Remove(PlayerHand.Find(c => c.Rank == rank && c.Suit == suit));
                        MakeBtnCardOnScreen();
                        pickedOne = false;
                        gotNumberTwo = false;

                    }
                }
            }
        }

        private void BtnEndTurn_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTurn == PlayerName && pickedOne) {

                deck.EndTurn(PlayerName);
                pickedOne = false;
                gotNumberTwo = false;
            }
            
        }

        private void BtnDeck_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentTurn == PlayerName && !pickedOne) {


                PlayerHand.Add(deck.DrawSingle(PlayerName));
                MakeBtnCardOnScreen();
 
            }
            pickedOne = true;
        }

        private void MakeBtnCardOnScreen() {
            try
            {
                CanvasBottom.Children.Clear();
                double Left = 1112 / PlayerHand.Count;
                double MarginLeft = 0.0;
                foreach (Card card in PlayerHand)
                {
                    Button button = new Button();
                    Image image = new Image();
                    string temp = "./Cards/" + card.ToString() + ".png";
                    BitmapImage bitmapImage = new BitmapImage();
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(temp, UriKind.RelativeOrAbsolute);
                    bitmapImage.EndInit();
                    image.Source = bitmapImage;
                    button.Content = image;
                    button.Name = card.ToString();
                    button.Click += Card_Click;
                    button.MaxHeight = 152;
                    CanvasBottom.Children.Add(button);
                    button.Background = Brushes.White;
                    button.BorderThickness = new Thickness(0, 0, 0, 0);
                    button.Margin = new Thickness(MarginLeft, 0, 0, 0);
                    MarginLeft += Left;
                }
            }
            catch (Exception e)
            {
                string s = e.Message;
            }
        }
        private void UpdateOtherPlayersCard(List<Player> AllPlayers)
        {
            AllPlayers.Remove(AllPlayers.Find(a => a.Name == PlayerName));
           
            CanvasTop.Children.Clear();
            CanvasLeft.Children.Clear();
            CanvasRight.Children.Clear();
            
            try
            {
                if (AllPlayers.Count == 1)
                {
                    PlayerTop.Content = AllPlayers.ElementAt(0).Name + ": " + AllPlayers.ElementAt(0).CardsInHand;
                    double Left = 500 / AllPlayers.ElementAt(0).CardsInHand;
                    double MarginLeft = 0.0;
                    for (int i = 0; i < AllPlayers.ElementAt(0).CardsInHand; i++)
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("./Cards/backCard.png", UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        CanvasTop.Children.Add(image);
                        image.MaxHeight = 100;
                        image.Margin = new Thickness(MarginLeft, 0, 0, 0);
                        MarginLeft += Left;
                    }
                }
                else if (AllPlayers.Count == 2)
                {
                    PlayerTop.Content = AllPlayers.ElementAt(0).Name + ": " + AllPlayers.ElementAt(0).CardsInHand;
                    PlayerRight.Content = AllPlayers.ElementAt(1).Name + ": " + AllPlayers.ElementAt(1).CardsInHand;
                    double Left = 500 / AllPlayers.ElementAt(0).CardsInHand;
                    double MarginLeft = 0.0;
                    for (int i = 0; i < AllPlayers.ElementAt(0).CardsInHand; i++)
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("./Cards/backCard.png", UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        CanvasTop.Children.Add(image);
                        image.MaxHeight = 100;
                        image.Margin = new Thickness(MarginLeft, 0, 0, 0);
                        MarginLeft += Left;
                    }
                    double Left1 = 500 / AllPlayers.ElementAt(1).CardsInHand;
                    double MarginLeft1 = 0.0;
                    for (int i = 0; i < AllPlayers.ElementAt(1).CardsInHand; i++)
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("./Cards/backCard.png", UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        CanvasRight.Children.Add(image);
                        image.MaxHeight = 100;
                        image.Margin = new Thickness(MarginLeft1, 0, 0, 0);
                        MarginLeft1 += Left1;
                    }
                }
                else if(AllPlayers.Count==3)
                {
                    PlayerTop.Content = AllPlayers.ElementAt(0).Name + ": " + AllPlayers.ElementAt(0).CardsInHand;
                    PlayerRight.Content = AllPlayers.ElementAt(1).Name + ": " + AllPlayers.ElementAt(1).CardsInHand;
                    PlayerLeft.Content = AllPlayers.ElementAt(2).Name + ": " + AllPlayers.ElementAt(2).CardsInHand;
                    double Left = 500 / AllPlayers.ElementAt(0).CardsInHand;
                    double MarginLeft = 0.0;
                    for (int i = 0; i < AllPlayers.ElementAt(0).CardsInHand; i++)
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("./Cards/backCard.png", UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        CanvasTop.Children.Add(image);
                        image.MaxHeight = 100;
                        image.Margin = new Thickness(MarginLeft, 0, 0, 0);
                        MarginLeft += Left;
                    }
                    double Left1 = 500 / AllPlayers.ElementAt(1).CardsInHand;
                    double MarginLeft1 = 0.0;
                    for (int i = 0; i < AllPlayers.ElementAt(1).CardsInHand; i++)
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("./Cards/backCard.png", UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        CanvasRight.Children.Add(image);
                        image.MaxHeight = 100;
                        image.Margin = new Thickness(MarginLeft1, 0, 0, 0);
                        MarginLeft1 += Left1;
                    }
                    double Left2 = 500 / AllPlayers.ElementAt(2).CardsInHand;
                    double MarginLeft2 = 0.0;
                    for (int i = 0; i < AllPlayers.ElementAt(2).CardsInHand; i++)
                    {
                        Image image = new Image();
                        BitmapImage bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.UriSource = new Uri("./Cards/backCard.png", UriKind.RelativeOrAbsolute);
                        bitmapImage.EndInit();
                        image.Source = bitmapImage;
                        CanvasLeft.Children.Add(image);
                        image.MaxHeight = 100;
                        image.Margin = new Thickness(MarginLeft2, 0, 0, 0);
                        MarginLeft2 += Left2;
                    }
                }
            }
            catch (Exception e)
            {
                string s=e.Message;
            }



        }
        private void PickSuitClick(object sender, RoutedEventArgs e) {
            StackPanelSuit.Children.Clear();
            Button button = sender as Button;
            string temp = button.Name;
            string[] splitResult = temp.Split('_');
            Enum.TryParse(splitResult[0], out Card.RankID rank);
            Enum.TryParse(splitResult[1], out Card.SuitID suit);
            deck.PlaceDown(PlayerName,new Card(suit,rank));

            pickedOne = false;
            gotNumberTwo = false;

        }
        private void PickSuit()
        {
            {
                Button button = new Button();
                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("./Cards/Eight_Hearts.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                image.Source = bitmapImage;
                button.Content = image;
                button.Click += PickSuitClick;
                button.Background = Brushes.White;
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                button.Name = "Eight_Hearts";
                StackPanelSuit.Children.Add(button);
            }
            {
                Button button = new Button();
                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("./Cards/Eight_Diamonds.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                image.Source = bitmapImage;
                button.Click += PickSuitClick;
                button.Background = Brushes.White;
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                button.Content = image;
                button.Name = "Eight_Diamonds";
                StackPanelSuit.Children.Add(button);
            }
            {
                Button button = new Button();
                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("./Cards/Eight_Clubs.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                image.Source = bitmapImage;
                button.Content = image;
                button.Click += PickSuitClick;
                button.Background = Brushes.White;
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                button.Name = "Eight_Clubs";
                StackPanelSuit.Children.Add(button);
            }
            {
                Button button = new Button();
                Image image = new Image();
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.UriSource = new Uri("./Cards/Eight_Spades.png", UriKind.RelativeOrAbsolute);
                bitmapImage.EndInit();
                image.Source = bitmapImage;
                button.Content = image;
                button.Click += PickSuitClick;
                button.Background = Brushes.White;
                button.BorderThickness = new Thickness(0, 0, 0, 0);
                button.Name = "Eight_Spades";
                StackPanelSuit.Children.Add(button);
            }


            
        }
    }
}
