using Workday;

namespace WorkdayTests;

public class WorkdayCalendarTest
{
    private readonly string DATE_FORMAT = "dd-MM-yyyy HH:mm";

    [Fact]
    public void TestWorkdayIncrement_NegativeIncrement()
    {
        // Arrange
        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 18, 5, 0);
        const decimal increment = -5.5m;

        // Act
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        // Assert
        var expected = new DateTime(2004, 5, 14, 12, 0, 0);
        Assert.Equal(expected.ToString(DATE_FORMAT), incrementedDate.ToString(DATE_FORMAT));
    }

    [Fact]
    public void TestWorkdayIncrement_PositiveIncrement_LargeValue()
    {
        // Arrange
        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 19, 3, 0);
        const decimal increment = 44.723656m;

        // Act
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        // Assert
        var expected = new DateTime(2004, 7, 27, 13, 47, 0);
        Assert.Equal(expected.ToString(DATE_FORMAT), incrementedDate.ToString(DATE_FORMAT));
    }

    [Fact]
    public void TestWorkdayIncrement_NegativeIncrement_WithFraction()
    {
        // Arrange
        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 18, 3, 0);
        const decimal increment = -6.7470217m;

        // Act
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        // Assert
        var expected = new DateTime(2004, 5, 13, 10, 2, 0);
        Assert.Equal(expected.ToString(DATE_FORMAT), incrementedDate.ToString(DATE_FORMAT));
    }

    [Fact]
    public void TestWorkdayIncrement_PositiveIncrement_StartInWorkingHours()
    {
        // Arrange
        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 8, 3, 0);
        const decimal increment = 12.782709m;

        // Act
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        // Assert
        var expected = new DateTime(2004, 6, 10, 14, 18, 0);
        Assert.Equal(expected.ToString(DATE_FORMAT), incrementedDate.ToString(DATE_FORMAT));
    }

    [Fact]
    public void TestWorkdayIncrement_PositiveIncrement_StartBeforeWorkHours()
    {
        // Arrange
        IWorkdayCalendar calendar = new WorkdayCalendar();
        calendar.SetWorkdayStartAndStop(8, 0, 16, 0);
        calendar.SetRecurringHoliday(5, 17);
        calendar.SetHoliday(new DateTime(2004, 5, 27));

        var start = new DateTime(2004, 5, 24, 7, 3, 0);
        const decimal increment = 8.276628m;

        // Act
        var incrementedDate = calendar.GetWorkdayIncrement(start, increment);

        // Assert
        var expected = new DateTime(2004, 6, 4, 10, 12, 0);
        Assert.Equal(expected.ToString(DATE_FORMAT), incrementedDate.ToString(DATE_FORMAT));
    }
}

