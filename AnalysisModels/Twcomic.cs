using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using ComicWorm;
using HtmlAgilityPack;

namespace AnalysisModels
{
    public class Twcomic : IAnalysisModel
    {
        public Tuple<string, AnalysisChapterHandler, AnalysisPageHandler> GetAnalysisModel()
        {
            return
                new Tuple<string, AnalysisChapterHandler, AnalysisPageHandler>(nameof(Hanhan88), AnalysisChapter, AnalysisPage);
        }

        public string GetWebset()
        {
            return "www.twcomic.com";
        }

        private List<Chapter> AnalysisChapter(HtmlDocument htmlDoc, Comic comic)
        {
            List<Chapter> chapters = new List<Chapter>();
            HtmlNodeCollection htmlNodeCollection = htmlDoc.DocumentNode.SelectNodes("//div[@class='cVol']/div[@class='cVolList']/div");
            for (int ii = 0; ii < htmlNodeCollection.Count; ii++)
            {
                HtmlNode childNode = htmlNodeCollection[ii].FirstChild;
                Chapter chapter = new Chapter();
                chapter.Name = childNode.InnerText;
                chapter.Number = htmlNodeCollection.Count - ii;
                //chapter.UpdateTime = childNode.GetAttributeValue("title", "");
                chapter.Url = childNode.GetAttributeValue("href", "");
                chapters.Add(chapter);
            }
            return chapters;
        }

        private List<Page> AnalysisPage(HtmlDocument htmlDoc, Chapter chapter)
        {
            List<Page> pages = new List<Page>();
            HtmlNodeCollection htmlNodeCollection = htmlDoc.DocumentNode.SelectNodes("//script");
            foreach (HtmlNode hn in htmlNodeCollection)
            {
                if (hn.InnerText == null)
                {
                    continue;
                }
                if (!hn.InnerText.Contains("var sFiles="))
                {
                    continue;
                }
                Regex regex = new Regex(@"(/[\w+-]+)+.JPG");
                MatchCollection mc = regex.Matches(hn.InnerText);
                for (int ii = 0; ii < mc.Count; ii++)
                {
                    Page page = new Page();
                    page.Number = ii + 1;
                    Regex regex2 = new Regex(@"/ok-comic[0-9][0-9]/");
                    MatchCollection mc2 = regex2.Matches(mc[ii].Value);
                    string header = mc2[0].Value.Replace("/ok-comic", "").Replace("/", "");
                    page.Url = "http://cao.twcomic.com/dm" + header + mc[ii].Value;
                    pages.Add(page);
                }
                break;
            }
            return pages;
        }
    }
}
