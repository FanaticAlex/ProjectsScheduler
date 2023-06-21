using System;

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
    }

    public class ProjectTask
    {
        public ProjectTask(int duration, string resourceName)
        {
            Duration = duration;
            ResourceName = resourceName;
            ID = Guid.NewGuid().ToString();
        }
        public string ResourceName { get; set; }
        public int Duration { get; set; }
        public string ID { get; }
    }

    public class Resource
    {
        public string Name { get; set; }
        public int MaxParallelTasks { get; set; } = 1;
        public List<int> Vacations { get; set; } = new List<int> { };
    }

    public class ProjectsSet
    {
        // horizon is the upper bound of the start time of all tasks.
        public int horizon = 300;

        /*Search time limit in milliseconds. if it's equal to 0,
        then no time limit will be used.*/
        public int timeLimitInSeconds = 0;

        public List<Project> ProjectList { get; set; }
        public List<Resource> Resources { get; set; }

        public ProjectsSet()
        {
        }
    }
}
