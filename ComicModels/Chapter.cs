namespace ComicWorm
{
    public class Chapter
    {
        protected int number;
        public virtual int Number
        {
            get { return this.number; }
            set { this.number = value; }
        }

        protected string name;
        public virtual string Name
        {
            get { return this.name; }
            set { this.name = value; }
        }

        protected string url;
        public virtual string Url
        {
            get { return this.url; }
            set { this.url = value; }
        }
    }
}
