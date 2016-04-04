using System.Collections.Generic;
using HtmlAgilityPack;

namespace ComicWorm
{
    public delegate List<Page> AnalysisPageHandler(HtmlDocument htmlDoc, Chapter chapter);
}
