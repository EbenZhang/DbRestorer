using System.Linq;
using System.Windows;
using System.Windows.Input;
using DBRestorer.Ctrl;
using DBRestorer.Ctrl.Domain;
using DBRestorer.Domain;
using DBRestorer.Plugin.Interface;
using GalaSoft.MvvmLight.Command;
using PropertyChanged;
using WpfCommon.Utils;

namespace Plugin_DbRestorerConfig.Plugin_ExecutionOrder
{
    [AddINotifyPropertyChangedInterface]
    public class PluginExecutionOrderVm : ViewModelBaseEx
    {
        public ExecutionOrderProvider ExecutionPlans { get; set; } = ExecutionOrderProvider.Instance.Value;

        public ICommand DeleteCmd
        {
            get
            {
                return new RelayCommand(Delete, 
                    () => ExecutionPlans.Plans.Count > 1 && ExecutionPlans.CurrentPlan != null);
            }
        }

        public ICommand RenameCmd
        {
            get
            {
                return new RelayCommand<Window>(Rename, w => ExecutionPlans.CurrentPlan != null);
            }
        }

        public ICommand AddCmd
        {
            get { return new RelayCommand<Window>(AddNew, w => ExecutionPlans.CurrentPlan != null); }
        }

        public ICommand AddNewPluginCmd => new RelayCommand<Window>(AddNewPlugin);

        private void AddNewPlugin(Window w)
        {
            var wnd = new PluginsListWnd {Owner = w};
            var ret = wnd.ShowDialog();
            if (!ret.HasValue || !ret.Value) return;
            foreach (var plugin in wnd.SelectedPlugins)
            {
                if (!ExecutionPlans.CurrentPlan.ExecutionOrder.Contains(plugin))
                {
                    ExecutionPlans.CurrentPlan.ExecutionOrder.Add(plugin);
                }
            }
        }

        public ICommand RemovePluginCmd
        {
            get
            {
                return new RelayCommand(RemovePlugin,
                    () => !string.IsNullOrWhiteSpace(SelectedPlugin));
            }
        }

        private void RemovePlugin()
        {
            var index = ExecutionPlans.CurrentPlan.ExecutionOrder.IndexOf(SelectedPlugin);
            ExecutionPlans.CurrentPlan.ExecutionOrder.Remove(SelectedPlugin);
            if (index > 0)
            {
                SelectedPlugin = ExecutionPlans.CurrentPlan.ExecutionOrder[index - 1];
            }
        }

        public ICommand MoveUpPluginCmd
        {
            get { return new RelayCommand(MoveUpPlugin, 
                () => !string.IsNullOrWhiteSpace(SelectedPlugin) 
                && ExecutionPlans.CurrentPlan.ExecutionOrder.IndexOf(SelectedPlugin) != 0); }
        }

        private void MoveUpPlugin()
        {
            var plugin = SelectedPlugin;
            var index = ExecutionPlans.CurrentPlan.ExecutionOrder.IndexOf(SelectedPlugin);
            ExecutionPlans.CurrentPlan.ExecutionOrder.RemoveAt(index);
            ExecutionPlans.CurrentPlan.ExecutionOrder.Insert(index - 1, plugin);
            SelectedPlugin = plugin;
        }

        public ICommand MoveDownPluginCmd
        {
            get
            {
                return new RelayCommand(MoveDownPlugin,
                    () => !string.IsNullOrWhiteSpace(SelectedPlugin)
                          && ExecutionPlans.CurrentPlan.ExecutionOrder.IndexOf(SelectedPlugin) 
                          != ExecutionPlans.CurrentPlan.ExecutionOrder.Count - 1);
            }
        }

        private void MoveDownPlugin()
        {
            var plugin = SelectedPlugin;
            var index = ExecutionPlans.CurrentPlan.ExecutionOrder.IndexOf(SelectedPlugin);
            ExecutionPlans.CurrentPlan.ExecutionOrder.RemoveAt(index);
            ExecutionPlans.CurrentPlan.ExecutionOrder.Insert(index + 1, plugin);
            SelectedPlugin = plugin;
        }

        public string SelectedPlugin { get; set; }

        private void AddNew(Window window)
        {
            var wnd = new ExecutionOrderNameView(originalName: "") {Owner = window};
            var choice = wnd.ShowDialog();
            if (!choice.HasValue || !choice.Value) return;
            var newPlan = new Plan()
            {
                PlanId = ExecutionPlans.Plans.Max(r => r.PlanId) + 1,
                PlanName = wnd.NewName,
            };
            newPlan.ExecutionOrder.AddRange(Plugins.GetPlugins<IPostDbRestore>().Select(r => r.Value.PluginName));
            ExecutionPlans.Plans.Add(newPlan);
            ExecutionPlans.CurrentPlan = newPlan;
            ExecutionOrderProvider.SavePlan(newPlan);
        }

        private void Rename(Window window)
        {
            var wnd = new ExecutionOrderNameView(ExecutionPlans.CurrentPlan.PlanName) {Owner = window};
            var choice = wnd.ShowDialog();
            if (!choice.HasValue || !choice.Value) return;
            ExecutionPlans.CurrentPlan.PlanName = wnd.NewName;
            ExecutionOrderProvider.SavePlan(ExecutionPlans.CurrentPlan);
        }

        private void Delete()
        {
            ExecutionOrderProvider.DeletePlan(ExecutionPlans.CurrentPlan);
            var curIndex = ExecutionPlans.Plans.IndexOf(ExecutionPlans.CurrentPlan);
            ExecutionPlans.Plans.Remove(ExecutionPlans.CurrentPlan);

            if (curIndex > 0)
            {
                ExecutionPlans.CurrentPlan = ExecutionPlans.Plans.ElementAt(curIndex - 1);
            }
        }
    }
}
