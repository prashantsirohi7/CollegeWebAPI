namespace StudentWebAPIProject.Logging
{
    public class LogToFile : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Log to File");
        }
    }
}
