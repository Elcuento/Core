using System;
using UnityEngine;
using System.Collections.Generic;

public class LineSmoother : MonoBehaviour 
{
    public static List<Vector3> SmoothList(List<Vector3> points, int resolution)
    {
        List<Vector3> smoothedPoints = new List<Vector3>();

        // Add extra points at the beginning and end of the list
        Vector3 startPoint = points[0] + (points[0] - points[1]);
        Vector3 endPoint = points[points.Count - 1] + (points[points.Count - 1] - points[points.Count - 2]);
        points.Insert(0, startPoint);
        points.Add(endPoint);

        for (int i = 1; i < points.Count - 2; i++)
        {
            for (int j = 0; j <= resolution; j++)
            {
                float t = (float)j / resolution;
                Vector3 newPoint = GetCatmullRomPoint(t, points[i - 1], points[i], points[i + 1], points[i + 2]);
                smoothedPoints.Add(newPoint);
            }
        }

        return smoothedPoints;
    }

    // Catmull-Rom interpolation formula
    public static Vector3 GetCatmullRomPoint(float t, Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3)
    {
        float tt = t * t;
        float ttt = tt * t;

        Vector3 result =
            0.5f * ((2f * p1) +
                    (-p0 + p2) * t +
                    (2f * p0 - 5f * p1 + 4f * p2 - p3) * tt +
                    (-p0 + 3f * p1 - 3f * p2 + p3) * ttt);

        return result;
    }

}
