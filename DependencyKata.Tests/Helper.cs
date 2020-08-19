using System;
using System.Linq;

namespace DependencyKata.Tests
{
    public static class Helper
    {
        public static bool AreAllSpecifiedJobsCompletedOnce(string results, string expected)
        {
            try
            {
                foreach (var result in results)
                {
                    expected.Single(job => job.Equals(result));
                }
            }
            catch (InvalidOperationException e)
            {
                return false;
            }

            return true;
        }
    }
}
