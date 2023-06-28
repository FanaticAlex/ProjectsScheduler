namespace ProjectsScheduler.Core.InputData
{
    public class ProjectResource
    {
        public string Name { get; set; }
        public int MaxParallelTasks { get; set; } = 1;
        public List<int> Vacations { get; set; } = new List<int> { };
    }
}
