using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[Obsolete]
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class HexagonMesh : MonoBehaviour
{
    
    private Mesh hexMesh;
    private List<Vector3> vertices;
    private List<Color> colors;
    private List<int> triangles;
    private MeshCollider meshCollider;

    private void Awake () 
    {
        GetComponent<MeshFilter>().mesh = hexMesh = new Mesh();
        hexMesh.name = "Hexagon Mesh";
        vertices = new List<Vector3>();
        colors = new List<Color>();
        triangles = new List<int>();
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }
    public void Triangulate (List<Hexagon> cells) 
    {
        hexMesh.Clear();
        vertices.Clear();
        colors.Clear();
        triangles.Clear();
        
        cells.ForEach(Triangulate);

        hexMesh.vertices = vertices.ToArray();
        hexMesh.colors = colors.ToArray();
        hexMesh.triangles = triangles.ToArray();
        hexMesh.RecalculateNormals();

        meshCollider.sharedMesh = hexMesh;
    }
	
    private void Triangulate (Hexagon cell) 
    {
        for (HexagonDirection direction = HexagonDirection.NE; direction <= HexagonDirection.NW; direction++) 
        {
            Triangulate(direction, cell);
        }
    }
    
    private void Triangulate (HexagonDirection direction, Hexagon cell) 
    {
        Vector3 center = cell.transform.localPosition;
        AddTriangle(
            center,
            center + GridSettings.GetFirstCorner(direction, HexagonType.Visual),
            center + GridSettings.GetSecondCorner(direction, HexagonType.Visual)
        );
        //AddTriangleColor(cell.Color);
    }
    
    private void AddTriangleColor (Color color) 
    {
        colors.Add(color);
        colors.Add(color);
        colors.Add(color);
    }
    
    private void AddTriangle (Vector3 v1, Vector3 v2, Vector3 v3) 
    {
        int vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }
}
