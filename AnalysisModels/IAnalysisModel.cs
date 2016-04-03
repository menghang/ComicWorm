using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AnalysisModels
{
    public interface IAnalysisModel
    {
        Tuple<string,AnalysisChapterHandler, AnalysisPageHandler> GetAnalysisModel();

        string GetWebset();
    }
}
