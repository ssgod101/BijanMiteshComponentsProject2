using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace GameLibrary
{
    [DataContract]
    public class CallbackInfo
    {
        [DataMember] public int NumCards { get; private set; }
        [DataMember] public int NumDecks { get; private set; }
        [DataMember] public bool EmptyHand { get; private set; }

        public CallbackInfo(int c, int d, bool e)
        {
            NumCards = c;
            NumDecks = d;
            EmptyHand = e;
        }
    }
}
