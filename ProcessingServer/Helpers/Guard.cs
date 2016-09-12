using System;

namespace ProcessingServer.Helpers
{
    static class Guard
    {
        public static void NotNull(object parameter, string parameterName)
        {
            if (parameter == null)
                throw new ArgumentNullException(parameterName);
        }
    }
}
