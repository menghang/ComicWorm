using System;

namespace ComicWorm
{
    public interface IAnalysisModel
    {
        Tuple<string, AnalysisChapterHandler, AnalysisPageHandler> GetAnalysisModel();

        string GetWebset();
    }
}
