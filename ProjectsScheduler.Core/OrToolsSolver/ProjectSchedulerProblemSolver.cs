using Google.OrTools.Sat;
using ProjectsScheduler.Core.InputData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

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
            // Set the time limit.
            if (projectSet.timeLimitInSeconds > 0)
            {
                solver.StringParameters = "max_time_in_seconds:" + projectSet.timeLimitInSeconds;
            }
            // Solve the problem.
            CpSolverStatus status = solver.Solve(model);

            switch(status)
            {
                case CpSolverStatus.Optimal:
                    {
                        var result = new Result();
                        result.Status = Status.Optimal;
                        var allModelTasks = modelData.ModelProjects.SelectMany(p => p.ModelTasks);
                        foreach (var modelTask in allModelTasks)
                        {
                            result.TaskIdToTaskStartTime.Add(modelTask.Task.ID, unchecked((int)solver.Value(modelTask.Start)));
                        }

                        result.OverallTime = unchecked((int)solver.ObjectiveValue);
                        result.TimeSpent = stopwatch.Elapsed;
                        return result;
                    }
                case CpSolverStatus.Infeasible: throw new Exception("При данных условиях задача не решаема.");
                case CpSolverStatus.Feasible:
                    {
                        var result = new Result();
                        result.Status = Status.Stopped;
                        var allModelTasks = modelData.ModelProjects.SelectMany(p => p.ModelTasks);
                        foreach (var modelTask in allModelTasks)
                        {
                            result.TaskIdToTaskStartTime.Add(modelTask.Task.ID, unchecked((int)solver.Value(modelTask.Start)));
                        }

                        result.OverallTime = unchecked((int)solver.ObjectiveValue);
                        result.TimeSpent = stopwatch.Elapsed;
                        return result;


                        //throw new Exception("Процесс решения был остановлен.");
                    }
                case CpSolverStatus.ModelInvalid: throw new Exception("Заданные условия некорректны.");
                case CpSolverStatus.Unknown:
                    {
                        var result = new Result();
                        result.Status = Status.Unknown;
                        var allModelTasks = modelData.ModelProjects.SelectMany(p => p.ModelTasks);
                        foreach (var modelTask in allModelTasks)
                        {
                            result.TaskIdToTaskStartTime.Add(modelTask.Task.ID, unchecked((int)solver.Value(modelTask.Start)));
                        }

                        result.OverallTime = unchecked((int)solver.ObjectiveValue);
                        result.TimeSpent = stopwatch.Elapsed;
                        return result;

                        //throw new Exception("Unknown error.");
                    }
                default: throw new Exception("Статус решения не определен");
            }
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

        private static void AddVacationsCondition(CpModel model, ModelResource modelResource)
        {
            // ограничения на отпуска
            // один слот ресурса убираем 
            // ограничиваем пересечение m интервалов в даты отпусков
            var tasks = modelResource.Tasks;
            foreach (var t0 in modelResource.Resource.Vacations)
            {
                // t0 - отпуск

                //          t0
                // s1______e1|
                // s2______e2|

                // t0        
                // |s1_______e1
                // |s2_______e2

                // s1_____t0__e1
                //    s2__t0______e2

                // s2(maxs)   e1(mine)
                // t0 > min(e) || t0 < max(s)
                var combinations1 = GetCombinations(tasks.Count, modelResource.Resource.MaxParallelTasks);
                foreach (var combination in combinations1)
                {
                    var name = string.Join("_", combination);
                    var maxs = model.NewIntVar(0, 10000, "maxs_" + name);
                    var mine = model.NewIntVar(0, 10000, "mine_" + name);
                    IntVar[] ss = combination.Select(i => tasks[i].Start).ToArray();
                    IntVar[] ee = combination.Select(i => tasks[i].End).ToArray();
                    model.AddMaxEquality(maxs, ss);
                    model.AddMinEquality(mine, ee);

                    var con1 = model.NewBoolVar("con1_" + name);
                    model.Add(t0 >= mine).OnlyEnforceIf(con1);
                    model.Add(t0 < mine).OnlyEnforceIf(con1.Not());

                    var con2 = model.NewBoolVar("con2_" + name);
                    model.Add(t0 + 1 <= maxs).OnlyEnforceIf(con2);
                    model.Add(t0 + 1 > maxs).OnlyEnforceIf(con2.Not());

                    model.AddBoolOr(new List<ILiteral>() { con1, con2 });
                }
            }
        }

        /// <summary>
        /// многозадачные ресурсы могут выполнять m задач одновременно
        /// тогда ограничение будет: любые m+1 интервалов тасков не могут пересекаться
        /// </summary>
        /// <param name="model"></param>
        /// <param name="modelResource"></param>
        private static void AddMaxParalellTaskCondition(CpModel model, ModelResource modelResource)
        {
            var tasks = modelResource.Tasks;
            var combinations = GetCombinations(tasks.Count, modelResource.Resource.MaxParallelTasks + 1);
            foreach (var combination in combinations)
            {
                var name = string.Join("_", combination);
                var maxs = model.NewIntVar(0, 10000, "maxs" + name);
                var mine = model.NewIntVar(0, 10000, "mine" + name);
                IntVar[] ss = combination.Select(i => tasks[i].Start).ToArray();
                IntVar[] ee = combination.Select(i => tasks[i].End).ToArray();
                model.AddMaxEquality(maxs, ss);
                model.AddMinEquality(mine, ee);
                model.Add(maxs >= mine);
            }
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

        private static IEnumerable<int[]> GetCombinations(int tasksCount, int maxParallel)
        {
            int[] result = new int[maxParallel];
            Stack<int> stack = new Stack<int>(maxParallel);
            stack.Push(0);
            while (stack.Count > 0)
            {
                int index = stack.Count - 1;
                int value = stack.Pop();
                while (value < tasksCount)
                {
                    result[index++] = value++;
                    stack.Push(value);
                    if (index != maxParallel) continue;
                    yield return (int[])result.Clone(); // thanks to @xanatos
                                                        //yield return result;
                    break;
                }
            }
        }
    }
}
