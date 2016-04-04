using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ComicWorm
{
    public class ComicModel : Comic, INotifyPropertyChanged, IEquatable<ComicModel>
    {
        public ComicModel() : base()
        {
            this.chapters = new List<ChapterModel>();
        }

        public ComicModel(Comic _comic)
        {
            this.Name = _comic.Name;
            this.Url = _comic.Url;
            this.chapters= new List<ChapterModel>();
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
            set
            {
                if (value.StartsWith("http://") || value.StartsWith("https://"))
                {
                    this.url = value;
                }
                else
                {
                    this.url = "http://" + value;
                }
                RaisePropertyChanged(nameof(Url));
            }
        }

        public string Hash { get { return Utls.GetMD5(this.Url); } }

        private List<ChapterModel> chapters;
        public List<ChapterModel> Chapters
        {
            get { return this.chapters; }
            set { this.chapters = value; }
        }

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

        public bool Equals(ComicModel other)
        {
            return other.Hash.Equals(this.Hash);
        }
    }
}
