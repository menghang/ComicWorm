using System;
using System.Text.RegularExpressions;
using ComicModels;
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

        private void AnalysisChapter(HtmlDocument htmlDoc, ComicModel comic)
        {
            HtmlNodeCollection htmlNodeCollection = htmlDoc.DocumentNode.SelectNodes("//div[@class='cVol']/div[@class='cVolList']/div");
            for (int ii = 0; ii < htmlNodeCollection.Count; ii++)
            {
                HtmlNode childNode = htmlNodeCollection[ii].FirstChild;
                ChapterModel chapter = new ChapterModel();
                chapter.Name = childNode.InnerText;
                chapter.Number = htmlNodeCollection.Count - ii;
                //chapter.UpdateTime = childNode.GetAttributeValue("title", "");
                chapter.Url = childNode.GetAttributeValue("href", "");
                comic.Chapters.Add(chapter);
            }
        }

        private void AnalysisPage(HtmlDocument htmlDoc, ChapterModel chapter)
        {
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
                    PageModel page = new PageModel();
                    page.Number = ii + 1;
                    Regex regex2 = new Regex(@"/ok-comic[0-9][0-9]/");
                    MatchCollection mc2 = regex2.Matches(mc[ii].Value);
                    string header = mc2[0].Value.Replace("/ok-comic", "").Replace("/", "");
                    page.Url = "http://cao.twcomic.com/dm" + header + mc[ii].Value;
                    chapter.Pages.Add(page);
                }
            }
        }
    }
}
