using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grid : MonoBehaviour
{
    [SerializeField] private int width = 6;
    [SerializeField] private int height = 6;
    [SerializeField] private Color defaultColor = Color.white;

    [SerializeField] private Hexagon hexagonPrefab;
    
    private List<Hexagon> cells;
    private HexagonMesh hexagonMesh;

    private void Awake () 
    {
        cells = new List<Hexagon>();
        hexagonMesh = GetComponentInChildren<HexagonMesh>();

        for (int z = 0; z < height; z++) {
            for (int x = 0; x < width; x++) {
                CreateCell(x, z);
            }
        }
    }
    
    private void Start () 
    {
        hexagonMesh.Triangulate(cells);
    }

    public void ColorCell (Vector3 position, Color color) {
        position = transform.InverseTransformPoint(position);
        HexCoordinates coordinates = HexCoordinates.FromPosition(position);
        int index = Hexagon.Index(coordinates, width);
        Hexagon cell = cells[index];
        cell.Color = color;
        hexagonMesh.Triangulate(cells);
    }
	
    private void CreateCell (int x, int z) 
    {
        Vector3 position;
        position.x = (x + z * 0.5f - z / 2) * (GridSettings.INNER_RADIUS * 2f);
        position.y = 0f;
        position.z = z * (GridSettings.OUTER_RADIUS * 1.5f);

        Hexagon cell = Instantiate(hexagonPrefab);
        cells.Add(cell);
        cell.transform.SetParent(transform, false);
        cell.transform.localPosition = position;
        cell.Coordinates = HexCoordinates.FromOffsetCoordinates(x, z);
        cell.Color = defaultColor;
        
        //FIXME: this logic hurts my soul. Should be a more elegant way to write this.
        int index = Hexagon.Index(cell.Coordinates, width);
        if (x > 0) {
            cell.SetNeighbor(HexagonDirection.W, cells[index - 1]);
            if (z > 0 && (z & 1) == 0) //(z & 1) == 0 checks if its even
            {
                cell.SetNeighbor(HexagonDirection.SW, cells[index - width - 1]);
            }
        }
        if (z > 0) {
            if ((z & 1) == 0) {
                cell.SetNeighbor(HexagonDirection.SE, cells[index - width]);
            }
            else {
                cell.SetNeighbor(HexagonDirection.SW, cells[index - width]);
                if (x < width - 1) {
                    cell.SetNeighbor(HexagonDirection.SE, cells[index - width + 1]);
                }
            }
        }
    }
}
