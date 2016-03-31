using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ComicWormCore
{
    public class ComicModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime UpdateTime { get; set; }

        public string MD5 { get { return GetMD5(this.Url); } }

        public List<ChapterModel> Chapters { get; set; } = new List<ChapterModel>();

        private string GetMD5(string source)
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
