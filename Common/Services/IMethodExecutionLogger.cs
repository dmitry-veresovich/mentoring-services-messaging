using Common.Models;

namespace Common.Services
{
    public interface IMethodExecutionLogger
    {
        void Log(MethodExecutionLogInfo logInfo);
    }
}
