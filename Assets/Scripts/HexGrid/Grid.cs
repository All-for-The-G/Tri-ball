using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Hexagon grassHexagon;
    [SerializeField] private Hexagon dirtHexagon;
    
    private int width;
    private int height;
    private List<Hexagon> cells;

    public List<Hexagon> Cells => cells;
    public int Width => width;
    public int Height => height;

    private void Awake () 
    {
       GenerateEmptyGrid(10,10);
    }

    public void ChangeCellType (Vector3 position, Hexagon.TileType type) 
    {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = Hexagon.Index(coordinates, width);
        Hexagon cell = cells[index];
        if (cell.HexagonData.Type == type)
        {
            return;
        }
        
        HexagonData data = new HexagonData(cell.HexagonData.Coordinates, type, cell.HexagonData.XIndex, cell.HexagonData.ZIndex);
        InitializeCell(GetPrefabFromType(type), cell.transform.localPosition, data, index);
        Destroy(cell.gameObject);
    }

    //TODO: move generators to own class, to ensure height/width is always updated
    public void GenerateGrid(int height, int width, List<HexagonData> hexagonData)
    {
        DestroyGrid();
        this.height = height;
        this.width = width;
        hexagonData.ForEach(datum => CreateCell(datum.XIndex, datum.ZIndex, datum.Type));
    }

    public void GenerateEmptyGrid(int height, int width)
    {
        DestroyGrid();
        this.height = height;
        this.width = width;

        for (int z = 0; z < height; z++) 
        {
            for (int x = 0; x < width; x++) 
            {
                CreateCell(x, z);
            }
        }
    }

    private void CreateCell (int x, int z, Hexagon.TileType type = Hexagon.TileType.Grass) 
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (GridSettings.INNER_RADIUS * 2f);
        position.y = 0f;
        position.z = z * (GridSettings.OUTER_RADIUS * 1.5f);
        HexagonData hexagonData = new HexagonData(HexCoordinates.FromOffsetCoordinates(x, z), type, x, z);

        InitializeCell(GetPrefabFromType(type), position, hexagonData);
    }

    private void InitializeCell(Hexagon prefab, Vector3 position, HexagonData data, int index = -1)
    {
        Hexagon cell = Instantiate(prefab);
        if (index >= 0)
        {
            cells[index] = cell;
        }
        else
        {
            cells.Add(cell);
        }
        
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.HexagonData = data;
        
        //Need to recalculate entire grid, if inserting a new cell in the middle.
        
        if (index >= 0)
        {
            cells.ForEach(CalculateNeighbours);
        }
        else
        {
            CalculateNeighbours(cell);
        }
        
        
    }

    private Hexagon GetPrefabFromType(Hexagon.TileType type)
    {
        Hexagon prefab;
        switch (type)
        {
            case Hexagon.TileType.Grass:
                prefab = grassHexagon;
                break;
            case Hexagon.TileType.Desert:
                prefab = dirtHexagon;
                break;
            default:
                prefab = grassHexagon;
                break;
        }

        return prefab;
    }

    private void CalculateNeighbours(Hexagon cell)
    {
        int index = Hexagon.Index(cell.HexagonData.Coordinates, width);
        int x = (int)cell.transform.position.x;
        int z = (int)cell.transform.position.z;
        if (x > 0) 
        {
            cell.SetNeighbor(HexagonDirection.W, cells[index - 1]);
            if (z > 0 && (z & 1) == 0) //(z & 1) == 0 checks if its even
            {
                cell.SetNeighbor(HexagonDirection.SW, cells[index - width - 1]);
            }
        }
        if (z > 0) 
        {
            if ((z & 1) == 0) 
            {
                cell.SetNeighbor(HexagonDirection.SE, cells[index - width]);
            }
            else {
                cell.SetNeighbor(HexagonDirection.SW, cells[index - width]);
                if (x < width - 1) 
                {
                    cell.SetNeighbor(HexagonDirection.SE, cells[index - width + 1]);
                }
            }
        }
    }

    private void DestroyGrid()
    {
        cells?.ForEach(cell => Destroy(cell.gameObject));
        cells = new List<Hexagon>();
    }
}
