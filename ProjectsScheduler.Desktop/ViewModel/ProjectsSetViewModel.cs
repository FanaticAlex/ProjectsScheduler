using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal interface INode
    {
        public string Name { get; }
    }

    internal class Node : INode
    {
        public string Name { get; set; }
        public List<INode> Children { get; set; } = new List<INode>();
        public object Original { get; set; }
    }

    internal class ProjectsSetViewModel
    {
        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node> { };
        public ProjectViewModel SelectedNode { get; set; }

        public void SetProjectSet(ProjectsSet projectsSet)
        {
            Nodes.Clear();

            var resourcesToColors = ResourceViewModel.GetResourceToColor(projectsSet.Resources);
            foreach (var project in projectsSet.ProjectList)
            {
                var projectVM = new ProjectViewModel(project, null, resourcesToColors);
                var projectNode = new Node() { Name = project.Name, Original = projectVM};
                Nodes.Add(projectNode);
                foreach (var task in projectVM.Tasks)
                {
                    projectNode.Children.Add(task);
                }
            }

            var resourceNode = new Node() { Name = "Ресурсы" };
            foreach (var resource in projectsSet.Resources)
            {
                resourceNode.Children.Add(new ResourceViewModel(resource, null, null, resourcesToColors));
            }

            Nodes.Add(resourceNode);
        }
    }
}
