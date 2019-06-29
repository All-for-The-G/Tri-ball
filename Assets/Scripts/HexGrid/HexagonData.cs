using UnityEngine;

public struct HexagonData
{
    [SerializeField] private HexCoordinates coordinates;
    [SerializeField] private Hexagon.TileType type;
    private int xIndex, zIndex;

    public HexagonData(HexCoordinates coordinates, Hexagon.TileType type, int xIndex, int zIndex)
    {
        this.coordinates = coordinates;
        this.type = type;
        this.xIndex = xIndex;
        this.zIndex = zIndex;
    }

    public HexCoordinates Coordinates => coordinates;

    public Hexagon.TileType Type => type;

    public int XIndex => xIndex;

    public int ZIndex => zIndex;
}
