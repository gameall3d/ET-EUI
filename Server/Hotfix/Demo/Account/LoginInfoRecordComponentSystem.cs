using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ET
{
    public class LoginInfoRecordComponentDestroySystem: DestroySystem<LoginInfoRecordComponent>
    {
        public override void Destroy(LoginInfoRecordComponent self)
        {
            self.AcountLoginInfoDict.Clear();
        }
    }

    public static class LoginInfoRecordComponentSystem
    {
        public static void Add(this LoginInfoRecordComponent self, long key, int value)
        {
            if (self.AcountLoginInfoDict.ContainsKey(key))
            {
                self.AcountLoginInfoDict[key] = value;
                return;
            }
            
            self.AcountLoginInfoDict.Add(key, value);
        }

        public static void Remove(this LoginInfoRecordComponent self, long key)
        {
            if (self.AcountLoginInfoDict.ContainsKey(key))
            {
                self.AcountLoginInfoDict.Remove(key);
            }
        }

        public static int Get(this LoginInfoRecordComponent self, long key)
        {
            if (!self.AcountLoginInfoDict.TryGetValue(key, out int value))
            {
                return -1;
            }

            return value;
        }

        public static bool IsExist(this LoginInfoRecordComponent self, long key)
        {
            return self.AcountLoginInfoDict.ContainsKey(key);
        }
    }
}