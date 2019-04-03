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
            NewGame();

        }

        private void NewGame() {
            PlayerHand = new List<Card>();

            for (int i = 0; i < 5; i++)
            {
                PlayerHand.Add(deck.Draw());
                
            }
            foreach (var item in PlayerHand)
            {
                lbPlayerHand.Items.Add(item.ToString());
            }
            
        }
        private delegate void ClientUpdateDelegate(CallbackInfo info);
        public void UpdateGui(CallbackInfo info)
        {
            if (System.Threading.Thread.CurrentThread == this.Dispatcher.Thread)
            {
                lobby.UpdateLobby(info.numPlayers,info.playerNames);
            }
            else
            {
                // Only the main (dispatcher) thread can change the GUI
                this.Dispatcher.BeginInvoke(new ClientUpdateDelegate(UpdateGui), info);
            }
        }
    }
}
