using System;
using System.Collections.Generic;
using UnityEngine;

public enum HexagonType
{
    Grid,
    Visual
}

public static class GridSettings
{
    public const float OUTER_RADIUS = 1.02f;
    public const float VISUAL_OUTER_RADIUS = 1f;
    public const float INNER_RADIUS = OUTER_RADIUS * 0.866025404f;
    public const float VISUAL_INNER_RADIUS = VISUAL_OUTER_RADIUS * 0.866025404f;

    public static Vector3 GetFirstCorner (HexagonDirection direction, HexagonType type) 
    {
        return HexagonPoints(type)[(int)direction];
    }

    public static Vector3 GetSecondCorner (HexagonDirection direction, HexagonType type) 
    {
        return HexagonPoints(type)[(int)direction + 1];
    }
    
    private static List<Vector3> HexagonPoints(HexagonType type)
    {
        List<Vector3> points;
        switch (type)
        {
            case HexagonType.Grid:
                points = HexagonPoints(OUTER_RADIUS, INNER_RADIUS);
                break;
            case HexagonType.Visual:
                points = HexagonPoints(VISUAL_OUTER_RADIUS, VISUAL_INNER_RADIUS);
                break;
            default:
                throw new Exception("Unhandled HexagonType " + type);
        }

        return points;
    }
    
    private static List<Vector3> HexagonPoints (float outerRadius, float innerRadius)
    {
        return new List<Vector3>
        {
            new Vector3(0f, 0f, outerRadius),
            new Vector3(innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(0f, 0f, -outerRadius),
            new Vector3(-innerRadius, 0f, -0.5f * outerRadius),
            new Vector3(-innerRadius, 0f, 0.5f * outerRadius),
            new Vector3(0f, 0f, outerRadius)
        };
    }
}
