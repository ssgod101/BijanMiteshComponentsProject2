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
        //Lobby Info
        [DataMember] public int topSuit { get; private set; }
        [DataMember] public int topRank { get; private set; }
        //Game Info
        [DataMember] public int numPlayers { get; private set; }


        public CallbackInfo(int topS, int topR, int numPlayers)
        {
            topSuit = topS;
            topRank = topR;
        }

        public CallbackInfo(int num)
        {
            numPlayers = num;
        }
    }
}
