using System;

namespace AnalysisModels
{
    public interface IAnalysisModel
    {
        Tuple<string, AnalysisChapterHandler, AnalysisPageHandler> GetAnalysisModel();

        string GetWebset();
    }
}
