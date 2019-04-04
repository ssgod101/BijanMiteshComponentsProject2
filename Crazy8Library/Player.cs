using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crazy8Library
{
    public class Player
    {
        public ICallBack PlayerCallBack { get; set; }
        public string Name { get; set; }
        public bool Turn { get; set; }
        public int CardsInHand { get; set; }
        public bool IsHost { get; set; }
        public Player(string n, ICallBack callBack)
        {
            Name = n;
            PlayerCallBack = callBack;
            CardsInHand = 0;
        }
    }
}
