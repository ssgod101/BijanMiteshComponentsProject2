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
        [DataMember] public int topSuit { get; private set; }
        [DataMember] public int topRank { get; private set; }
        [DataMember] public bool emptyHand { get; private set; }
        [DataMember] public bool drawHand { get; private set; }

        //Lobby Info
        [DataMember] public int numPlayers { get; private set; }
        [DataMember] public string[] playerNames { get; private set; }

        public CallbackInfo(int topS, int topR, bool empty, bool draw)
        {
            topSuit = topS;
            topRank = topR;
            emptyHand = empty;
            drawHand = draw;
        }

        public CallbackInfo(int num, string[] names)
        {
            numPlayers = num;
            playerNames = names;
        }
    }
}
