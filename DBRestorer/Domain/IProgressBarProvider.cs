using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBRestorer.Domain
{
    public interface IProgressBarProvider
    {
        void Start(bool willReportProgress, string taskDesc);
        void OnError(string errMsg);
        void OnCompleted(string msg);
        void ReportProgress(int percent);
    }
}
