namespace Workday;

public class WorkdayCalendar : IWorkdayCalendar
{
    private TimeSpan workdayStart;
    private TimeSpan workdayEnd;
    private readonly HashSet<DateTime> holidays = [];
    private readonly List<(int Month, int Day)> recurringHolidays = [];

    public void SetHoliday(DateTime date) => holidays.Add(date.Date);

    public void SetRecurringHoliday(int month, int day)
    {
        if (month is < 1 or > 12)
            throw new ArgumentOutOfRangeException(nameof(month), "Month must be between 1 and 12");

        var daysInMonth = month switch
        {
            2 => 29,
            4 or 6 or 9 or 11 => 30,
            _ => 31
        };
        if (day < 1 || day > daysInMonth)
            throw new ArgumentOutOfRangeException(nameof(day), $"Day must be between 1 and {daysInMonth} for month {month}");

        recurringHolidays.Add((month, day));
    }

    public void SetWorkdayStartAndStop(int startHours, int startMinutes, int stopHours, int stopMinutes)
    {
        if (startHours < 0 ||
            startHours > 23 ||
            stopHours < 0 ||
            stopHours > 23 ||
            startMinutes < 0 ||
            startMinutes > 59 ||
            stopMinutes < 0 ||
            stopMinutes > 59)
            throw new ArgumentException("Invalid hours or minutes.");

        workdayStart = new TimeSpan(startHours, startMinutes, 0);
        workdayEnd = new TimeSpan(stopHours, stopMinutes, 0);
        if (workdayEnd <= workdayStart)
            throw new ArgumentException("Workday end time must be after workday start time");
    }

    private bool IsHoliday(DateTime date) =>
        holidays.Contains(date.Date) ||
        recurringHolidays.Any(r => date.Month == r.Month && date.Day == r.Day);

    private bool IsWorkDay(DateTime date) =>
        date.DayOfWeek is not DayOfWeek.Saturday &&
        date.DayOfWeek is not DayOfWeek.Sunday &&
        !IsHoliday(date);

    private DateTime GetNextWorkDay(DateTime date)
    {
        do { date = date.AddDays(1).Date; }
        while (!IsWorkDay(date));
        return date.Add(workdayStart);
    }

    private DateTime GetPreviousWorkDay(DateTime date)
    {
        do { date = date.AddDays(-1).Date; }
        while (!IsWorkDay(date));
        return date.Add(workdayEnd);
    }

    private DateTime NormalizeStart(DateTime startTime, bool forward)
    {
        if (!IsWorkDay(startTime.Date))
            return forward ? GetNextWorkDay(startTime) : GetPreviousWorkDay(startTime);

        var time = startTime.TimeOfDay;
        if (forward)
        {
            if (time < workdayStart) return startTime.Date.Add(workdayStart);
            if (time >= workdayEnd) return GetNextWorkDay(startTime);
        }
        else
        {
            if (time >= workdayEnd) return startTime.Date.Add(workdayEnd);
            if (time < workdayStart) return GetPreviousWorkDay(startTime);
        }
        return startTime;
    }

    // Adds or subtracts work minutes by moving through work days.
    private DateTime AddWorkMinutes(DateTime startTime, double minutes, bool forward)
    {
        while (minutes > 0)
        {
            if (forward)
            {
                var available = (workdayEnd - startTime.TimeOfDay).TotalMinutes;
                if (available >= minutes)
                    return startTime.AddMinutes(minutes);
                minutes -= available;
                startTime = GetNextWorkDay(startTime);
            }
            else
            {
                var available = (startTime.TimeOfDay - workdayStart).TotalMinutes;
                if (available >= minutes)
                    return startTime.AddMinutes(-minutes);
                minutes -= available;
                startTime = GetPreviousWorkDay(startTime);
            }
        }
        return startTime;
    }

    public DateTime GetWorkdayIncrement(DateTime startDate, decimal incrementInWorkdays)
    {
        var workdayMinutes = (workdayEnd - workdayStart).TotalMinutes;
        var totalMinutes = (double)Math.Abs(incrementInWorkdays) * workdayMinutes;
        var forward = incrementInWorkdays >= 0;
        var normalizeStart = NormalizeStart(startDate, forward);

        return AddWorkMinutes(normalizeStart, totalMinutes, forward);
    }
}
