using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class LoginInfoRecordComponent : Entity, IAwake, IDestroy
    {
        public Dictionary<long, int> AcountLoginInfoDict = new Dictionary<long, int>();
    }
}
