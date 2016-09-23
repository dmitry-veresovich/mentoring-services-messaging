using System;
using System.Collections.Generic;
using System.Linq;
using Common.Models;
using Common.Services;
using PostSharp.Aspects;

namespace Common.PostSharp
{
    [Serializable]
    public class LoggingAspect : OnMethodBoundaryAspect
    {
        private static readonly object sync = new object();
        private readonly IMethodExecutionLogger _methodExecutionLogger = new MethodExecutionLogger();

        public override void OnEntry(MethodExecutionArgs args)
        {
            var info = GetLogInfo(args);

            lock (sync)
            {
                _methodExecutionLogger.Log(info);
            }
        }

        private static MethodExecutionLogInfo GetLogInfo(MethodExecutionArgs args)
        {
            return new MethodExecutionLogInfo
            {
                DateTime = DateTime.Now,
                MethodName = args.Method.Name,
                Parameters = GetParameters(args).ToArray(),
            };
        }

        private static IEnumerable<Parameter> GetParameters(MethodExecutionArgs args)
        {
            var parametersInfo = args.Method.GetParameters();
            foreach (var parameterInfo in parametersInfo)
            {
                yield return new Parameter
                {
                    Name = parameterInfo.Name,
                    Value = args.Arguments.GetArgument(parameterInfo.Position),
                };
            }
        }
    }
}
