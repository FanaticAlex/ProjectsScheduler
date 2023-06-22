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

            var resourcesToColors = ResourceViewModel.GetResourceToColor(inputData.Resources);
            Projects = inputData.ProjectList.Select(p => new ProjectViewModel(p, result, resourcesToColors)).ToList();
            Resources = inputData.Resources.Select(r => new ResourceViewModel(r, result, tasks, resourcesToColors)).ToList();
        }
    }
}
