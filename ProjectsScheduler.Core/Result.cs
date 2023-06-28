using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProjectsScheduler.Core
{
    public enum Status
    {
        Optimal,
        Stopped,
        Unknown
    }

    /// <summary>
    /// Решение задачи расписаний.
    /// </summary>
    public class Result
    {
        public Status Status { get; set; }

        /// <summary>
        /// Минимальное время необходмое для завершения всех задач всех проектов.
        /// </summary>
        public int OverallTime { get; set; }

        /// <summary>
        /// Время старта каждой задачи [id_задачи;время_начала]
        /// </summary>
        public Dictionary<string, int> TaskIdToTaskStartTime { get; set; } = new Dictionary<string, int>();

        /// <summary>
        /// Время потраченное на решение задачи.
        /// </summary>
        public TimeSpan TimeSpent { get; set; }
    }
}
