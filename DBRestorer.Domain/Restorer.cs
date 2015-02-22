namespace DBRestorer.Domain
{
    public class Restorer
    {
        private readonly ISqlServerUtil _sqlUtil;
        public event ISqlServerUtil.ProgressReport OnProgress;
        public event ISqlServerUtil.ErrorReport OnError;
        public Restorer(ISqlServerUtil sqlUtil)
        {
            this._sqlUtil = sqlUtil;
        }

        public void Restore(ISqlServerUtil.DbRestorOptions opt)
        {
            _sqlUtil.Restore(opt, OnProgress, OnError);
        }
    }
}