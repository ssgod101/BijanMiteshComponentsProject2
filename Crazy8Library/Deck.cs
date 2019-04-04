using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;  // WCF types

//Rules about Crazy 8's
//
//Create a lobby that keeps track of players before starting a new game.
//
//2-4 Players
//2 makes next player take 2 cards from deck * currentChain;
//Jack skips next player's turn.
//8 allows player to choose new suit.

namespace Crazy8Library
{
    public interface ICallBack
    {
        [OperationContract(IsOneWay = true)] void UpdateGui(CallbackInfo info);
    }

    // Define a WCF service contract for the Shoe "service"
    [ServiceContract(CallbackContract = typeof(ICallBack))]
    public interface IDeck
    {
        [OperationContract] bool Join(string name);
        [OperationContract(IsOneWay = true)] void Leave(string name);
        [OperationContract] Card[] Draw(string name, int amount);
        [OperationContract] Card DrawSingle(string name);
        [OperationContract] bool PlaceDown(string name,int suit, int rank);
        [OperationContract] bool NewGame(string name);
    }
    //------------------------------------------------------------------
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Deck : IDeck
    {
        private List<Card> cards;   // a container to hold the cards
        private int cardIdx;        // the index of the next card to draw
        private static uint objCount = 0;
        private uint objNum;
        private bool canJoin = true;

        private int currentSuit;
        private int currentRank;
        private string currentAdmin;
        private int currentTurn;

        // Member variables related to the callbacks
        private Dictionary<string, Player> userCallBacks;
        private List<string> turnManager;

        //Lobby Functionality

        public bool Join(string name)
        {
            if (userCallBacks.Count >= 4 || userCallBacks.ContainsKey(name) || !canJoin)
            {
                return false;
            }
            else
            {
                ICallBack cb = OperationContext.Current.GetCallbackChannel<ICallBack>();
                Player newPlayer = new Player(name, cb);
                Console.WriteLine(name + " has joined!");
                userCallBacks.Add(name, newPlayer);
                turnManager.Add(name);
                userCallBacks[name].IsHost = userCallBacks.Count == 1;
                if (userCallBacks[name].IsHost) { currentAdmin = name; }
                updateAllClients("", false);
                return true;
            }
        }
        public void Leave(string name)
        {
            if (userCallBacks.ContainsKey(name))
            {
                Console.WriteLine(name + " has left!");
                if (turnManager[currentTurn] == name) { }
                userCallBacks.Remove(name);
                turnManager.Remove(name);
                //if()
                //0 1 2 3
                if(userCallBacks.Count > 0) { currentAdmin = userCallBacks.First().Value.Name; }
                updateAllClients("", false);
            }
        }

        //Admin functions
        public bool LockServer()
        {
            canJoin = false;
            return true;
        }

        public bool UnlockServer()
        {
            canJoin = true;
            return true;
        }

        public bool NewGame(string name)
        {
            if (currentAdmin != name) { return false; }
            Shuffle();
            currentTurn = 0;
            currentSuit = (int)cards[cardIdx].Suit;
            currentRank = (int)cards[cardIdx].Rank;
            cardIdx++;
            updateAllClients("", true);
            return true;
        }

        //Game functions

        public Deck()
        {
            objNum = ++objCount;
            Console.WriteLine("Creating Crazy8 Server Object #" + objNum);

            // Initialize member variables
            cards = new List<Card>();
            userCallBacks = new Dictionary<string, Player>();
            currentSuit = 0;
            currentRank = 0;
            Repopulate();
        }

        public Card[] Draw(string name, int amount)
        {
            Card[] cardsToGive = new Card[amount];
            for (int i = 0; i < amount; i++)
            {
                if (cardIdx >= cards.Count) { Shuffle(); }
                Card card = cards[cardIdx++];
                userCallBacks[name].CardsInHand++;
                Console.WriteLine("[Game #" + objNum + "] Dealing " + card);
                cardsToGive[i] = card;
            }
            return cardsToGive;
        }

        public Card DrawSingle(string name)
        {
            if (cardIdx >= cards.Count) { Shuffle(); }
            Card card = cards[cardIdx++];
            userCallBacks[name].CardsInHand++;
            Console.WriteLine("[Game #" + objNum + "] Dealing " + card);
            return card;
        }

        public bool EndTurn()
        {
            currentTurn++;
            if (currentTurn >= turnManager.Count) { currentTurn = 0; }
            //updateAllClients();
            return false;
        }

        void Repopulate()
        {
            cards.Clear();

            // For each suit
            foreach (Card.SuitID s in Enum.GetValues(typeof(Card.SuitID)))
            {
                // For each rank
                foreach (Card.RankID r in Enum.GetValues(typeof(Card.RankID)))
                {
                    cards.Add(new Card(s, r));
                }
            }

            // Randomize the collection
            Shuffle();
        }
        public void Shuffle()
        {
            Random rng = new Random();
            cards = cards.OrderBy(number => rng.Next()).ToList();
            cardIdx = 0;
        }

        public bool PlaceDown(string name, int suit, int rank)
        {
            if ((suit != currentSuit && rank != currentRank) || cardIdx > cards.Count - 1) { return false; }
            currentSuit = suit;
            currentRank = rank;
            userCallBacks[name].CardsInHand--;
            if (CheckWinner(name))
            {
                updateAllClients(name, false);
            }
            else
            {
                updateAllClients("", false);
            }
            return true;
        }

        bool CheckWinner(string name)
        {
            return userCallBacks[name].CardsInHand == 0;
        }


        //CallBack functions

        private void updateAllClients(string winner, bool start)
        {
            CallbackInfo info = new CallbackInfo(userCallBacks.Count, userCallBacks.Keys.ToArray(), turnManager[currentTurn], currentAdmin, winner, currentSuit, currentRank, start);
            foreach (Player player in userCallBacks.Values)
            {
                player.PlayerCallBack.UpdateGui(info);
            }
        }
    }
}
