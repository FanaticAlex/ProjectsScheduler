using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class TaskViewModel : BaseVewModel, INode
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

        private ResourceViewModel _resource;
        public ResourceViewModel Resource
        {
            get { return _resource; }
            set
            {
                _resource = value;
                ProjectTask.ResourceName = value.Name;
                OnPropertyChanged(nameof(Name));
            }
        }

        public List<ResourceViewModel> ResourcesSet { get; set; } = new List<ResourceViewModel>();

        public ProjectTask ProjectTask { get; set; }

        public TaskViewModel()
        {
            ProjectTask = new ProjectTask(0, "test");
        }

        public TaskViewModel(ProjectTask task, Result result, List<ResourceViewModel> resources)
        {
            if (task == null)
                return;

            ResourcesSet = resources;

            ProjectTask = task;
            Start = result?.TaskIdToTaskStartTime[task.ID];
            Resource = resources.Single(r => r.Name == task.ResourceName);
        }
    }
}
