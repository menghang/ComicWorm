using ComicModels;
using HtmlAgilityPack;

namespace AnalysisModels
{
    public delegate void AnalysisChapterHandler(HtmlDocument htmlDoc, ComicModel comic);
}
