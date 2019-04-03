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
        [OperationContract] Card Draw();
    }
    //------------------------------------------------------------------
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Deck : IDeck
    {
        private List<Card> cards;   // a container to hold the cards
        private int cardIdx;        // the index of the next card to draw
        private static uint objCount = 0;
        private uint objNum;

        // Member variables related to the callbacks
        private Dictionary<string, ICallBack> userCallBacks;

        public bool Join(string name)
        { 
            if(userCallBacks.Count >= 4 || userCallBacks.ContainsKey(name.ToUpper()))
            {
                return false;
            }
            else
            {
                ICallBack cb = OperationContext.Current.GetCallbackChannel<ICallBack>();
                Console.WriteLine(name + " has joined!");
                userCallBacks.Add(name.ToUpper(),cb);
                updateLobby();
                return true;
            }
        }
        public void Leave(string name)
        {
            if (userCallBacks.ContainsKey(name.ToUpper()))
            {
                Console.WriteLine(name + " has left!");
                userCallBacks.Remove(name.ToUpper());
                updateLobby();
            }
        }
        public Deck()
        {
            objNum = ++objCount;
            Console.WriteLine("Creating Crazy8 Server Object #" + objNum);

            // Initialize member variables
            cards = new List<Card>();
            userCallBacks = new Dictionary<string, ICallBack>();
            Repopulate();
        }

        public Card Draw()
        {
            if (cardIdx >= cards.Count)
                // No cards remaining to be drawn
                throw new System.IndexOutOfRangeException("The Deck is empty.");

            Card card = cards[cardIdx++];

            updateAllClients(false);

            Console.WriteLine("[Game #" + objNum + "] Dealing " + card);

            return card;
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


        }

        private void updateLobby()
        {
            CallbackInfo info = new CallbackInfo(userCallBacks.Count,userCallBacks.Keys.ToArray());
            foreach(ICallBack cb in userCallBacks.Values)
            {
                cb.UpdateGui(info);
            }
        }

        private void updateAllClients(bool emptyHand)
        {

        }
    }
}
