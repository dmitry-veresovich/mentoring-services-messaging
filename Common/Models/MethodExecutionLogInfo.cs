using System;

namespace Common.Models
{
    public class MethodExecutionLogInfo
    {
        public DateTime DateTime { get; set; }
        public string MethodName { get; set; }
        public Parameter[] Parameters { get; set; }
    }
}
