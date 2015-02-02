Title: Getting the first/last day of the week or month with DateTime in C#
Abstract: Some useful C# utility extension methods for getting the first and last day of the week or month a given date is in.
Published: 2015-02-02 18:12
Updated: 2015-02-02 18:12

Recently I came across the need to find out the date of the first day of a particular week in C#. After browsing [this rather confusing collection of answers](http://stackoverflow.com/questions/38039/how-can-i-get-the-datetime-for-the-start-of-the-week "External Link: Stack Overflow"), I pieced together parts of various code snippets into this little collection of utility methods.

    :::csharp
    public static partial class DateTimeExtensions
    {
        public static DateTime FirstDayOfWeek(this DateTime dt)
        {
            var culture = System.Threading.Thread.CurrentThread.CurrentCulture;
            var diff = dt.DayOfWeek - culture.DateTimeFormat.FirstDayOfWeek;
            if(diff < 0) 
                diff += 7;
            return dt.AddDays(-diff).Date;
        }

        public static DateTime LastDayOfWeek(this DateTime dt)
        {
            return dt.FirstDayOfWeek().AddDays(6);
        }

        public static DateTime FirstDayOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, 1);
        }

        public static DateTime LastDayOfMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1).AddDays(-1);
        }

        public static DateTime FirstDayOfNextMonth(this DateTime dt)
        {
            return dt.FirstDayOfMonth().AddMonths(1);
        }
    }

So now you can easily get month and week boundaries for any given `DateTime`:

    var firstdayOfThisWeek = DateTime.Now.FirstDayOfWeek();

Hopefully someone else will find these useful!