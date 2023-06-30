using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class Node
    {
        public string Name { get; set; }
        public ObservableCollection<Node> Children { get; set; } = new ObservableCollection<Node>();
        public object Original { get; set; }
    }

    internal class ProjectsSetViewModel
    {
        public ObservableCollection<Node> Nodes { get; set; } = new ObservableCollection<Node> { };
        public Object? SelectedObject { get; set; }

        public void SetProjectSet(ProjectsSet projectsSet)
        {
            Nodes.Clear();
            var resourcesVM = projectsSet.Resources.Select(r => new ResourceViewModel(r, null, null, projectsSet.Resources)).ToList();
            foreach (var project in projectsSet.ProjectList)
            {
                var projectVM = new ProjectViewModel(project, null, resourcesVM);
                var projectNode = new Node() { Name = project.Name, Original = projectVM};
                Nodes.Add(projectNode);
                foreach (var task in projectVM.Tasks)
                {
                    var taskNode = new Node() { Name = task.Name, Original = task };
                    projectNode.Children.Add(taskNode);
                }
            }

            var resourceRootNode = new Node() { Name = "Ресурсы" };
            Nodes.Add(resourceRootNode);
            foreach (var resourceVM in resourcesVM)
            {
                var resourceNode = new Node() { Name = resourceVM.Name, Original = resourceVM };
                resourceRootNode.Children.Add(resourceNode);
                foreach(var subResource in  resourceVM.SubResources)
                {
                    var subResourceNode = new Node() { Name =  subResource.Name, Original = subResource };
                    resourceNode.Children.Add(subResourceNode);
                }
            }
        }
    }
}
