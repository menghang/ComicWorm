using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicModels
{
    public class PageModel : INotifyPropertyChanged, IEquatable<PageModel>
    {
        public int number;
        public int Number
        {
            get { return this.number; }
            set { this.number = value; RaisePropertyChanged(nameof(Number)); }
        }

        private string url;
        public string Url
        {
            get { return this.url; }
            set { this.url = value; RaisePropertyChanged(nameof(Url)); }
        }

        private bool downloaded;
        public bool Downloaded
        {
            get { return this.downloaded; }
            set { this.downloaded = value; RaisePropertyChanged(nameof(Downloaded)); }
        }

        private bool selected;
        public bool Selected
        {
            get { return this.selected; }
            set { this.selected = value; RaisePropertyChanged(nameof(Selected)); }
        }

        public string Hash
        {
            get { return Utls.GetMD5(this.Url); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Equals(PageModel other)
        {
            return other.Hash.Equals(this.Hash);
        }
    }
}
