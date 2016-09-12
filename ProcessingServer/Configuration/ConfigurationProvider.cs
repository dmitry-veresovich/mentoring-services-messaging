using System;
using System.Collections.Generic;

namespace ProcessingServer.Configuration
{
    internal class ConfigurationProvider : IConfigurationProvider
    {
        public string CurrentDirectory => AppDomain.CurrentDomain.BaseDirectory;
        public string InDirectory => "in";
        public string OutDirectory => "out";
        public string FilePattern => "img_*.*";
        public string NumberDelimeter => "_";
        public IEnumerable<string> FileExtensions => new [] { "jpg" };
        public long ObserveInterval => 5000;
        public int MaxAttempts => 3;
    }
}
