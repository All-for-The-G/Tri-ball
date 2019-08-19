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
    public const float OUTER_RADIUS = 2.6f;
    public const float VISUAL_OUTER_RADIUS = 2.6f;
    public const float INNER_RADIUS = OUTER_RADIUS * 0.866025404f;
    public const float VISUAL_INNER_RADIUS = VISUAL_OUTER_RADIUS * 0.866025404f;
}
