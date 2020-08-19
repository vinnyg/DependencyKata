using System;

namespace DependencyKata.Exceptions
{
    public class CircularDependencyException : Exception
    {
        public CircularDependencyException() { }
    }
}
