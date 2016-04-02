using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComicWormCore
{
    public interface IAnalysisModel
    {
        Tuple<string, DownloadModel.AnalysisChapterHandler, DownloadModel.AnalysisPageHandler> GetAnalysisModel();
    }
}
