using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

[ExecuteInEditMode]
public class MapEditor : MonoBehaviour
{
    public Vector3[] hexagonPos = new Vector3[] { };
    [HideInInspector] public string mapEditorScenePath = "Assets/MapEditorPlugin/MapEditorPlugin_demo_scene.unity";
    private void Start()
    {
        Selection.activeGameObject = gameObject;
    }
    public void GenerateMap()
    {
        Debug.Log("Bingo Map Generated!");
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(MapEditor))]
//[CanEditMultipleObjects]
public class InspectorPlugin : Editor
{
    public static InspectorPlugin instance;
    static string mapEditorScenePath = "Assets/MapEditorPlugin/MapEditorPlugin_demo_scene.unity";
    MapEditor mapEditor;
    Vector3 a;
    bool repaint = false;
    Vector3 selectedPos = Vector2.one * 1000f;
    public Color selectionColor = Color.blue;
    InspectorPlugin()
    {
        instance = this;
    }
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        mapEditor = (MapEditor)target;
        GUILayout.Label("M A P   E D I T O R");
        //GUILayout.Label("Map Editor Scene Path");
        //mapEditor.mapEditorScenePath = EditorGUILayout.TextField(mapEditor.mapEditorScenePath);
        //mapEditorScenePath = mapEditor.mapEditorScenePath;
        GUILayout.BeginHorizontal();
        a = EditorGUILayout.Vector3Field("Enter New Pos",a);
        GUILayout.EndHorizontal();
        if (GUILayout.Button("Add new hexagon"))
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
        if (mapEditor!=null)
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
            Handles.color = selectionColor;
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
    [MenuItem("TriBall/MapEditor %M")]
    static void OpenBuiltInMapEditor()
    {
        if (mapEditorScenePath != "")
        {
            EditorSceneManager.OpenScene(mapEditorScenePath);
            EditorSceneManager.sceneLoaded += (scene,mode)=> {
            };
        }
    }
    private void OnEnable()
    {
        //Selection.SetActiveObjectWithContext();
        //SelectEditorObject();
        FocusPosition(((MapEditor)target).transform.position);
        Debug.Log("OnEnable");
        Tools.hidden = true;
        SceneView.lastActiveSceneView.drawGizmos = false;
        SceneView.lastActiveSceneView.Focus();
        //WindowPlugin.instance.Focus();
        WindowPlugin.Init();
    }
    private void OnDestroy()
    {
        Tools.hidden = false;
        SceneView.lastActiveSceneView.drawGizmos = true;
    }
    private void OnDisable()
    {
        SelectEditorObject();
    }
    private void SelectEditorObject()
    {
        Selection.activeObject = target;
    }
    public void FocusPosition(Vector3 pos)
    {
        SceneView.lastActiveSceneView.Frame(new Bounds(pos, Vector3.one), false);
    }
}
public class WindowPlugin: EditorWindow
{

    public static WindowPlugin instance = null;

    [MenuItem("TriBall/MapEditorWindow %M")]
    public static void Init()
    {
        GetWindow<WindowPlugin>("Map Editor Window");
    }

    public WindowPlugin()
    {
        instance = this;
    }

    void OnGUI()
    {
        GUILayout.Label("TRI-BALL M A P   E D I T O R");
        if (GUILayout.Button("Green Color"))
        {
            //WindowPlugin.instance.Focus();
            InspectorPlugin.instance.selectionColor = Color.green;
        }
        if (GUILayout.Button("Red Color"))
        {
            //WindowPlugin.instance.Focus();
            InspectorPlugin.instance.selectionColor = Color.red;
        }
        if (GUILayout.Button("Yellow Color"))
        {
            //WindowPlugin.instance.Focus();
            InspectorPlugin.instance.selectionColor = Color.yellow;
        }
    }
}
#endif

