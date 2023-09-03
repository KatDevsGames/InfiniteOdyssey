using System;

namespace InfiniteOdyssey.Extensions;

public interface IRewindable
{
    int Version { get; }
    void Rewind() => RewindTo(Version - 1);
    void RewindTo(int version);

    public class RewindException : Exception
    {
        public RewindException() { }
        public RewindException(string message) : base(message) { }
    }
}