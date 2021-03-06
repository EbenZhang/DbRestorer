﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ExtendedCL;

namespace DBRestorer.Ctrl.Domain
{
    public abstract class ISqlServerUtil
    {
        public static readonly List<string> SystemDatabases = new List<string>
        {
            "master",
            "tempdb",
            "model",
            "msdb"
        };

        public abstract List<string> GetSqlInstances();
        public abstract List<string> GetDatabaseNames(string instanceName);

        public abstract Task Restore(DbRestorOptions dbRestorOptions,
            IProgressBarProvider progressBarProvider, Action additionalCallbackOnCompleted);

        public struct DbRestorOptions
        {
            public string SqlServerInstName { get; set; }
            public string SrcPath { get; set; }
            public string TargetDbName { get; set; }
            public string RelocateMdfTo { get; set; }
            public string RelocateLdfTo { get; set; }
        }
    }
}