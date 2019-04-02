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
        [DataMember] public int topSuit { get; private set; }
        [DataMember] public int topRank { get; private set; }
        
        public CallbackInfo(int topS, int topR)
        {
            topSuit = topS;
            topRank = topR;
        }
    }
}
