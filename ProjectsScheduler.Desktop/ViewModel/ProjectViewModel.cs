using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class ProjectViewModel : INode
    {
        public string Name
        {
            get { return Project?.Name; }
            set { Project.Name = value; }
        }

        public int? Deadline
        {
            get { return Project.Deadline; }
            set { Project.Deadline = value; }
        }

        public List<TaskViewModel> Tasks { get; set; }

        public Project Project { get; set; }

        public ProjectViewModel(Project project, Result result, List<ResourceViewModel> resources)
        {
            Project = project;
            Tasks = project.Tasks.Select(t => new TaskViewModel(t, result, resources)).ToList();
        }
    }
}
