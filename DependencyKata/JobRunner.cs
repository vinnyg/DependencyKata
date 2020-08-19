using DependencyKata.Exceptions;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace DependencyKata
{
    public class JobRunner
    {
        public IEnumerable<JobItem> ProcessJobs(string jobList)
        {
            var jobPairs = jobList.Split('\n');
            var jobs = jobPairs.Select(pair => new JobItem(Regex.Split(pair, @"=>")[0].Trim()));

            var jobDictionary = jobs.ToDictionary(job => job.Id);

            foreach(var pair in jobPairs)
            {
                var jobDependencyPair = Regex.Split(pair, @"=>");
                if (!string.IsNullOrEmpty(jobDependencyPair[1].Trim()))
                {
                    jobDictionary[jobDependencyPair[0].Trim()].Dependency =
                        jobDictionary.GetValueOrDefault(jobDependencyPair[1].Trim());
                }
            }

            var result = jobDictionary.Select(pair => pair.Value);
            Validate(result);

            return result;
        }

        public string Run(IEnumerable<JobItem> jobs)
        {
            var executedJobIds = new List<string>();

            foreach (var job in jobs)
            {
                ExecuteJob(job, executedJobIds);
            }

            var output = string.Concat(executedJobIds);

            return output;
        }

        private void ExecuteJob(JobItem job, List<string> executedJobIds)
        {

            if (!executedJobIds.Contains(job.Id))
            {
                if (job.Dependency != null)
                {
                    ExecuteJob(job.Dependency, executedJobIds);
                }

                executedJobIds.Add(job.Id);
            }
        }

        private void Validate(IEnumerable<JobItem> jobList)
        {
            if (jobList.Any(job => job.Id.Equals(job.Dependency?.Id)))
            {
                throw new InvalidDependencyException();
            }

            var dependencyDictionary = new Dictionary<string, JobItem>();

            // Use a list to track job items in chains that have been traversed
            // so that we don't have to traverse those chains again
            foreach (var job in jobList)
            {
                var dependencyList = new List<JobItem>();
                CheckCircularDependency(job, dependencyList);
            }
        }

        private void CheckCircularDependency(JobItem job, List<JobItem> dependencyList)
        {
            dependencyList.Add(job);

            if (dependencyList.Count() > dependencyList.Distinct().Count())
            {
                throw new CircularDependencyException();
            }

            if (job.Dependency != null)
            {
                CheckCircularDependency(job.Dependency, dependencyList);
            }

        }
    }
}
