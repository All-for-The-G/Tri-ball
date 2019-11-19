using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonController : MonoBehaviour
{
    public EditorMap editorMap;
    public Hexagon hexagon;

    void OnMouseDown()
    {
        // this object was clicked - do something
        if (editorMap != null) editorMap.ChangeTileHexagon(hexagon);
    }
}
