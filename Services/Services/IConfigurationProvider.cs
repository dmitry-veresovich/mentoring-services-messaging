using System.Collections.Generic;

namespace Services.Services
{
    public interface IConfigurationProvider
    {
        string CurrentDirectory { get; }
        string InDirectory { get; }
        string OutDirectory { get; }
        string FilePattern { get; }
        string NumberDelimeter { get; }
        IEnumerable<string> FileExtensions { get; }
        long ObserveInterval { get; }
        int MaxAttempts { get; }
    }
}
