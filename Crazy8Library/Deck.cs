using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;  // WCF types

namespace Crazy8Library
{
    public interface ICallBack
    {
        [OperationContract(IsOneWay = true)] void UpdateGui(CallbackInfo info);
    }
    public interface IDeck
    {

    }
    public class Deck
    {
        private List<Card> cards;   // a container to hold the cards



        public Deck() {
            Repopulate();
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
    }
}
