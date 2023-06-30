namespace ProjectsScheduler.Core.InputData
{
    public class SubResource
    {
        public string Name { get; set; }

        public int SubResourceId { get; set; }

        public List<int> Vacations { get; set; } = new List<int> { };
    }
}
