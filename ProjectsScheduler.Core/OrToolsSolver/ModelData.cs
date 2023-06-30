using Google.OrTools.Sat;
using ProjectsScheduler.Core.InputData;
using System.Threading.Tasks;
using static Google.OrTools.ConstraintSolver.RoutingModel.ResourceGroup;

namespace ProjectsScheduler.Core.OrToolsSolver
{
    internal class ModelProject
    {
        public Project Project { get; set; }
        public List<ModelTask> ModelTasks { get; set; } = new List<ModelTask>();
        public IntVar Deadline { get; set; }

        public ModelProject(Project project, CpModel model)
        {
            Project = project;
            if (project.Deadline != null)
                Deadline = model.NewConstant(project.Deadline.Value);
        }
    }

    internal class ModelTask
    {
        public ProjectTask Task { get; set; }
        public IntVar Start { get; set; }
        public IntVar End { get; set; }
        public IntervalVar Interval { get; set; }

        public IntVar SubresourceNumber { get; set; }

        public ModelTask(ProjectTask task, CpModel model, int horizon, int subResourceCount)
        {
            if (subResourceCount == 0)
                throw new Exception("У ресурса нет субресурсов");

            Task = task;
            Start = model.NewIntVar(0, horizon, "s" + task.ID);
            End = model.NewIntVar(0, horizon, "e" + task.ID);
            Interval = model.NewIntervalVar(Start, task.Duration, End, "i" + task.ID);
            SubresourceNumber = model.NewIntVar(0, subResourceCount - 1, "sub" + task.ID);
        }
    }

    internal class ModelResource
    {
        public ProjectResource Resource { get; set; }
        public List<ModelTask> Tasks { get; set; } = new List<ModelTask>();

        public ModelResource(ProjectResource resource)
        {
            Resource = resource;
        }
    }

    internal class ModelData
    {
        public List<ModelProject> ModelProjects { get; set; } = new List<ModelProject>();

        /// <summary>
        /// это просто альтернативная группировка тасков по ресурсам
        /// </summary>
        public List<ModelResource> ModelResources { get; set; } = new List<ModelResource>();

        /// <summary>
        /// Перемменная обозначает времени нужное для завершения всех проектов
        /// </summary>
        public IntVar makespan;

        public ModelData(ProjectsSet projectSet, CpModel model)
        {
            foreach (var project in projectSet.ProjectList)
            {
                // проект
                var modelProject = new ModelProject(project, model);
                ModelProjects.Add(modelProject);
                foreach (var task in project.Tasks)
                {
                    // ресурс
                    var modelResource = ModelResources
                        .FirstOrDefault(modelResource => modelResource.Resource.Name == task.ResourceName);
                    if (modelResource == null)
                    {
                        var resource = projectSet.Resources.Single(resource => resource.Name == task.ResourceName);
                        modelResource = new ModelResource(resource);
                        ModelResources.Add(modelResource);
                    }

                    // таск
                    var modelTask = new ModelTask(task, model, projectSet.horizon, modelResource.Resource.SubResources.Count());
                    modelProject.ModelTasks.Add(modelTask);

                    

                    modelResource.Tasks.Add(modelTask); // для подсчета загруженности ресурсов
                }
            }


            // Objective: minimize the makespan (maximum end times of all tasks) of the problem.
            makespan = model.NewIntVar(0, projectSet.horizon, "makespan");
        }
    }
}
