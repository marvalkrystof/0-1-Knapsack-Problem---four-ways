namespace Logger
{
    public class Logger
    {

        static ILoggingMethod loggingMethod = new ConsoleLogging();
       

        public static void LogCriticalFailure(string message)
        {
            loggingMethod.Log(message);
            Environment.Exit(1);
        }

        public static void Log(string message)
        {
            loggingMethod.Log(message);
        }



    }
}