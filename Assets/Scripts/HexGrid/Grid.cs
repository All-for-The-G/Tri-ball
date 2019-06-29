using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Xml.Serialization;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private Hexagon hexagonPrefab;
    
    private int width;
    private int height;
    private List<Hexagon> cells;

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
        HexagonData data = new HexagonData(cell.HexagonData.Coordinates, type, cell.HexagonData.XIndex, cell.HexagonData.ZIndex);
        cell.HexagonData = data;
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

        Hexagon cell = Instantiate(hexagonPrefab);
        cells.Add(cell);
        cell.transform.SetParent(transform, false);
        cell.transform.localScale = new Vector3(GridSettings.VISUAL_OUTER_RADIUS * 36f, GridSettings.VISUAL_OUTER_RADIUS * 36f, GridSettings.VISUAL_OUTER_RADIUS * 36f);
        cell.transform.localPosition = position;
        HexagonData hexagonData = new HexagonData(HexCoordinates.FromOffsetCoordinates(x, z), type, x, z);
        cell.HexagonData = hexagonData;

        //FIXME: this logic hurts my soul. Should be a more elegant way to write this.
        int index = Hexagon.Index(hexagonData.Coordinates, width);
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
