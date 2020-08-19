namespace DependencyKata
{
    public class JobItem
    {
        public string Id { get; set; }
        public JobItem Dependency { get; set; }

        public JobItem(string job)
        {
            Id = job;
        }
    }
}
