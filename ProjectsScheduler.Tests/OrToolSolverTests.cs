using ProjectsScheduler.Core.InputData;
using ProjectsScheduler.Core.OrToolsSolver;
using System.Text.Json;

namespace ProjectsScheduler.Tests
{
    public class OrToolSolverTests
    {
        [Fact]
        public void Test_Example_simple()
        {
            var data = File.ReadAllText("Example_simple.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(12, result.OverallTime);
        }

        [Fact]
        public void Test_Example_simple_palallel()
        {
            var data = File.ReadAllText("Example_simple_parallel.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(6, result.OverallTime);
        }

        [Fact]
        public void Test_Example_simple_vacations()
        {
            var data = File.ReadAllText("Example_simple_vacations.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(15, result.OverallTime);
        }

        /// <summary>
        /// ≈сли таски одного проекта на одном и том же ресурсе идут подр€д,
        /// то их можно выполн€ть паралельно на этот ресурс.
        /// </summary>
        [Fact]
        public void Test_Example_simple_dividedTask()
        {
            var data = File.ReadAllText("Example_simple_dividedTask.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(1, result.OverallTime);
        }

        /// <summary>
        /// ” каждой задачи может быть дэдлайн - точка вермени до которой должна быть выполнена задача.
        /// </summary>
        [Fact]
        public void Test_Example_simple_Deadlines()
        {
            var data = File.ReadAllText("Example_simple_deadlines.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            var project = projectSet.ProjectList.Single(p => p.Name == "Project 4");
            var task = project.Tasks.First();

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(12, result.OverallTime);
            Assert.True(result.TaskIdToTaskStartTime[task.ID] <= project.Deadline);
        }

        [Fact]
        public void Test_Example1()
        {
            var data = File.ReadAllText("Example1.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(5, result.OverallTime);
        }

        [Fact]
        public void Test_Example2()
        {
            var data = File.ReadAllText("Example2.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(6, result.OverallTime);
        }

        [Fact]
        public void Test_Example3()
        {
            var data = File.ReadAllText("Example3.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(6, result.OverallTime);
        }

        [Fact]
        public void Test_Example5()
        {
            var data = File.ReadAllText("Example5.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(15, result.OverallTime);
        }

        [Fact]
        public void Test_Example6()
        {
            var data = File.ReadAllText("Example6.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Status == Core.Status.Optimal);
            Assert.Equal(6, result.OverallTime);
        }
    }
}