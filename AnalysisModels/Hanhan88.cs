using System;
using System.Collections.Generic;
using ComicWorm;
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

        private List<Chapter> AnalysisChapter(HtmlDocument htmlDoc, Comic comic)
        {
            return null;
        }

        private List<Page> AnalysisPage(HtmlDocument htmlDoc, Chapter chapter)
        {
            return null;
        }
    }
}
