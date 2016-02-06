using DBRestorer.Plugin.Interface;
using System.ComponentModel.Composition;

namespace SamplePlugin
{
    [Export(typeof(IPostDbRestore))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class SamplePlugin : IPostDbRestore
    {
        public void OnDBRestored(string sqlInstName, string dbName)
        {
        }
    }
}
