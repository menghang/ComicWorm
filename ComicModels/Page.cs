namespace ComicWorm
{
    public class Page
    {
        protected int number;
        public virtual int Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        protected string url;
        public virtual string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
    }
}
