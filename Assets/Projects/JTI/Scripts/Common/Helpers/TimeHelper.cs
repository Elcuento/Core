using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeHelper : MonoBehaviour
{
    public static long GetMidnightTimeLeft()
    {
        var ts = DateTime.Today.AddDays(1).Subtract(DateTime.UtcNow);
        var secondsToMidnight = (int)ts.TotalSeconds;

        return secondsToMidnight;
    }

    public static string FormatTimeLeftNumberDHMS(long timeLeft, bool showHour = true, bool showDays = true, bool showMinutes = false, bool showSecond = false) // d/h
    // 00:00:00:00
    {
        var str = "";

        if (timeLeft <= 0)
        {
            str = "0";

            if (showHour)
            {
                str = $"0:" + str;
            }

            if (showDays && showHour)
            {
                str = $"0:" + str;
            }

            return str;
        }

        var date = new DateTime();
        date = date.AddSeconds(timeLeft);

        if (showMinutes)
        {
            str = $"{date.Minute:D2}";
        }

        if (showSecond)
        {
            str += $":{date.Second:D2}";
        }

        if (showHour)
        {
            str = $"{date.Hour:D2}" + (showMinutes ? ":" : "") + str;
        }

        if (showDays && showHour)
        {
            str = $"{date.Day - 1:D2}:" + str;
        }

        return str;
    }

    public static string FormatTimeLeft(long timeLeft, bool detailed, bool days = true, bool hours = true, bool minutes = true)
    {
        if (timeLeft <= 0)
            return "";

        var date = new DateTime();
        date = date.AddSeconds(timeLeft);

        if (date.Day > 1)
        {
            var day = detailed && days ? "d" : "";
            var hour = detailed && hours ? "h" : "";
            var minute = detailed && minutes ? "m" : "";

            if (minutes)
            {
                return string.Format($"{date.Day - 1}{day} {date.Hour}{hour} {date.Minute}{minute}");
            }
            else
            {
                return string.Format($"{date.Day - 1}{day} {date.Hour}{hour}");
            }

        }
        else
        {
            var minute = detailed && minutes ? "m" : "";
            var hour = detailed && hours ? "h" : "";

            if (minutes)
            {
                return string.Format($"{date.Hour}{hour} {date.Minute}{minute}");
            }
            else
            {
                return string.Format($"{date.Hour}{hour}");
            }

        }
    }

    public static string FormatTimeLeftNumber(long timeLeft, bool showHour = true, bool showDays = true)
    {
        var str = "";

        if (timeLeft <= 0)
        {
            str = "00:00";

            if (showHour)
            {
                str = $"00:" + str;
            }

            if (showDays && showHour)
            {
                str = $"00:" + str;
            }

            return str;
        }

        var date = new DateTime();
        date = date.AddSeconds(timeLeft);

        str = $"{date.Minute:D2}:{date.Second:D2}";

        if (showHour)
        {
            str = $"{date.Hour:D2}:" + str;
        }

        if (showDays && showHour)
        {
            str = $"{date.Day - 1:D2}:" + str;
        }

        return str;
    }
}
