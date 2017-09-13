using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml.Serialization;
using DBRestorer.Plugin.Interface;
using Nicologies;
using PropertyChanged;
using Nicologies.WpfCommon.Utils;

namespace DBRestorer.Ctrl.Domain
{
    [AddINotifyPropertyChangedInterface]
    public class Plan
    {
        public string PlanName { get; set; }
        public int PlanId { get; set; }
        public ObservableCollection<string> ExecutionOrder { get; set; } = new ObservableCollection<string>();
    }

    [AddINotifyPropertyChangedInterface]
    public class ExecutionOrderProvider
    {
        public static Lazy<ExecutionOrderProvider> Instance = new Lazy<ExecutionOrderProvider>(() => new ExecutionOrderProvider());
        private static readonly string PlansFolder = Path.Combine(PathHelper.ProcessAppDir, "Plans");
        private static readonly string LastPlanIdFilePath = Path.Combine(PlansFolder, "LastPlanId.txt");
        static ExecutionOrderProvider()
        {
            CreatePlansFolderIfNotExists();
        }
        private ExecutionOrderProvider()
        {
            LoadPlans();
        }
        public ObservableCollection<Plan> Plans { get; set; } = new ObservableCollection<Plan>();
        public Plan CurrentPlan { get; set; }

        public void SavePlans()
        {
            foreach (var plan in Plans)
            {
                SavePlan(plan);
            }
        }

        public static void SavePlan(Plan plan)
        {
            var planFile = GetPlanFilePath(plan);
            if (File.Exists(planFile))
            {
                File.Delete(planFile);
            }
            try
            {
                using (var stream = File.OpenWrite(planFile))
                {
                    new XmlSerializer(typeof (Plan)).Serialize(stream, plan);
                }
            }
            catch
            {
            }
        }

        private static string GetPlanFilePath(Plan plan)
        {
            var planFile = Path.Combine(PlansFolder, "plan" + plan.PlanId + ".xml");
            return planFile;
        }

        public void LoadPlans()
        {
            var plans = GetAllPlans();
            if (!plans.Any())
            {
                plans.Add(CreateDefaultPlan());
            }
            Plans.AddRange(plans);
            var isOnlyDefaultPlan = plans.Count == 1;
            if (isOnlyDefaultPlan)
            {
                CurrentPlan = Plans.First();
            }
            else
            {
                LoadLastUsedPlanId();
            }
        }

        private void LoadLastUsedPlanId()
        {
            CurrentPlan = Plans.FirstOrDefault();
            if (!File.Exists(LastPlanIdFilePath))
            {
                return;
            }
            var id = File.ReadAllLines(LastPlanIdFilePath).FirstOrDefault();
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            CurrentPlan = Plans
                .FirstOrDefault(r => r.PlanId.ToString(CultureInfo.InvariantCulture) == id) ?? Plans.First();
        }

        private static List<Plan> GetAllPlans()
        {
            var plansFolderInfo = new DirectoryInfo(PlansFolder);
            var planFiles = plansFolderInfo.GetFiles("plan*.xml").ToList();
            return CreatePlansFromFiles(planFiles);
        }

        private static List<Plan> CreatePlansFromFiles(List<FileInfo> planFiles)
        {
            return planFiles.Select(LoadPlanFromFile).Where(plan => plan != null).ToList();
        }

        private static Plan LoadPlanFromFile(FileInfo file)
        {
            try
            {
                using (var stream = file.OpenRead())
                {
                    return (Plan) new XmlSerializer(typeof (Plan)).Deserialize(stream);
                }
            }
            catch
            {
                return null;
            }
        }

        private static void CreatePlansFolderIfNotExists()
        {
            if (!Directory.Exists(PlansFolder))
            {
                Directory.CreateDirectory(PlansFolder);
            }
        }

        private static Plan CreateDefaultPlan()
        {
            var plugins = Plugins.GetPlugins<IPostDbRestore>();
            var plan = new Plan()
            {
                PlanId = 0,
                PlanName = "Default Plugins Execution Order",
            };
            plan.ExecutionOrder.AddRange(plugins.Select(r => r.Value.PluginName));
            SavePlan(plan);
            return plan;
        }

        public static void DeletePlan(Plan plan)
        {
            var planFile = GetPlanFilePath(plan);
            if (File.Exists(planFile))
            {
                File.Delete(planFile);
            }
        }
    }
}
