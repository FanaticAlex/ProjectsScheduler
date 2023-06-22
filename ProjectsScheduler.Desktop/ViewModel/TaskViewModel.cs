using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class TaskViewModel : INode
    {
        public string Name
        {
            get { return ProjectTask.ResourceName + ProjectTask.Duration; }
            set { }
        }
        public int? Start { get; set; }
        public int Duration
        {
            get { return ProjectTask.Duration; }
            set { ProjectTask.Duration = value; }
        }

        public Color ResourceColor { get; set; }

        public ProjectTask ProjectTask { get; set; }

        public TaskViewModel()
        {
            ProjectTask = new ProjectTask(0, "test");
        }

        public TaskViewModel(ProjectTask task, Result result, Dictionary<string, Color> resourcesToColors)
        {
            if (task == null)
                return;

            ProjectTask = task;
            Start = result?.TaskIdToTaskStartTime[task.ID];
            ResourceColor = resourcesToColors[task.ResourceName];
        }
    }
}
