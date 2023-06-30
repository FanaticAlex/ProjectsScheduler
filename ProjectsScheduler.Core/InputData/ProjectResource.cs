namespace ProjectsScheduler.Core.InputData
{
    public class ProjectResource
    {
        public string Name { get; set; }
        public int? MaxParallelTasks
        {
            get { return null; }
            set
            {
                if (SubResources.Count == 0)
                {
                    for (int i = 0; i < value; i++)
                    {
                        var newSubResource = new SubResource();
                        newSubResource.Name = $"SubResourceName_{i}";
                        newSubResource.SubResourceId = i;
                        SubResources.Add(newSubResource);
                    }
                }
            }
        }

        public List<SubResource> SubResources { get; set; } = new List<SubResource>();
    }
}
