using System;
using ExtendedCL;

namespace DBRestorer.Domain
{
    public class Restorer
    {
        private readonly ISqlServerUtil _sqlUtil;

        public Restorer(ISqlServerUtil sqlUtil)
        {
            _sqlUtil = sqlUtil;
        }

        public void Restore(ISqlServerUtil.DbRestorOptions opt, IProgressBarProvider progressBarProvider,
            Action additionalCallbackOnCompleted)
        {
            _sqlUtil.Restore(opt, progressBarProvider, additionalCallbackOnCompleted);
        }
    }
}