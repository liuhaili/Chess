using Lemon;
using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using LitJson;
using Chess.Entity;
using Chess.Message;

public class JsonSerialize : ISerializeObject
{
    public object Deserialize(byte[] data, Type type)
    {
        throw new NotImplementedException();
    }

    public object DeserializeFromString(string data, Type type)
    {
        if (type == typeof(EAccount))
            return JsonMapper.ToObject<EAccount>(data);
        else if (type == typeof(EBattleRecord))
            return JsonMapper.ToObject<EBattleRecord>(data);
        else if (type == typeof(EBuyRecord))
            return JsonMapper.ToObject<EBuyRecord>(data);
        else if (type == typeof(EFriends))
            return JsonMapper.ToObject<EFriends>(data);
        else if (type == typeof(ELoginRecord))
            return JsonMapper.ToObject<ELoginRecord>(data);
        else if (type == typeof(EStore))
            return JsonMapper.ToObject<EStore>(data);
        else if (type == typeof(EBattleServers))
            return JsonMapper.ToObject<EBattleServers>(data);
        else if (type == typeof(List<EAccount>))
            return JsonMapper.ToObject<List<EAccount>>(data);
        else if (type == typeof(List<EBattleRecord>))
            return JsonMapper.ToObject<List<EBattleRecord>>(data);
        else if (type == typeof(List<EBuyRecord>))
            return JsonMapper.ToObject<List<EBuyRecord>>(data);
        else if (type == typeof(List<EFriends>))
            return JsonMapper.ToObject<List<EFriends>>(data);
        else if (type == typeof(List<ELoginRecord>))
            return JsonMapper.ToObject<List<ELoginRecord>>(data);
        else if (type == typeof(List<EStore>))
            return JsonMapper.ToObject<List<EStore>>(data);
        else if (type == typeof(List<EBattleServers>))
            return JsonMapper.ToObject<List<EBattleServers>>(data);
        else if (type == typeof(string))
            return JsonMapper.ToObject<string>(data);
        else if (type == typeof(List<string>))
            return JsonMapper.ToObject<List<string>>(data);
        else if (type == typeof(Battle))
            return JsonMapper.ToObject<Battle>(data);
        else if (type == typeof(PokerBattle))
            return JsonMapper.ToObject<PokerBattle>(data);
        return null;
    }

    public byte[] Serialize(object obj)
    {
        throw new NotImplementedException();
    }

    public string SerializeToString(object obj)
    {
        return JsonMapper.ToJson(obj);
    }
}
