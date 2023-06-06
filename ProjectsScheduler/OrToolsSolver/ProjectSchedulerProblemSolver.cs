using Google.OrTools.Sat;
using ProjectsScheduler.Data;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsScheduler.OrToolsSolver
{
    public class ProjectSchedulerProblemSolver
    {
        public Result Solve(ProjectsSet projectSet)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();

            var model = new CpModel();
            var modelData = new ModelData(projectSet, model);
            InitModel(modelData, model);

            // Create the solver.
            CpSolver solver = new CpSolver();
            // Set the time limit.
            if (projectSet.timeLimitInSeconds > 0)
            {
                solver.StringParameters = "max_time_in_seconds:" + projectSet.timeLimitInSeconds;
            }
            // Solve the problem.
            CpSolverStatus status = solver.Solve(model);

            var result = new Result();
            result.Success = (status == CpSolverStatus.Optimal);

            var allModelTasks = modelData.ModelProjects.SelectMany(p => p.ModelTasks);
            foreach (var modelTask in allModelTasks)
            {
                result.TaskIdToTaskStartTime.Add(modelTask.Task.ID, unchecked((int)solver.Value(modelTask.Start)));
            }

            result.OverallTime = solver.ObjectiveValue;
            result.TimeSpent = stopwatch.Elapsed;
            return result;
        }

        private static void InitModel(ModelData modelData, CpModel model)
        {
            // ----- Creates model -----

            // Creates precedences inside jobs.
            for (int j = 0; j < modelData.ModelProjects.Count; ++j) // для каждого проекта
            {
                for (int t = 0; t < modelData.ModelProjects[j].ModelTasks.Count - 1; ++t) // для каждой задачи в проекте
                {
                    model.Add(modelData.ModelProjects[j].ModelTasks[t].End <= modelData.ModelProjects[j].ModelTasks[t + 1].Start);
                }
            }

            // Adds no_overkap constraints on unary resources.
            foreach (var modelResource in modelData.ModelResources)
            {
                model.AddNoOverlap(modelResource.Tasks.Select(t => t.Interval));
            }

            // Creates array of end_times of jobs.
            IntVar[] allEnds = new IntVar[modelData.ModelProjects.Count];
            for (int i = 0; i < modelData.ModelProjects.Count; i++)
            {
                allEnds[i] = modelData.ModelProjects[i].ModelTasks.Last().End;
            }

            model.AddMaxEquality(modelData.makespan, allEnds);
            model.Minimize(modelData.makespan);
        }
    }
}
