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

        public ModelTask(ProjectTask task, CpModel model, int horizon)
        {
            Task = task;
            Start = model.NewIntVar(0, horizon, task.ID);
            End = model.NewIntVar(0, horizon, task.ID);
            Interval = model.NewIntervalVar(Start, task.Duration, End, task.ID);
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
                    // таск
                    var modelTask = new ModelTask(task, model, projectSet.horizon);
                    modelProject.ModelTasks.Add(modelTask);

                    // ресурс
                    var modelResource = ModelResources
                        .FirstOrDefault(modelResource => modelResource.Resource.Name == task.ResourceName);
                    if (modelResource == null)
                    {
                        var resource = projectSet.Resources.Single(resource => resource.Name == task.ResourceName);
                        modelResource = new ModelResource(resource);
                        ModelResources.Add(modelResource);
                    }

                    modelResource.Tasks.Add(modelTask); // для подсчета загруженности ресурсов
                }
            }


            // Objective: minimize the makespan (maximum end times of all tasks) of the problem.
            makespan = model.NewIntVar(0, projectSet.horizon, "makespan");
        }
    }
}
