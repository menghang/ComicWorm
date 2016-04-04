using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicWorm
{
    public class RedirectedTextWriter : TextWriter
    {
        private  MainWindowViewModel view;
        private string buffer="";
        public RedirectedTextWriter(MainWindowViewModel _view)
        {
            this.view = _view;
        }

        public override void Write(char value)
        {
            this.buffer += value;
            if (buffer.EndsWith(Environment.NewLine))
            {
                this.view.Log += buffer;
                buffer = "";
            }
        }

        public override void Write(string value)
        {
            this.view.Log += value;
        }

        public override Encoding Encoding
        {
            get { return Encoding.Default; }
        }
    }
}
