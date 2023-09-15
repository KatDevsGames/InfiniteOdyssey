using System;

namespace InfiniteOdyssey.Randomization;

public class GenerationException : Exception
{
    public GenerationException() { }

    public GenerationException(string message) : base(message) { }
}