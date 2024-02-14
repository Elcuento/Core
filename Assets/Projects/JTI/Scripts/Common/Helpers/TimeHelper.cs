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

    public static string FormatTimeLeftLabelAndValue(long timeLeft, bool days = true, bool hours = true, bool minutes = true, bool seconds = true, string[] labels = null)
    {
        if (timeLeft <= 0)
            return "";

        if (labels == null) labels = new[] { "d", "h", "m", "s" };

        var date = new DateTime();
        date = date.AddSeconds(timeLeft);

        string GetLabel(int n) => labels != null && labels.Length > n ? labels[n] : "";

        var dayStr = days ? ((date.Day - 1) + GetLabel(0)) : "";
        var horStr = hours ? (date.Hour + GetLabel(1)) : "";
        var minStr = minutes ? (date.Minute + GetLabel(2)) : "";
        var minSec = seconds ? (date.Second + GetLabel(3)) : "";

        var final = "";

        if (days && date.Day > 0)
        {
            final += dayStr;
        }
        if (hours)
        {
            final += (final.Length > 0 ? " " : "") + horStr;
        }

        if (minutes)
        {
            final += (final.Length > 0 ? " " : "") + minStr;
        }
        if (seconds)
        {
            final += (final.Length > 0 ? " " : "") + minSec;
        }
        return final;
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
}
