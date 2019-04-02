using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ServiceModel;

namespace GameLibrary
{
    [ServiceContract]
    public interface IMe {

        string Name_ { [OperationContract] get; [OperationContract] set; }

    }
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
    public class Me:IMe
    {
        public string Name_ { get; set; }
    }
}
