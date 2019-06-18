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
    
    [SerializeField] private HexCoordinates coordinates;
    [SerializeField] private TileType type;
    [SerializeField] private List<Hexagon> neighbors; //Do not initialize in code. Set size in prefab instead.
    [SerializeField] private List<Material> materials; //Do not initialize in code. Set size in prefab instead.

    private Renderer renderer;
    private void Awake()
    {
        type = TileType.Grass;
        renderer = GetComponent<Renderer>();
    }

    public HexCoordinates Coordinates
    {
        get => coordinates;
        set => coordinates = value;
    }

    public TileType Type
    {
        get => type;
        set { type = value; UpdateMaterial(); }
    }

    private void UpdateMaterial()
    {
        if (materials.ElementAtOrDefault((int) type) != null)
        {
            renderer.material = materials.ElementAt((int) type);
        }
        else
        {
            renderer.material = materials.ElementAt((int) TileType.Grass);
            Debug.LogError("Could not find material for " + type + " at " + coordinates);
        }
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
