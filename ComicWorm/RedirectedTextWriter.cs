﻿using System;
using System.IO;
using System.Text;

namespace ComicWorm
{
    internal class RedirectedTextWriter : TextWriter
    {
        private MainWindowViewModel view;
        private string buffer = "";
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
