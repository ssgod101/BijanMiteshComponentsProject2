﻿using System;
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
        //[OperationContract] Card[] Draw(string name, int amount);
        [OperationContract] Card DrawSingle(string name);
        [OperationContract] bool PlaceDown(string name,Card placed);
        [OperationContract] bool NewGame(string name);
        [OperationContract] bool EndTurn(string name);
        //[OperationContract] bool LockServer(string name);
        //[OperationContract] bool UnlockServer(string name);
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
        private Card TopCard;
        private string currentAdmin;
        private int TwoChain;

        // Member variables related to the callbacks
        private Dictionary<string, Player> userCallBacks;
        private List<string> turnManager;
        private int turnIndex;
        private string currentTurn;

        //Lobby Functionality

        public bool Join(string name)
        {
            if (userCallBacks.Count >= 4 || userCallBacks.ContainsKey(name) || name=="" || !canJoin)
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
                updateAllClients("", false,false);
                return true;
            }
        }
        public void Leave(string name)
        {
            if (userCallBacks.ContainsKey(name))
            {
                string nextPlayer = name;
                bool enoughPlayers = false;
                Console.WriteLine(name + " has left!");
                if (turnManager[turnIndex] == name && !canJoin)
                {
                    NextTurn();
                    nextPlayer = turnManager[turnIndex];

                }
                userCallBacks.Remove(name);
                turnManager.Remove(name);
                if(turnManager.Count >= 2 && !canJoin)
                {
                    turnIndex = turnManager.FindIndex(n => n == nextPlayer);
                }
                else if(!canJoin)
                {
                    enoughPlayers = true;
                }

                //if()
                //0 1 2 3
                if(userCallBacks.Count > 0) { currentAdmin = userCallBacks.First().Value.Name; }
                updateAllClients("", false,enoughPlayers);
            }
        }

        //Admin functions
        private bool LockServer(string name)
        {
            if (currentAdmin != name) { return false; }
            canJoin = false;
            return true;
        }

        private bool UnlockServer()
        {
            canJoin = true;
            return true;
        }

        public bool NewGame(string name)
        {
            if (currentAdmin != name) { return false; }
            LockServer(name);
            Shuffle();
            turnIndex = 0;
            TopCard = cards[cardIdx];
            cardIdx++;
            updateAllClients("", true,false);
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
            turnManager = new List<string>();
            turnIndex = 0;
            TwoChain = 0;
            TopCard = null;
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
            updateAllClients("",false,false);
            return card;
        }

        public bool EndTurn(string name)
        {
            if (turnManager[turnIndex] != name) { return false; }
            NextTurn();
            updateAllClients("",false,false);
            return true;
        }

        private int checkNextTurn()
        {
            int testNum = turnIndex + 1;
            if (testNum >= turnManager.Count) { testNum = 0; }
            return testNum;
        }

        private void NextTurn()
        {
            int testNum = turnIndex + 1;
            if (testNum >= turnManager.Count) { testNum = 0; }
            turnIndex = testNum;
            currentTurn = turnManager[turnIndex];
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

        public bool PlaceDown(string name, Card placed)
        {
            if (placed.Suit != TopCard.Suit && placed.Rank != TopCard.Rank || turnManager[turnIndex] != name) { return false; }
            //if ((suit != currentSuit && rank != currentRank) || cardIdx > cards.Count - 1) { return false; }
            TopCard = placed;
            userCallBacks[name].CardsInHand--;
            if (CheckWinner(name))
            {
                UnlockServer();
                TwoChain = 0;
                updateAllClients(name, false,false);
            }
            else
            {
                if (placed.Rank == Card.RankID.Jack) {
                    NextTurn();
                }
                if(placed.Rank == Card.RankID.Two){TwoChain+=2;}
                else{TwoChain = 0;}
                
            }
            EndTurn(name);
            return true;
        }

        bool CheckWinner(string name)
        {
            return userCallBacks[name].CardsInHand == 0;
        }


        //CallBack functions

        private void updateAllClients(string winner, bool start,bool notEnough)
        {
            CallbackInfo info = new CallbackInfo(userCallBacks.Count, userCallBacks.Values.ToList(), turnManager[turnIndex], currentAdmin, winner, TopCard,TwoChain, start);
            foreach (Player player in userCallBacks.Values)
            {
                player.PlayerCallBack.UpdateGui(info);
            }
        }
    }
}
