using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using static Google.OrTools.ConstraintSolver.RoutingModel.ResourceGroup;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal interface INode
    {
        public string Name { get; }
    }

    internal class Node : INode
    {
        public string Name { get; set; }
        public ObservableCollection<INode> Children { get; set; } = new ObservableCollection<INode>();
        public object Original { get; set; }
    }

    internal class ProjectsSetViewModel
    {
        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node> { };
        public Object? SelectedObject { get; set; }

        public void SetProjectSet(ProjectsSet projectsSet)
        {
            Nodes.Clear();
            var resourcesVM = projectsSet.Resources
                .Select(r => new ResourceViewModel(r, null, null, projectsSet.Resources)).ToList();
            foreach (var project in projectsSet.ProjectList)
            {
                var projectVM = new ProjectViewModel(project, null, resourcesVM);
                var projectNode = new Node() { Name = project.Name, Original = projectVM};
                Nodes.Add(projectNode);
                foreach (var task in projectVM.Tasks)
                {
                    projectNode.Children.Add(task);
                }
            }

            var resourceNode = new Node() { Name = "Ресурсы" };
            foreach (var resourceVM in resourcesVM)
            {
                resourceNode.Children.Add(resourceVM);
            }

            Nodes.Add(resourceNode);
        }
    }
}
