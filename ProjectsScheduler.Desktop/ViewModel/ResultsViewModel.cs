using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsScheduler.Desktop.ViewModel
{

    internal class ResultsViewModel
    {
        public List<ProjectViewModel> Projects { get; set; }

        public List<ResourceViewModel> Resources { get; set; }

        public int TimeMax { get; set; }

        public void SetData(Result result, ProjectsSet inputData)
        {
            var tasks = inputData.ProjectList.SelectMany(p => p.Tasks).ToList();
            TimeMax = result.OverallTime;

            var resourcesVM = inputData.Resources.Select(r => new ResourceViewModel(r, result, tasks, inputData.Resources)).ToList();
            Projects = inputData.ProjectList.Select(p => new ProjectViewModel(p, result, resourcesVM)).ToList();
            Resources = inputData.Resources.Select(r => new ResourceViewModel(r, result, tasks, inputData.Resources)).ToList();
        }

        public void Clear()
        {
            Projects = new List<ProjectViewModel>();
            Resources = new List<ResourceViewModel>();
            TimeMax = 0;
        }
    }
}
