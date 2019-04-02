using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Crazy8Library
{
    [DataContract]
    public class Card
    {
        // Enumerations

        public enum SuitID { Clubs, Diamonds, Hearts, Spades };
        public enum RankID { Ace, King, Queen, Jack, Ten, Nine, Eight, Seven, Six, Five, Four, Three, Two };

        // Public properties and methods - only the member properties need to be decorated with
        // [DataMember] and these must have both a get and a set, otherwise a runtime error will occur.
        // Here we've made the sets private so that won't be available to the client.

        [DataMember] public SuitID Suit { get; private set; }
        [DataMember] public RankID Rank { get; private set; }

        public override string ToString()
        {
            return Rank.ToString() + " of " + Suit.ToString();
        }

        // Constructor

        public Card(SuitID s, RankID r)
        {
            Suit = s;
            Rank = r;
        }
    }
}
