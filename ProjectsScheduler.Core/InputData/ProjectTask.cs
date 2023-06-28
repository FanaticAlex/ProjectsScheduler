namespace ProjectsScheduler.Core.InputData
{
    public class ProjectTask
    {
        public ProjectTask(int duration, string resourceName)
        {
            Duration = duration;
            ResourceName = resourceName;
            ID = Guid.NewGuid().ToString();
        }
        public int Duration { get; set; }
        public string ResourceName { get; set; }
        public string ID { get; }
        public bool IsSplittable { get; set; }
    }
}
