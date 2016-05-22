using System.ComponentModel.Composition;
using System.Windows;
using DBRestorer.Plugin.Interface;

namespace Plugin_DbRestorerConfig.Plugin_ExecutionOrder
{
    [Export(typeof(IDbRestorerSettings))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    public class PluginExecutionOrder : IDbRestorerSettings
    {
        public string Name => "Plugin Execution Order";

        public void ShowSettings(Window ownerWindow)
        {
            var wnd = new PluginExecutionOrderView {Owner = ownerWindow};
            wnd.ShowDialog();
        }
    }
}
