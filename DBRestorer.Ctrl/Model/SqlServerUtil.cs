using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceProcess;
using System.Threading.Tasks;
using DBRestorer.Domain;
using DBRestorer.Model;
using ExtendedCL;
using Microsoft.SqlServer.Management.Smo;

namespace DBRestorer.Ctrl.Model
{
    public class SqlServerUtil : ISqlServerUtil
    {
        public static readonly string FinishedRestore = "Finished Restoring.";

        public override List<string> GetSqlInstances()
        {
            var services = ServiceController.GetServices().Where(x => x.Status == ServiceControllerStatus.Running);
            return services.Where(r => IsMsSqlService(r)
                || IsDefaultInstServiceName(r))
                .Select(NormalizeInstName)
                .Distinct()
                .ToList();
        }

        private static string NormalizeInstName(ServiceController r)
        {
            return InstancePathConversion.GetInstsPath(
                "LOCALHOST", r.ServiceName.ToUpperInvariant().Replace("MSSQL$", ""));
        }

        private static bool IsMsSqlService(ServiceController r)
        {
            return r.ServiceName.ToUpperInvariant().StartsWith("MSSQL$");
        }

        private static bool IsDefaultInstServiceName(ServiceController r)
        {
            return r.ServiceName.ToUpperInvariant() == "MSSQLSERVER";
        }

        public override List<string> GetDatabaseNames(string instanceName)
        {
            var server = new Server(instanceName);
            return (from Database db in server.Databases select db.Name).ToList();
        }

        public override async Task Restore(DbRestorOptions opt, IProgressBarProvider progressBarProvider,
            Action additionalCallbackOnCompleted)
        {
            await Task.Run(() =>
            {
                var srv = new Server(opt.SqlServerInstName);
                if (srv.Databases.Contains(opt.TargetDbName))
                {
                    srv.KillDatabase(opt.TargetDbName);
                }
                var res = new Restore();
                srv.ConnectionContext.StatementTimeout = 0;
                res.Devices.AddDevice(opt.SrcPath, DeviceType.File);
                
                res.Database = opt.TargetDbName;
                res.Action = RestoreActionType.Database;
                res.PercentCompleteNotification = 1;
                res.ReplaceDatabase = true;
                res.Complete += (sender, args) =>
                {
                    if(res.AsyncStatus.ExecutionStatus == ExecutionStatus.Failed)
                    {
                        progressBarProvider.OnError(args.Error.ToString());
                        return;
                    }
                    progressBarProvider.OnCompleted(FinishedRestore);
                    additionalCallbackOnCompleted?.Invoke();
                };
                res.PercentComplete += (sender, args) =>
                {
                    if (res.AsyncStatus.ExecutionStatus == ExecutionStatus.Failed)
                    {
                        progressBarProvider.OnError(res.AsyncStatus.LastException.ToString());
                    }
                    // give 10% for the recovering which happens after restoring.
                    const double WeightOfRestoring = 90.0 / 100.0;
                    progressBarProvider.ReportProgress((int)(args.Percent * WeightOfRestoring));
                };

                var fileList = res.ReadFileList(srv);

                var dataFile = new RelocateFile(
                    logicalFileName: fileList.Rows[0][0].ToString(),
                    physicalFileName: opt.RelocateMdfTo
                    );

                var logFile = new RelocateFile(
                    logicalFileName: fileList.Rows[1][0].ToString(),
                    physicalFileName: opt.RelocateLdfTo
                    );

                res.RelocateFiles.Add(dataFile);
                res.RelocateFiles.Add(logFile);

                res.ContinueAfterError = false;

                progressBarProvider.Start(true, "Restoring...");
                res.SqlRestoreAsync(srv);
            });
        }
    }
}