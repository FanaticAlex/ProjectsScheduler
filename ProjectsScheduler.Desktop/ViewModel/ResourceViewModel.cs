using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class ResourceViewModel
    {
        public string Name
        {
            get { return Resource.Name; }
            set { Resource.Name = value; }
        }

        public Color ResourceColor { get; set; }

        public ProjectResource Resource { get; set; }

        public List<SubResourceViewModel> SubResources { get; set; } = new List<SubResourceViewModel>();

        public static List<Color> DefaultColors = new List<Color>()
        {
            Color.FromRgb(125, 159, 211),
            Color.FromRgb(246, 231, 35),
            Color.FromRgb(155, 226, 0),
            Color.FromRgb(167, 127, 170),
            Color.FromRgb(224, 183, 99),
            Color.FromRgb(161, 127, 255),
            Color.FromRgb(0, 158, 18),
            Color.FromRgb(168, 117, 84),
            Color.FromRgb(226, 90, 0),
            Color.FromRgb(255, 0, 100),
            Color.FromRgb(255, 100, 100),
            Color.FromRgb(255, 100, 0),
        };

        public ResourceViewModel()
        {
            Resource = new ProjectResource();
        }

        public ResourceViewModel(ProjectResource resource, Result result, List<ProjectTask> tasks, List<ProjectResource> resources)
        {
            Resource = resource;
            ResourceColor = GetNextColor(resources.IndexOf(resource));
            var resourceTasks = tasks?.Where(t => t.ResourceName == Name).ToList();
            SubResources = resource.SubResources.Select(sr => new SubResourceViewModel(sr, resourceTasks, result)).ToList();
        }

        private Color GetNextColor(int number)
        {
            var color = DefaultColors[number];
            return color;
        }
    }
}
