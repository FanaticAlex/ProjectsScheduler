using ProjectsScheduler.Core;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace ProjectsScheduler.Desktop.ViewModel
{
    internal class SubResourceViewModel
    {
        public string Name
        {
            get { return SubResource?.Name; }
            set { SubResource.Name = value; }
        }

        public List<int> Load { get; set; } = new List<int>();

        public List<int> Vacations
        {
            get { return SubResource?.Vacations; }
            set { SubResource.Vacations = value; }
        }

        public SubResource SubResource { get; set; }

        public SubResourceViewModel()
        {
        }

        public SubResourceViewModel(SubResource subResource, List<ProjectTask> tasks, Result result)
        {
            SubResource = subResource;
            Load = GetResourceLoad(tasks, result);
        }

        private void Vacations_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            SubResource.Vacations.Clear();
            SubResource.Vacations.AddRange(Vacations);
        }

        private List<int> GetResourceLoad(List<ProjectTask> tasks, Result result)
        {
            if (tasks == null)
                return null;

            if (result == null)
                return null;

            var ret = new List<int>();
            foreach (var task in tasks)
            {
                if (result.TaskIdToSubtaskNumber[task.ID] == SubResource.SubResourceId)
                {
                    for (int i = 0; i < task.Duration; i++)
                    {
                        ret.Add(result.TaskIdToTaskStartTime[task.ID] + i);
                    }
                }
            }

            return ret;
        }
    }
}
