using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization; // WCF DataContract types

namespace Crazy8Library
{
    [DataContract]
    public class CallbackInfo
    {
        //Game Info
        [DataMember] public Card CurrentCard { get; private set; }
        [DataMember] public bool StartGame { get; private set; }
        [DataMember] public string Administrator { get; private set; }
        [DataMember] public string Winner { get; set; }
        [DataMember] public string CurrentTurn { get; set; }
        [DataMember] public int PickUpCards { get; private set; }
        [DataMember] public bool NotEnoughPlayers { get; set; }

        //Lobby Info
        [DataMember] public int numPlayers { get; private set; }
        [DataMember] public List<Player> AllPlayers { get; private set; }

        public CallbackInfo(int num, List<Player> players, string turn, string admin, string win, Card top, int pick, bool start, bool invalid)
        {
            Administrator = admin;
            Winner = win;
            CurrentCard = top;
            StartGame = start;
            numPlayers = num;
            AllPlayers = players;
            CurrentTurn = turn;
            PickUpCards = pick;
            NotEnoughPlayers = invalid;
        }
    }
}
