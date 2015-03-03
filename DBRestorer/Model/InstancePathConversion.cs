using System;

namespace DBRestorer.Model
{
    public static class InstancePathConversion
    {
        public static string GetInstsPath(string machineName, string instName)
        {
            if (string.Compare(instName, "MSSQLSERVER", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                instName = ".";
            }
            var path = machineName.Replace("LOCALHOST", ".") + "\\" + instName;
            return path.Replace(".\\.", ".");
        }
    }
}