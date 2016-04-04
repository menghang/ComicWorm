using System.Collections.ObjectModel;
using System.ComponentModel;
using ComicModels;

namespace ComicWorm
{
    public class MainWindowViewModel : INotifyPropertyChanged
    {
        public ObservableCollection<ComicModel> Comics { get; set; }

        public ObservableCollection<ChapterModel> Chapters { get; set; } = new ObservableCollection<ChapterModel>();

        public ObservableCollection<PageModel> Pages { get; set; } = new ObservableCollection<PageModel>();

        private static readonly int MaxLogBuffer = 65535;
        private string log;
        public string Log
        {
            get
            {
                return this.log;
            }
            set
            {
                this.log = value;
                if (this.log.Length > MaxLogBuffer)
                {
                    this.log = this.log.Substring(this.Log.Length - MaxLogBuffer);
                }
                RaisePropertyChanged(nameof(Log));
            }
        }

        private bool busy;
        public bool Busy { set { this.busy = value; RaisePropertyChanged(nameof(EnableUI)); } }
        public bool EnableUI { get { return !(this.busy); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void RaisePropertyChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
