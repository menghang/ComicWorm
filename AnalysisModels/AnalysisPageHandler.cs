using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ComicModels;
using HtmlAgilityPack;

namespace AnalysisModels
{
    public delegate void AnalysisPageHandler(HtmlDocument htmlDoc, ChapterModel chapter);
}
