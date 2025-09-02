using System.Diagnostics;

namespace FamiliyApplication.AspireApp.Web.CosmosDb.User
{
    public static class UserExtensions
    {
        public static string GetBirthDayStringNo(this UserDto user)
        {
            var birthDate = user.BirthDate;

            return BuildBirthDayStringNo(birthDate);
        }

        public static string BuildBirthDayStringNo(DateTime? dateTime)
        {
            DateOnly dt;
            if (dateTime == null)
                dt = new DateOnly(1, 1, 1);
            else
                dt = new DateOnly(dateTime.Value.Year, dateTime.Value.Month, dateTime.Value.Day);
            return BuildBirthDayStringNo(dt);
        }

        public static string BuildBirthDayStringNo(DateOnly birthDate)
        {
            if (birthDate == default)
                return "Mangler fødselsdag";

            var today = DateTime.UtcNow.Date;
            var birthdate = new DateTime(birthDate.Year, birthDate.Month, birthDate.Day);

            if (birthdate > today)
                return "Fødselsdag er i fremtiden";

            var years = today.Year - birthdate.Year;
            var months = today.Month - birthdate.Month;
            var days = today.Day - birthdate.Day;

            // Adjust for incomplete year
            if (months < 0)
            {
                years--;
                months += 12;
            }

            // Adjust for incomplete month
            if (days < 0)
            {
                months--;
                var previousMonth = today.AddMonths(-1);
                days += DateTime.DaysInMonth(previousMonth.Year, previousMonth.Month);
            }

            // Build the result string
            var result = "";

            if (years > 0)
                result += $"{years} år";

            if (months > 0)
                result += $"{(string.IsNullOrEmpty(result) ? "" : " og ")}{months} måneder";

            if (days > 0)
                result += $"{(string.IsNullOrEmpty(result) ? "" : " og ")}{days} dager";

            return string.IsNullOrEmpty(result) ? "0 dager." : result + ".";
        }

    }
}
