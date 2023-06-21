using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class ResourceViewModel
    {
        public string Name { get; set; }
        public Color ResourceColor { get; set; }
        public List<int> Load { get; set; }
    }

    internal class ProjectViewModel
    {
        public string Name { get; set; }
        public List<TaskViewModel> Tasks { get; set; }
    }

    internal class TaskViewModel
    {
        public string Name { get; set; }
        public int Start { get; set; }
        public int Duration { get; set; }
        public Color ResourceColor { get; set; }
    }

    internal class TasksSchedulingResultsViewModel
    {
        public List<ProjectViewModel> Projects { get; set; }

        public List<ResourceViewModel> Resources { get; set; }

        public int TimeMax { get; set; }

        public void SetData(Result result, ProjectsSet inputData)
        {
            var tasks = inputData.ProjectList.SelectMany(p => p.Tasks).ToList();
            TimeMax = result.OverallTime;

            var resourcesToColors = GetResourceToColor(inputData.Resources);
            Projects = inputData.ProjectList.Select(p => GetProjectViewModel(p, result, resourcesToColors)).ToList();

            Resources = inputData.Resources.Select(r => GetResourceViewModel(r, result, tasks, resourcesToColors)).ToList();
        }

        private static ProjectViewModel GetProjectViewModel(Project project, Result result, Dictionary<string, Color> resourcesToColors)
        {
            var tasks = project.Tasks.Select(t => new TaskViewModel
            {
                Name = t.ResourceName + t.Duration,
                Duration = t.Duration,
                Start = result.TaskIdToTaskStartTime[t.ID],
                ResourceColor = resourcesToColors[t.ResourceName]
            }).ToList();

            return new ProjectViewModel()
            {
                Name = project.Name,
                Tasks = tasks
            };
        }

        private static ResourceViewModel GetResourceViewModel(Resource resource, Result result, List<ProjectTask> tasks, Dictionary<string, Color> resourcesToColors)
        {
            var resourceTasks = tasks.Where(t => t.ResourceName == resource.Name).ToList();
            return new ResourceViewModel()
            {
                Name = resource.Name,
                ResourceColor = resourcesToColors[resource.Name],
                Load = GetResourceLoad(resourceTasks, result)
            };
        }

        private static List<int> GetResourceLoad(List<ProjectTask> tasks, Result result)
        {
            var ret = new List<int>();
            foreach(var task in tasks)
            {
                for (int i = 0; i < task.Duration; i++)
                {
                    ret.Add(result.TaskIdToTaskStartTime[task.ID] + i);
                }
            }

            return ret;
        }

        private static Dictionary<string, Color> GetResourceToColor(List<Resource> resources)
        {
            var colors = new List<Color>()
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
    
            if (resources.Count > colors.Count)
            {
                Console.WriteLine("Too many resource");
                return null;
            }

            var result = new Dictionary<string, Color>();
            for (var i = 0; i < resources.Count; i++)
            {
                result.Add(resources[i].Name, colors[i]);
            }

            return result;
        }
    }
}
