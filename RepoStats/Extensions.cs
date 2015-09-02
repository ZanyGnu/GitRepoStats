
namespace RepoStats
{
    using System;
    using System.Collections.Generic;

    static public class Extensions
    {
        public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval, MidpointRounding roundingType)
        {
            return new TimeSpan(
                Convert.ToInt64(Math.Round(
                    time.Ticks / (decimal)roundingInterval.Ticks,
                    roundingType
                )) * roundingInterval.Ticks
            );
        }

        public static TimeSpan Round(this TimeSpan time, TimeSpan roundingInterval)
        {
            return Round(time, roundingInterval, MidpointRounding.ToEven);
        }

        public static DateTime Round(this DateTime datetime, TimeSpan roundingInterval)
        {
            return new DateTime((datetime - DateTime.MinValue).Round(roundingInterval).Ticks);
        }

        public static bool IsWithin(this DateTimeOffset obj, DateTime startDate, DateTime endDate)
        {
            return startDate <= obj && obj <= endDate;
        }

        public static string EscapeForFormat(this string obj)
        {
            return obj.Replace("{0}", "%0%").Replace("{1}", "%1%").Replace("{", "{{").Replace("}", "}}").Replace("%0%", "{0}").Replace("%1%", "{1}");
        }

        public static bool AreSimilar(this string obj1, string obj2)
        {
            string[] s1s = obj1.Split(' ', '/', '\\', '\r', '\n', '.');
            string[] s2s = obj2.Split(' ', '/', '\\', '\r', '\n', '.');
            HashSet<string> s1Set = new HashSet<string>(s1s);
            HashSet<string> s2Set = new HashSet<string>(s2s);
            int count1 = s1Set.Count;
            s1Set.IntersectWith(s2s);
            int count2 = s1Set.Count;
            double similarityIndex = count2 * 100.0 / count1;
            return similarityIndex > 90;
        }
    }
}
