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