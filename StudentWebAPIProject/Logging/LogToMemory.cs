namespace StudentWebAPIProject.Logging
{
    public class LogToMemory : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Log to Memory");
        }
    }
}
