using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon : MonoBehaviour
{
    [SerializeField] private HexCoordinates coordinates;
    [SerializeField] private Color color;
    [SerializeField] private List<Hexagon> neighbors; //Not not initialize in code. Set size in prefab instead.

    public HexCoordinates Coordinates
    {
        get => coordinates;
        set => coordinates = value;
    }

    public Color Color
    {
        get => color;
        set => color = value;
    }
    
    public Hexagon GetNeighbor (HexagonDirection direction) {
        return neighbors[(int)direction];
    }

    public static int Index(HexCoordinates coordinates, int width)
    {
        return coordinates.X + coordinates.Z * width + coordinates.Z / 2;
    }
    
    public void SetNeighbor (HexagonDirection direction, Hexagon cell) {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
