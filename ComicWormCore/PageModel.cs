using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicWormCore
{
    public class PageModel
    {
        public int Number { get; set; }

        public string Url { get; set; }

        private Database database = new Database();
        public bool Downloaded
        {
            get
            {
                return database.IsDownloaded(this);
            }
        }

        public string MD5 { get { return Utls.GetMD5(this.Url); } }
    }
}
