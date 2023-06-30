using Google.OrTools.Sat;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace ProjectsScheduler.Core.OrToolsSolver
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
            
            if (projectSet.timeLimitInSeconds > 0)
            {
                // Set the time limit.
                solver.StringParameters = "max_time_in_seconds:" + projectSet.timeLimitInSeconds;
            }

            // Solve the problem.
            CpSolverStatus status = solver.Solve(model);

            switch(status)
            {
                case CpSolverStatus.Optimal:
                    {
                        var result = GetResult(stopwatch, modelData, solver);
                        result.Status = Status.Optimal;
                        return result;
                    }
                case CpSolverStatus.Infeasible: throw new Exception("При данных условиях задача не решаема.");
                case CpSolverStatus.Feasible:
                    {
                        var result = GetResult(stopwatch, modelData, solver);
                        result.Status = Status.Stopped;
                        return result;
                    }
                case CpSolverStatus.ModelInvalid: throw new Exception("Заданные условия некорректны.");
                case CpSolverStatus.Unknown:
                    {
                        var result = GetResult(stopwatch, modelData, solver);
                        result.Status = Status.Unknown;
                        return result;
                    }
                default: throw new Exception("Статус решения не определен");
            }
        }

        private static Result GetResult(Stopwatch stopwatch, ModelData modelData, CpSolver solver)
        {
            var result = new Result();
            var allModelTasks = modelData.ModelProjects.SelectMany(p => p.ModelTasks);
            foreach (var modelTask in allModelTasks)
            {
                result.TaskIdToTaskStartTime.Add(modelTask.Task.ID, unchecked((int)solver.Value(modelTask.Start)));
            }

            foreach (var modelTask in allModelTasks)
            {
                result.TaskIdToSubtaskNumber.Add(modelTask.Task.ID, unchecked((int)solver.Value(modelTask.SubresourceNumber)));
            }

            result.OverallTime = unchecked((int)solver.ObjectiveValue);
            result.TimeSpent = stopwatch.Elapsed;
            return result;
        }

        private void InitModel(ModelData modelData, CpModel model)
        {
            AddSequentialTasksExecutionCondition(modelData, model);
            AddProjectDeadlineCondition(modelData, model);

            // Ограничения ресурсов
            foreach (var modelResource in modelData.ModelResources)
            {
                AddMaxParalellTaskCondition(model, modelResource);
                AddVacationsCondition(model, modelResource);
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

        /// <summary>
        /// Ограничения на отпуска.
        /// Интервалы отпусков субресурсов не должны пересекаться с задачами
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelResource"></param>
        private void AddVacationsCondition(CpModel model, ModelResource modelResource)
        {
            foreach(var subResource in modelResource.Resource.SubResources)
            {
                foreach(var vacation in subResource.Vacations)
                {
                    var start = model.NewConstant(vacation - 1);
                    var end = model.NewConstant(vacation);

                    foreach (var task in modelResource.Tasks)
                    {
                        var isThisSubresource = model.NewBoolVar(subResource.Name + "bool_vacation_" + vacation);
                        model.Add(task.SubresourceNumber == subResource.SubResourceId).OnlyEnforceIf(isThisSubresource);
                        model.Add(task.SubresourceNumber != subResource.SubResourceId).OnlyEnforceIf(isThisSubresource.Not());

                        AddNotOverlapTasksCondition(start, end, task.Start, task.End, model, isThisSubresource);
                    }
                }
            }
        }

        /// <summary>
        /// Ограничение на последовательное выполнение задач.
        /// - задачи на одном простом ресурсе не прересекаются.
        /// - специальный случай: если у ресурса есть несколько субресурсов, тогда задачи на одном субресурсе не должны пересекаться
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelResource"></param>
        private void AddMaxParalellTaskCondition(CpModel model, ModelResource modelResource)
        {
            int nTasks = modelResource.Tasks.Count();
            for (int i = 0; i < nTasks - 1; i++)
            {
                var i_task = modelResource.Tasks[i];
                for (int j = i + 1; j < nTasks; j++)
                {
                    var j_task = modelResource.Tasks[j];
                    var sameTechnician = model.NewBoolVar($"Job {i_task.Task.ID} and {j_task.Task.ID} have the same technician.");
                    model.Add(i_task.SubresourceNumber == j_task.SubresourceNumber).OnlyEnforceIf(sameTechnician);
                    model.Add(i_task.SubresourceNumber != j_task.SubresourceNumber).OnlyEnforceIf(sameTechnician.Not());
                    AddNotOverlapTasksCondition(i_task.Start, i_task.End, j_task.Start, j_task.End, model, sameTechnician);
                }
            }
        }

        private void AddNotOverlapTasksCondition(IntVar i_start, IntVar i_end, IntVar j_start, IntVar j_end, CpModel model, BoolVar condition)
        {
            var name = string.Join("_", Guid.NewGuid());
            var maxs = model.NewIntVar(0, 10000, "maxs" + name);
            var mine = model.NewIntVar(0, 10000, "mine" + name);
            IntVar[] ss = new IntVar[] { i_start, j_start };
            IntVar[] ee = new IntVar[] { i_end, j_end };
            model.AddMaxEquality(maxs, ss);
            model.AddMinEquality(mine, ee);
            model.Add(maxs >= mine).OnlyEnforceIf(condition);
        }

        /// <summary>
        /// Ограничения на дэдлайны проектов
        /// </summary>
        /// <param name="modelData"></param>
        /// <param name="model"></param>
        private static void AddProjectDeadlineCondition(ModelData modelData, CpModel model)
        {
            foreach (var project in modelData.ModelProjects)
            {
                if (project.Deadline != null)
                    model.Add(project.ModelTasks.Last().End <= project.Deadline);
            }
        }

        /// <summary>
        /// Ограничение на то, что задачи в проектах выполняются только последовательно одна за другой.
        /// время конца первой задачи не может быть больше времени начала следующей задачи
        /// </summary>
        /// <param name="modelData"></param>
        /// <param name="model"></param>
        private static void AddSequentialTasksExecutionCondition(ModelData modelData, CpModel model)
        {
            for (int j = 0; j < modelData.ModelProjects.Count; ++j)
            {
                for (int t = 0; t < modelData.ModelProjects[j].ModelTasks.Count - 1; ++t)
                {
                    var firstTask = modelData.ModelProjects[j].ModelTasks[t];
                    var secondTask = modelData.ModelProjects[j].ModelTasks[t + 1];
                    if (firstTask.Task.ResourceName == secondTask.Task.ResourceName)
                        model.Add(firstTask.Start <= secondTask.Start); // для случая тасков с одинаковыми ресурсами, они могут выполняться одновременно
                    else
                        model.Add(firstTask.End <= secondTask.Start);
                }
            }
        }
    }
}
