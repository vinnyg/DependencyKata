using DependencyKata.Exceptions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace DependencyKata.Tests
{
    [TestClass]
    public class JobRunnerTests
    {
        private JobRunner jobRunner;
        [TestInitialize]
        public void Setup()
        {
            jobRunner = new JobRunner();
        }

        [TestMethod]
        public void Should_ParseJobs_When_JobsAreReceived()
        {
            // Arrange
            var jobList = "a => \n b => \n c =>";

            // Act
            var jobs = jobRunner.ProcessJobs(jobList);

            // Assert
            Assert.AreEqual(3, jobs.Count());
            Assert.IsTrue(jobs.Any(job => job.Id.Equals("a")));
            Assert.IsTrue(jobs.Any(job => job.Id.Equals("b")));
            Assert.IsTrue(jobs.Any(job => job.Id.Equals("c")));
        }

        [TestMethod]
        public void Should_ParseDependencies_When_JobsAreReceived()
        {
            // Arrange
            var jobList = "a => \n b => c \n c =>";

            // Act
            var jobs = jobRunner.ProcessJobs(jobList);

            var jobWithDependency = jobs.Single(job => job.Id.Equals("b"));

            // Assert
            Assert.AreEqual(3, jobs.Count());
            Assert.IsTrue(jobs.Any(job => job.Id.Equals("a")));
            Assert.IsTrue(jobs.Any(job => job.Id.Equals("b")));
            Assert.IsTrue(jobs.Any(job => job.Id.Equals("c")));
            Assert.AreEqual("c", jobWithDependency.Dependency.Id);
        }

        [TestMethod]
        public void Should_RunJobs_When_JobsAreProcessed()
        {
            // Arrange
            var jobList = "a => \n b => \n c =>";
            // If I were a good developer I would replace this with a helper function
            var jobs = jobRunner.ProcessJobs(jobList);

            // Act
            var output = jobRunner.Run(jobs);

            // Assert
            Assert.AreEqual("abc", output);
        }

        [TestMethod]
        public void Should_RunParentJobsBeforeChildJobs()
        {
            // Arrange
            var jobList = "a => \n b => c \n c =>";
            var jobs = jobRunner.ProcessJobs(jobList);

            // Act
            var output = jobRunner.Run(jobs);

            // Assert
            Assert.IsTrue(Helper.AreAllSpecifiedJobsCompletedOnce(output, "abc"));
            Assert.IsTrue(output.IndexOf("c") < output.IndexOf("b"));
        }

        [TestMethod]
        public void Should_RunParentJobsBeforeChildJobs_2()
        {
            // Arrange
            var jobList = "a => \n b => c \n c => f \n d => a \n e => b \n f =>";
            var jobs = jobRunner.ProcessJobs(jobList);

            // Act
            var output = jobRunner.Run(jobs);

            // Assert
            Assert.IsTrue(Helper.AreAllSpecifiedJobsCompletedOnce(output, "abcdef"));
            Assert.IsTrue(output.IndexOf("c") < output.IndexOf("b"));
            Assert.IsTrue(output.IndexOf("f") < output.IndexOf("c"));
            Assert.IsTrue(output.IndexOf("a") < output.IndexOf("d"));
            Assert.IsTrue(output.IndexOf("b") < output.IndexOf("e"));
        }

        [TestMethod]
        public void Should_ThrowInvalidDependencyException_When_JobDependsOnItself()
        {
            // Arrange
            var jobList = "a => \n b => \n c => c";

            // Act
            // Assert
            Assert.ThrowsException<InvalidDependencyException>(() => jobRunner.ProcessJobs(jobList));
        }

        [TestMethod]
        public void Should_ThrowCircularDependencyException_When_ThereIsACircularDependency()
        {
            // Arrange
            var jobList = "a => \n b => c \n c => f \n d => a \n e => \n f => b";

            // Act
            // Assert
            Assert.ThrowsException<CircularDependencyException>(() => jobRunner.ProcessJobs(jobList));
        }
    }
}
