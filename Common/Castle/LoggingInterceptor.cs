using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;
using Common.Models;
using Common.Services;

namespace Common.Castle
{
    public class LoggingInterceptor : IInterceptor
    {
        private static readonly object sync = new object();
        private readonly IMethodExecutionLogger _methodExecutionLogger;

        public LoggingInterceptor(IMethodExecutionLogger methodExecutionLogger)
        {
            _methodExecutionLogger = methodExecutionLogger;
        }

        public void Intercept(IInvocation invocation)
        {
            var info = GetLogInfo(invocation);

            lock (sync)
            {
                _methodExecutionLogger.Log(info);
            }

            invocation.Proceed();
        }

        private static MethodExecutionLogInfo GetLogInfo(IInvocation invocation)
        {
            return new MethodExecutionLogInfo
            {
                DateTime = DateTime.Now,
                MethodName = invocation.Method.Name,
                Parameters = GetParameters(invocation).ToArray(),
            };
        }

        private static IEnumerable<Parameter> GetParameters(IInvocation invocation)
        {
            var parametersInfo = invocation.Method.GetParameters();
            foreach (var parameterInfo in parametersInfo)
            {
                yield return new Parameter
                {
                    Name = parameterInfo.Name,
                    Value = invocation.GetArgumentValue(parameterInfo.Position),
                };
            }
        }
    }
}
