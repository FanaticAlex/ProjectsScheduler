using ProjectsScheduler.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsScheduler
{
    internal class ConsoleVisualizer
    {
        public void Show(ProjectsSet projectSet, Result result)
        {
            if (!result.Success)
            {
                Console.WriteLine("No solution found!");
                return;
            }


            var resourceToColors = GetResourceToColor(projectSet.GetAllResources());
            if (resourceToColors == null)
                return;

            var marginLeft = projectSet.ProjectList
                .SelectMany(p => p.Tasks)
                .Select(t => $"{t.Duration + t.ResourceName}".Length)
                .Max() + 10;

            foreach (var project in projectSet.ProjectList)
            {
                Console.WriteLine($"{project.Name}:");
                foreach (var task in project.Tasks)
                {
                    // task
                    var taskLine = $" Task {task.Duration + task.ResourceName}";
                    var line = taskLine + new string(' ', marginLeft - taskLine.Length) + "|";
                    Console.Write(line);

                    // value
                    Console.Write(new string(' ', result.TaskIdToTaskStartTime[task.ID]));
                    WriteWithColor(resourceToColors[task.ResourceName], new string('*', task.Duration));

                    Console.WriteLine();
                }
            }

            Console.WriteLine(new string('-', 100));

            var label = "Timeline->" ;
            Console.Write(label + new string(' ', marginLeft - label.Length) + "|");
            Console.WriteLine("0        10        20        30        40        50        60        70        80");

            Console.Write(new string(' ', marginLeft + unchecked((int)(result.OverallTime))));
            Console.Write(" <-SolveTime = " + result.OverallTime);

            Console.WriteLine();
            foreach (var item in resourceToColors)
            {
                WriteWithColor(item.Value, item.Key);
                Console.WriteLine();
            }

            Console.WriteLine($"Time spent: {result.TimeSpent}");
        }

        private static void WriteWithColor(ConsoleColor color, string line)
        {
            Console.ForegroundColor = color;
            Console.Write(line);
            Console.ForegroundColor = ConsoleColor.Gray;
        }

        private static Dictionary<string, ConsoleColor> GetResourceToColor(List<string> resources)
        {
            var colors = new List<ConsoleColor>()
        {
            ConsoleColor.Red,
            ConsoleColor.Green,
            ConsoleColor.Blue,
            ConsoleColor.Yellow,
            ConsoleColor.Magenta,
            ConsoleColor.Cyan,
            ConsoleColor.DarkRed,
            ConsoleColor.DarkGreen,
            ConsoleColor.DarkBlue,
            ConsoleColor.DarkYellow,
            ConsoleColor.DarkMagenta,
            ConsoleColor.DarkCyan,
        };

            if (resources.Count > colors.Count)
            {
                Console.WriteLine("Too many resource");
                return null;
            }

            var result = new Dictionary<string, ConsoleColor>();
            for (var i = 0; i < resources.Count; i++)
            {
                result.Add(resources[i], colors[i]);
            }

            return result;
        }
    }
}
