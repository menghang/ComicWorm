using System;
using ComicModels;
using HtmlAgilityPack;

namespace AnalysisModels
{
    public class Hanhan88 : IAnalysisModel
    {
        public Tuple<string, AnalysisChapterHandler, AnalysisPageHandler> GetAnalysisModel()
        {
            return
                new Tuple<string, AnalysisChapterHandler, AnalysisPageHandler>(nameof(Hanhan88), AnalysisChapter, AnalysisPage);
        }

        public string GetWebset()
        {
            return "www.hanhan88.com";
        }

        private void AnalysisChapter(HtmlDocument htmlDoc, ComicModel comic)
        {

        }

        private void AnalysisPage(HtmlDocument htmlDoc, ChapterModel chapter)
        {

        }
    }
}
