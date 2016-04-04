using System.Collections.Generic;
using HtmlAgilityPack;

namespace ComicWorm
{
    public delegate List<Chapter> AnalysisChapterHandler(HtmlDocument htmlDoc, Comic comic);
}
