using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ComicWorm
{
    public class ComicModel : INotifyPropertyChanged, IEquatable<ComicModel>
    {
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

        public List<ChapterModel> Chapters { get; set; } = new List<ChapterModel>();

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
