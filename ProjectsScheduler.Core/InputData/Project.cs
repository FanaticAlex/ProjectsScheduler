namespace ProjectsScheduler.Core.InputData
{
    /// <summary>
    /// Проект, состоящий из тасков
    /// </summary>
    public class Project
    {
        public Project(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
        public List<ProjectTask> Tasks { get; set; } = new List<ProjectTask>();

        private int? _deadline;
        public int? Deadline
        {
            get { return _deadline; }
            set
            {
                if (value < Tasks.Select(t => t.Duration).Sum())
                    throw new Exception("Дэдлайн не может быть меньше длительности задачи.");

                _deadline = value;
            }
        }
    }
}
