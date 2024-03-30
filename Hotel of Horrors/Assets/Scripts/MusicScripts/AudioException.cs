using System;

public class AudioError : Exception
{
    public AudioError() : base() { }
    public AudioError(string message) : base(message) { }
    public AudioError(string message, Exception inner) : base(message, inner) { }
}
