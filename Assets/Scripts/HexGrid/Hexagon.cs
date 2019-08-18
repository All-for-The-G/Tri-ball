using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class Hexagon : MonoBehaviour
{
    public enum TileType
    {
        Grass,
        Desert
    }

    [SerializeField] private HexagonData hexagonData;
    [SerializeField] private List<Hexagon> neighbors; //Do not initialize in code. Set size in prefab instead.

    private Renderer renderer;
    private void Awake()
    {
        renderer = GetComponent<Renderer>();
    }

    public HexagonData HexagonData { get; set; }
    
    public Hexagon GetNeighbor (HexagonDirection direction) 
    {
        return neighbors[(int)direction];
    }

    public static int Index(HexCoordinates coordinates, int width)
    {
        return coordinates.X + coordinates.Z * width + coordinates.Z / 2;
    }
    
    public void SetNeighbor (HexagonDirection direction, Hexagon cell) 
    {
        neighbors[(int)direction] = cell;
        cell.neighbors[(int)direction.Opposite()] = this;
    }
}
