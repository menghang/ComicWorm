using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicWormCore
{
    public static class AnalysisModels
    {
        private static readonly List<string> siteList = new List<string> { "www.hanhan88.com", "www.fumanhua.net",
            "www.mangago.me", "www.bbhou.com", "www.xindm.cn", "www.chuixue.com", "www.77mh.com",
            "www.seemh.com", "www.manhuatai.com", "manhua.dmzj.com", "www.tuku.cc", "www.twcomic.com",
            "manhua.upuigma.com", "dm5.com", "www.veryim.net", "www.kxdm.com", "www.ccmanhua.com" };

        public class AnalysisMethod
        {
            public DownloadModel.AnalysisChapterHandler AnalysisChapter;
            public DownloadModel.AnalysisPageHandler AnalysisPage;
        }

        public static AnalysisMethod GetAnalysisMethod (string _url)
        {
            string webset;
            if (_url.StartsWith("http://"))
            {
                webset = (_url.Remove("http://".Length).Split('/'))[0]; }
            else if (_url.StartsWith("https://"))
            {
                webset = (_url.Remove("https://".Length).Split('/'))[0];
            }
            else
            {
                webset = (_url.Split('/'))[0];
            }

            return AnalysisMethods.ContainsKey(webset) ? AnalysisMethods[webset] : null;
        }

        private static readonly Dictionary<string, AnalysisMethod> AnalysisMethods
            = new Dictionary<string, AnalysisMethod>
            {
                ["http://www.hanhan88.com/"] = new AnalysisMethod
                {
                    AnalysisChapter = (htmlDoc, comic) =>
                    {

                    },
                    AnalysisPage = (htmlDoc, chapter) =>
                    {

                    }
                }
            };
    }
}
