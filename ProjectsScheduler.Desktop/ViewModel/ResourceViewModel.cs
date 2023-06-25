using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class ResourceViewModel : INode
    {
        public string Name
        {
            get { return Resource.Name; }
            set { Resource.Name = value; }
        }
        public Color ResourceColor { get; set; }
        public List<int> Load { get; set; }

        public Resource Resource { get; set; }

        public string Vacations => $"[{String.Join(",", Resource.Vacations)}]";

        public static List<Color> DefaultColors = new List<Color>()
        {
            Color.FromRgb(125, 159, 211),
            Color.FromRgb(246, 231, 35),
            Color.FromRgb(155, 226, 0),
            Color.FromRgb(167, 127, 170),
            Color.FromRgb(224, 183, 99),
            Color.FromRgb(161, 127, 255),
            Color.FromRgb(128, 128, 128),
            Color.FromRgb(168, 117, 84),
            Color.FromRgb(226, 90, 0),
            Color.FromRgb(255, 0, 100),
            Color.FromRgb(255, 100, 100),
            Color.FromRgb(255, 100, 0),
        };

        public ResourceViewModel()
        {
            Resource = new Resource();
        }

        public ResourceViewModel(Resource resource, Result result, List<ProjectTask> tasks, List<Resource> resources)
        {
            Resource = resource;
            var resourceTasks = tasks?.Where(t => t.ResourceName == resource.Name).ToList();
            ResourceColor = GetNextColor(resources.IndexOf(resource));
            Load = GetResourceLoad(resourceTasks, result);
        }

        public int GetMaxParallelTask(int time)
        {
            var isVacation = Resource.Vacations.Contains(time) ? 1 : 0;
            return Resource.MaxParallelTasks - isVacation;
        }

        private Color GetNextColor(int number)
        {
            var color = DefaultColors[number];
            return color;
        }

        private static List<int> GetResourceLoad(List<ProjectTask> tasks, Result result)
        {
            if (tasks == null)
                return null;

            if (result == null)
                return null;

            var ret = new List<int>();
            foreach (var task in tasks)
            {
                for (int i = 0; i < task.Duration; i++)
                {
                    ret.Add(result.TaskIdToTaskStartTime[task.ID] + i);
                }
            }

            return ret;
        }
    }
}
