using Google.Protobuf.WellKnownTypes;
using System;

namespace ProjectsScheduler.Core.InputData
{
    public class ProjectsSet
    {
        // horizon is the upper bound of the start time of all tasks.
        public int horizon = 300;

        /*Search time limit in milliseconds. if it's equal to 0,
        then no time limit will be used.*/
        public int timeLimitInSeconds = 3;

        public List<Project> ProjectList { get; set; }
        public List<ProjectResource> Resources { get; set; }

        public ProjectsSet()
        {
        }
    }
}
