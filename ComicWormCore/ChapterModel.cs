using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ComicWorm
{
    public class ChapterModel : Chapter, INotifyPropertyChanged, IEquatable<ChapterModel>
    {
        public ChapterModel() : base()
        {
            this.pages = new List<PageModel>();
        }

        public ChapterModel(Chapter _chapter)
        {
            this.Number = _chapter.Number;
            this.Name = _chapter.Name;
            this.Url = _chapter.Url;
            this.pages = new List<PageModel>();
        }

        public override int Number
        {
            get { return this.number; }
            set { this.number = value; RaisePropertyChanged(nameof(Number)); }
        }

        public override string Name
        {
            get { return this.name; }
            set
            {
                this.name = value;
                this.name = Utls.RemoveIllegalChar(this.name);
                RaisePropertyChanged(nameof(Name));
            }
        }

        public override string Url
        {
            get { return this.url; }
            set { this.url = value; RaisePropertyChanged(nameof(Url)); }
        }

        private List<PageModel> pages;
        public List<PageModel> Pages
        {
            get { return this.pages; }
            set { this.pages = value; }
        }

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
