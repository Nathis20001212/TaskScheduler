public static class Logger
{
    
    public static void Log(int time, string message)
    {
        Console.WriteLine($"[t={time,4}s] | {message}");
    }

}