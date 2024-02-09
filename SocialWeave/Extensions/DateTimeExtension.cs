using System.Globalization;

namespace SocialWeave.Extensions
{
    /// <summary>
    /// Provides extension methods for the <see cref="DateTime"/> class.
    /// </summary>
    public static class DateTimeExtension
    {
        /// <summary>
        /// Returns a string representation of the duration between the current moment and the specified <see cref="DateTime"/>.
        /// </summary>
        /// <param name="thisDateTime">The <see cref="DateTime"/> instance.</param>
        /// <returns>A string representing the duration between the current moment and the specified <see cref="DateTime"/>.</returns>
        public static string Duration(this DateTime thisDateTime)
        {
            if (thisDateTime == null || !(thisDateTime is DateTime))
            {
                return "0 min";
            }

            TimeSpan duration = DateTime.Now.Subtract(thisDateTime);

            if (duration.TotalMinutes < 60)
            {
                return duration.TotalMinutes.ToString("F1", CultureInfo.InvariantCulture) + " min";
            }
            else if (duration.TotalHours < 24)
            {
                return duration.TotalHours.ToString("F1", CultureInfo.InvariantCulture) + " h";
            }
            else
            {
                DateTime nextMonth = thisDateTime.AddMonths(1);
                TimeSpan daysInMonth = nextMonth.AddDays(-nextMonth.Day).Subtract(thisDateTime.AddDays(-thisDateTime.Day));

                if (duration.TotalDays < daysInMonth.TotalDays)
                {
                    return ((int)duration.TotalDays).ToString("F1", CultureInfo.InvariantCulture) + " d";
                }
                else
                {
                    return thisDateTime.Year.ToString("F1", CultureInfo.InvariantCulture) + " year";
                }
            }
        }
    }
}
