using System;
using System.IO;
using NLog;
using NLog.Config;
using NLog.Targets;

namespace ProcessingServer
{
    static class NLogFactory
    {
        public static LogFactory CreateAndConfigure()
        {
            var currentDir = AppDomain.CurrentDomain.BaseDirectory;
            var conf = new LoggingConfiguration();
            var fileTarget = new FileTarget
            {
                Name = "Default",
                FileName = Path.Combine(currentDir, "log.txt"),
                Layout = "${date} ${message} ${onexception:inner=${exception:format=toString}}"
            };
            conf.AddTarget(fileTarget);
            conf.AddRuleForAllLevels(fileTarget);

            var logFactory = new LogFactory(conf);
            return logFactory;
        }
    }
}
