using Lemon;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChessServer
{
    public class JsonSerialize : ISerializeObject
    {
        public object Deserialize(byte[] data, Type type)
        {
            throw new NotImplementedException();
        }

        public object DeserializeFromString(string data, Type type)
        {
            return JsonConvert.DeserializeObject(data, type);
        }

        public byte[] Serialize(object obj)
        {
            throw new NotImplementedException();
        }

        public string SerializeToString(object obj)
        {
            return JsonConvert.SerializeObject(obj);
        }
    }
}
