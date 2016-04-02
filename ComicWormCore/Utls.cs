using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ComicWormCore
{
    public static class Utls
    {
        public static string GetMD5(string source)
        {
            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            byte[] byteValue = System.Text.Encoding.UTF8.GetBytes(source);
            byte[] byteHash = md5.ComputeHash(byteValue);
            md5.Clear();
            string sTemp = "";
            for (int i = 0; i < byteHash.Length; i++)
            {
                sTemp += byteHash[i].ToString("X").PadLeft(2, '0');
            }
            return sTemp.ToLower();
        }
    }
}
