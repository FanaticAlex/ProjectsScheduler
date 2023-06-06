using Google.OrTools.Sat;
using ProjectsScheduler;
using ProjectsScheduler.Data;
using ProjectsScheduler.OrToolsSolver;
using System.Text.Json;

class JobshopSat
{
    public static void Main(String[] args)
    {
        var json = File.ReadAllText("Data\\TestProjectsSet.json");
        var projectSet = JsonSerializer.Deserialize<ProjectsSet>(json);

        var solver = new ProjectSchedulerProblemSolver();
        var result = solver.Solve(projectSet);

        var visualizer = new ConsoleVisualizer();
        visualizer.Show(projectSet, result);

        Console.ReadKey();
    }
}