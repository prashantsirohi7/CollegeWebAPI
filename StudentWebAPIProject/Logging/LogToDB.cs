namespace StudentWebAPIProject.Logging
{
    public class LogToDB : ILogger
    {
        public void Log(string message)
        {
            Console.WriteLine(message);
            Console.WriteLine("Log to DB");
        }
    }
}
