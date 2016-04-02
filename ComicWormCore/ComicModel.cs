using System;
using System.Collections.Generic;
using System.Security.Cryptography;

namespace ComicWormCore
{
    public class ComicModel
    {
        public string Name { get; set; }

        public string Url { get; set; }

        public string MD5 { get { return Utls.GetMD5(this.Url); } }

        public List<ChapterModel> Chapters { get; set; } = new List<ChapterModel>();        
    }
}
