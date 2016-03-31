using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicWormCore
{
    public class ChapterModel
    {
        public int Number { get; set; }

        public string Name { get; set; }

        public string Url { get; set; }

        public DateTime UpdateTime { get; set; }

        public List<PageModel> Pages { get; set; } = new List<PageModel>();

        private bool download;
        public bool Downloaded
        {
            get
            {
                foreach(PageModel page in this.Pages)
                {
                    if (!page.Downloaded)
                    {
                        this.download = false;
                        return this.download;
                    }
                }
                this.download = false;
                return this.download;
            }
            set
            {
                this.download = value;
            }
        }

        public void AddPage(PageModel page)
        {
            page.Downloaded = this.download;
            this.Pages.Add(page);
        }
    }
}
