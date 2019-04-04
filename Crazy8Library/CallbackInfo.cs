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
        [DataMember] public int TopSuit { get; private set; }
        [DataMember] public int TopRank { get; private set; }
        [DataMember] public bool StartGame { get; private set; }
        [DataMember] public string Administrator { get; private set; }
        [DataMember] public string Winner { get; set; }
        [DataMember] public string CurrentTurn { get; set; }

        //Lobby Info
        [DataMember] public int numPlayers { get; private set; }
        [DataMember] public string[] playerNames { get; private set; }

        public CallbackInfo(int num, string[] names,string turn,string admin, string win,int topS, int topR, bool start)
        {
            Administrator = admin;
            Winner = win;
            TopSuit = topS;
            TopRank = topR;
            StartGame = start;
            numPlayers = num;
            playerNames = names;
            CurrentTurn = turn;
        }
    }
}
