namespace Workday;

internal class Program
{
    private static void Main(string[] args)
    {
        Console.WriteLine("Workday Calendar Example");

        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        const string format = "dd-MM-yyyy HH:mm";

        // Example 1
        ProcessExample(calendar, new DateTime(2004, 5, 24, 18, 5, 0), -5.5m, format);

        // Example 2
        ProcessExample(calendar, new DateTime(2004, 5, 24, 19, 3, 0), 44.723656m, format);

        // Example 3
        // This example looks like is incorrect in terms of expected value my calculations are 1 minute off
        ProcessExample(calendar, new DateTime(2004, 5, 24, 18, 3, 0), -6.7470217m, format);

        // Example 4
        ProcessExample(calendar, new DateTime(2004, 5, 24, 8, 3, 0), 12.782709m, format);

        // Example 5
        ProcessExample(calendar, new DateTime(2004, 5, 24, 7, 3, 0), 8.276628m, format);
    }

    private static void ProcessExample(IWorkdayCalendar calendar, DateTime start, decimal increment, string format)
    {
        var result = calendar.GetWorkdayIncrement(start, increment);
        PrintResult(start, increment, result, format);
    }

    private static void PrintResult(DateTime start, decimal increment, DateTime result, string format)
    {
        Console.WriteLine($"{start.ToString(format)} with an addition of {increment} work days is {result.ToString(format)}");
    }
}
