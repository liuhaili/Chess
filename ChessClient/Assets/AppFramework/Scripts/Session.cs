using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{
    public class Session
    {
        public static int UserID = 0;

        public static string GetSessionKey()
        {
            string guid = Guid.NewGuid().ToString();
            StringBuilder sb = new StringBuilder();
            sb.Append("lemon20120530");
            sb.Append(UserID);
            sb.Append(guid);
            MD5 newMd5 = new MD5CryptoServiceProvider();
            byte[] sourceBit = Encoding.UTF8.GetBytes(sb.ToString());
            byte[] directBit = newMd5.ComputeHash(sourceBit);
            string newstr = BitConverter.ToString(directBit).Replace("-", "");
            return UserID + "_" + guid + "_" + newstr.ToLower();
        }
    }
}
