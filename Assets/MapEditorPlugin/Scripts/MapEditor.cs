using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

//[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    public bool drawHexagon = false;
    public Vector3[] hexagonPos = new Vector3[] { };
    public void GenerateMap()
    {
        drawHexagon = true;
        Debug.Log("Bingo Map Generated!");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapEditor))]
//[CanEditMultipleObjects]
public class InspectorPlugin : Editor
{
    MapEditor mapEditor;
    Vector3 a;
    bool repaint = false;
    Vector3 selectedPos = Vector2.one * 1000f;
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        mapEditor = (MapEditor)target;
        GUILayout.Label("M A P   E D I T O R");
        a = EditorGUILayout.Vector3Field("Enter V3",a);
        if (GUILayout.Button("Generate Map Visuals"))
        {
            Vector3[] newHexagon = new Vector3[mapEditor.hexagonPos.Length+1];
            for (int i=0;i< mapEditor.hexagonPos.Length;i++)
            {
                newHexagon[i] = mapEditor.hexagonPos[i];
            }
            newHexagon[newHexagon.Length - 1] = a;
            mapEditor.hexagonPos = newHexagon;
            mapEditor.GenerateMap();
        }
    }
    public void OnSceneGUI()
    {
        Event guiEvent = Event.current;

        if (guiEvent.type == EventType.Repaint)
        {
            //DrawHexagon(selectedPos, Color.red,true);
            Draw();
        }
        else if (guiEvent.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
        }
        else
        {
            HandleInput(guiEvent);
            if (repaint)
            {
                HandleUtility.Repaint();
            }
        }

    }
    private void HandleInput(Event _guiEvent)
    {
        Ray mouseRay = HandleUtility.GUIPointToWorldRay(_guiEvent.mousePosition);
        float discHeight = 0, distToDisc = (discHeight - mouseRay.origin.z) / mouseRay.direction.z;
        Vector3 mousePosition = mouseRay.GetPoint(distToDisc);

        if ((_guiEvent.type == EventType.MouseDown || _guiEvent.type == EventType.MouseDrag) && _guiEvent.button == 0)
        {
            //UpdateSelection(mousePosition);
            //Debug.Log("Zero: "+Camera.current.WorldToScreenPoint(mapEditor.hexagonPos[0])+", mpos:"+ mousePosition);
            Vector3 rayHitPos = GetGroundHitPos(mouseRay.origin,mouseRay.direction);
            Debug.Log("Mouse Ray: " + rayHitPos);
            selectedPos = rayHitPos;
        }

        if (_guiEvent.type == EventType.MouseUp && _guiEvent.button == 0)
        {
            repaint = true;
        }
    }
    private void Draw()
    {
        if (mapEditor!=null && mapEditor.drawHexagon)
        {
            for (int i = 0; i < mapEditor.hexagonPos.Length; i++)
            {
                DrawHexagon(mapEditor.hexagonPos[i],Color.white);
            }
            //Debug.Log("Yep!");
        }
    }
    private void DrawHexagon(Vector3 _pos,Color _default,bool _ignoreSelection = false)
    {
        Vector3[] hexagon = new Vector3[] {
                    new Vector3(0f,0f,1.7f)+_pos,
                    new Vector3(1.7f,0f,.85f)+_pos,
                    new Vector3(1.7f,0f,-.85f)+_pos,
                    new Vector3(0f,0f,-1.7f)+_pos,
                    new Vector3(-1.7f,0f,-.85f)+_pos,
                    new Vector3(-1.7f,0f,.85f)+_pos
                };
        Handles.color = _default;
        if (Vector3.Distance(selectedPos,_pos)<1.7 && !_ignoreSelection)
        {
            Handles.color = Color.blue;
        }
        Handles.DrawAAConvexPolygon(hexagon);
        Handles.color = Color.red;
        Handles.DrawPolyLine(hexagon);
        Handles.color = Color.white;
    }
    private float GetHypotenuse(Vector3 _rayOrigin,Vector3 _rayDir)
    {
        float Hypotenuse = Vector3.Distance(_rayOrigin, new Vector3(_rayOrigin.x, 0,_rayOrigin.z)) / Mathf.Sin((180 - (Vector3.Angle((new Vector3(_rayOrigin.x, 0,_rayOrigin.z) - _rayOrigin).normalized, _rayDir) + 90)) * Mathf.Deg2Rad);
        return Hypotenuse;
    }
    private Vector3 GetGroundHitPos(Vector3 _rayOrigin, Vector3 _rayDir)
    {
        Vector3 GroundHitPos = _rayOrigin + (_rayDir * GetHypotenuse(_rayOrigin, _rayDir));
        return GroundHitPos;
    }
}
#endif

