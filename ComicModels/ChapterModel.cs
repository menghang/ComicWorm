using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ComicWorm
{
    public class ChapterModel : INotifyPropertyChanged, IEquatable<ChapterModel>
    {
        private int number;
        public int Number
        {
            get { return this.number; }
            set { this.number = value; RaisePropertyChanged(nameof(Number)); }
        }

        private string name;
        public string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.name = Utls.RemoveIllegalChar(this.name);
                RaisePropertyChanged(nameof(Name));
            }
        }

        private string url;
        public string Url
        {
            get { return this.url; }
            set { this.url = value; RaisePropertyChanged(nameof(Url)); }
        }

        public List<PageModel> Pages { get; set; } = new List<PageModel>();

        private bool downloaded;
        public bool Downloaded
        {
            get { return this.downloaded; }
            set { this.downloaded = value; RaisePropertyChanged(nameof(Downloaded)); }
        }

        public string Hash { get { return Utls.GetMD5(this.Url); } }

        private bool selected;
        public bool Selected
        {
            get { return this.selected; }
            set { this.selected = value; RaisePropertyChanged(nameof(Selected)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        public bool Equals(ChapterModel other)
        {
            return other.Hash.Equals(this.Hash);
        }
    }
}
