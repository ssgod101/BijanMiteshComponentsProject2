using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Crazy8Library
{
    [DataContract]
    public class Player
    {
        internal ICallBack PlayerCallBack { get; set; }
        [DataMember]public string Name { get; set; }
        internal bool Turn { get; set; }
        [DataMember]public int CardsInHand { get; set; }
        internal bool IsHost { get; set; }
        internal Player(string n, ICallBack callBack)
        {
            Name = n;
            PlayerCallBack = callBack;
            CardsInHand = 0;
        }
    }
}
