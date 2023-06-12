using ProjectsScheduler.Data;
using ProjectsScheduler.OrToolsSolver;
using System.Text.Json;

namespace ProjectsScheduler.Tests
{
    public class OrToolSolverTests
    {
        [Fact]
        public void Test_Example1()
        {
            var data = File.ReadAllText("Example1.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Success);
            Assert.Equal(5, result.OverallTime);
        }

        [Fact]
        public void Test_Example2()
        {
            var data = File.ReadAllText("Example2.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Success);
            Assert.Equal(5, result.OverallTime);
        }

        [Fact]
        public void Test_Example5()
        {
            var data = File.ReadAllText("Example5.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Success);
            Assert.Equal(15, result.OverallTime);
        }

        [Fact]
        public void Test_Example6()
        {
            var data = File.ReadAllText("Example6.json");
            var projectSet = JsonSerializer.Deserialize<ProjectsSet>(data);
            var solver = new ProjectSchedulerProblemSolver();
            var result = solver.Solve(projectSet);

            Assert.True(result.Success);
            Assert.Equal(6, result.OverallTime);
        }
    }
}