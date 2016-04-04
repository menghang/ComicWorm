namespace ComicWorm
{
    public class Comic
    {
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
