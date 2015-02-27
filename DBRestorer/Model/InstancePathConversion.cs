using Microsoft.SqlServer.Management.Smo.Wmi;

namespace DBRestorer.Model
{
    public static class InstancePathConversion
    {
        public static string GetInstsPath(string machineName, string instName)
        {
            var path = machineName.Replace("LOCALHOST", ".") + "\\" + instName.Replace("MSSQLSERVER", ".");
            return path.Replace(".\\.", ".");
        }
    }
}