﻿using Google.OrTools.Sat;
using ProjectsScheduler.Data;

namespace ProjectsScheduler.OrToolsSolver
{
    class ModelProject
    {
        public Project Project { get; set; }
        public List<ModelTask> ModelTasks { get; set; } = new List<ModelTask>();
    }

    class ModelTask
    {
        public ProjectTask Task { get; set; }
        public IntVar Start { get; set; }
        public IntVar End { get; set; }
        public IntervalVar Interval { get; set; }
    }

    class ModelResource
    {
        public string ResourceName { get; set; }
        public List<ModelTask> Tasks { get; set; } = new List<ModelTask>();
    }

    internal class ModelData
    {
        // ----- Creates all intervals and integer variables -----
        public List<ModelProject> ModelProjects { get; set; } = new List<ModelProject>();

        // это просто альтернативная группировка тасков по ресурсам
        public List<ModelResource> ModelResources { get; set; } = new List<ModelResource>();

        public IntVar makespan;

        public ModelData(ProjectsSet projectSet, CpModel model)
        {
            // Creates all individual interval variables.
            foreach (var project in projectSet.ProjectList)
            {
                var modelProject = new ModelProject();
                modelProject.Project = project;
                ModelProjects.Add(modelProject);
                foreach (var task in project.Tasks)
                {
                    var modelTask = new ModelTask();
                    modelTask.Task = task;
                    modelTask.Start = model.NewIntVar(0, projectSet.horizon, task.ID);
                    modelTask.End = model.NewIntVar(0, projectSet.horizon, task.ID);
                    modelTask.Interval = model.NewIntervalVar(modelTask.Start, task.Duration, modelTask.End, task.ID);
                    modelProject.ModelTasks.Add(modelTask);


                    var modelResource = ModelResources
                        .FirstOrDefault(modelResource => modelResource.ResourceName == task.ResourceName);
                    if (modelResource == null)
                    {
                        modelResource = new ModelResource();
                        modelResource.ResourceName = task.ResourceName;
                        ModelResources.Add(modelResource);
                    }

                    modelResource.Tasks.Add(modelTask);
                }
            }


            // Objective: minimize the makespan (maximum end times of all tasks)
            // of the problem.
            makespan = model.NewIntVar(0, projectSet.horizon, "makespan");
        }
    }
}
