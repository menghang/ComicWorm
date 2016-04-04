using System;
using System.ComponentModel;

namespace ComicWorm
{
    public class PageModel :Page, INotifyPropertyChanged, IEquatable<PageModel>
    {
        public PageModel() : base()
        {            
        }

        public PageModel(Page _page)
        {
            this.Number = _page.Number;
            this.Url = _page.Url;
        }

        public override int Number
        {
            get { return this.number; }
            set { this.number = value; RaisePropertyChanged(nameof(Number)); }
        }
        
        public override string Url
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
