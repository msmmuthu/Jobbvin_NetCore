namespace Jobbvin.Client
{
    public static class DateFormats
    {
        public static string ToJobbvinDate(this DateTime date)
        {
            return date.ToString("dd/MM/yyyy");
        }
    }
}
