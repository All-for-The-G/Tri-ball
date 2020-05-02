﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

namespace TriBallMapEditorPlugin
{
    public enum TileType
    {
        GRASS,
        DIRT,
        WATER,
        BLOCKED,
        DEFAULT,
    }
    [ExecuteInEditMode]
    public class MapEditor : MonoBehaviour
    {
        [HideInInspector] public float radius = 3.4f;
        [HideInInspector] public int width = 10;
        [HideInInspector] public int height = 10;
        [HideInInspector] public TileType[,] grid = new TileType[0,0];
        //public Vector3[] hexagonPos = new Vector3[] { };
        [HideInInspector] public string mapEditorScenePath = "Assets/MapEditorPlugin/MapEditorPlugin_demo_scene.unity";
#if UNITY_EDITOR
        public TriBallMenuConfig menuConfig;
#endif
        private void Start()
        {
            GenerateMap();
            Selection.activeGameObject = gameObject;
        }
        public void GenerateMap()
        {
            grid = new TileType[width,height];
            for (int i = 0; i < width; i++)
            {
                for (int j = 0; j < height; j++)
                {
                    grid[i, j] = TileType.DEFAULT;
                }
            }
            Debug.Log("Bingo Map Generated!");
        }
        public void LoadMap()
        {

        }
        public void SaveMap()
        {

        }
    }

    [CreateAssetMenu(fileName = "TriBallMapData", menuName = "TriBall/Map Data")]
    public class TriBallMapData : ScriptableObject
    {

    }
#if UNITY_EDITOR
    [CustomEditor(typeof(MapEditor))]
    //[CanEditMultipleObjects]
    public class InspectorPlugin : Editor
    {
        public static InspectorPlugin instance;
        static string mapEditorScenePath = "Assets/MapEditorPlugin/MapEditorPlugin_demo_scene.unity";
        public MapEditor mapEditor;
        Vector3 a;
        bool repaint = false;
        Vector3 selectedPos = Vector2.one * 1000f;
        //public Color selectionColor = Color.white;
        public TileType paintTileType = TileType.DEFAULT;
        InspectorPlugin()
        {
            instance = this;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            //mapEditor = (MapEditor)target;
            //GUILayout.Label("M A P   E D I T O R");
            //GUILayout.Label("Map Editor Scene Path");
            //mapEditor.mapEditorScenePath = EditorGUILayout.TextField(mapEditor.mapEditorScenePath);
            //mapEditorScenePath = mapEditor.mapEditorScenePath;
            //GUILayout.BeginHorizontal();
            //a = EditorGUILayout.Vector3Field("Enter New Pos", a);
            //GUILayout.EndHorizontal();
            //if (GUILayout.Button("Add new hexagon"))
            //{
            //    Vector3[] newHexagon = new Vector3[mapEditor.hexagonPos.Length + 1];
            //    for (int i = 0; i < mapEditor.hexagonPos.Length; i++)
            //    {
            //        newHexagon[i] = mapEditor.hexagonPos[i];
            //    }
            //    newHexagon[newHexagon.Length - 1] = a;
            //    mapEditor.hexagonPos = newHexagon;
            //    mapEditor.GenerateMap();
            //}
        }
        public void OnSceneGUI()
        {
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.Repaint)
            {
                DrawHexagon(/*selectedPos*/Vector3.zero, Color.red, true);
                DrawMap();
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
            DrawMenu();
        }
        private void HandleInput(Event _guiEvent)
        {
            Ray mouseRay = HandleUtility.GUIPointToWorldRay(_guiEvent.mousePosition);
            float discHeight = 0, distToDisc = (discHeight - mouseRay.origin.z) / mouseRay.direction.z;
            Vector3 mousePosition = mouseRay.GetPoint(distToDisc);

            if ((_guiEvent.type == EventType.MouseDown || _guiEvent.type == EventType.MouseDrag) && _guiEvent.button == 0)
            {
                //Debug.Log("Zero: "+Camera.current.WorldToScreenPoint(mapEditor.hexagonPos[0])+", mpos:"+ mousePosition);
                Vector3 rayHitPos = GetGroundHitPos(mouseRay.origin, mouseRay.direction);
                //Debug.Log("Mouse Ray: " + rayHitPos);
                selectedPos = rayHitPos;
                UpdateSelection(selectedPos);
            }

            if (_guiEvent.type == EventType.MouseUp && _guiEvent.button == 0)
            {
                repaint = true;
            }
        }
        private void DrawMap()
        {
            //for (int i = 0; i < mapEditor.hexagonPos.Length; i++)
            //{
            //    DrawHexagon(mapEditor.hexagonPos[i], Color.white);
            //}
            //Debug.Log("Yep!");
            MapEditor me = (MapEditor)target;
            if (me == null || (me.width * me.height != me.grid.Length)) return;
            float w = me.width;
            float h = me.height;
            float r = me.radius;
            for (int i = 0; i < (int)w; i++)
            {
                for (int j = 0; j < (int)h; j++)
                {
                    Color tileColor = Color.red;
                    TileType tt = me.grid[i, j];
                    if (tt == TileType.DEFAULT)
                    {
                        tileColor = Color.white;
                    }
                    else if (tt == TileType.GRASS)
                    {
                        tileColor = Color.green;
                    }
                    else if (tt == TileType.DIRT)
                    {
                        tileColor = Color.grey;
                    }
                    else if (tt == TileType.WATER)
                    {
                        tileColor = Color.blue;
                    }
                    else if (tt == TileType.BLOCKED)
                    {
                        tileColor = Color.black;
                    }
                    float offset = j % 2 == 0 ? r / 4 : 3 * r / 4;
                    Vector3 pos = Vector2.zero;
                    pos.x += -(h * r) / 2 + (i * r) + offset; // for hex like structure
                    pos.y += (w * (r - r / 7)) / 2 - (j * (r - r / 7));
                    DrawHexagon(new Vector3(pos.x, 0, pos.y), tileColor);
                }
            }
        }
        private void UpdateSelection(Vector3 _hitPos)
        {
            MapEditor me = (MapEditor)target;
            if (me == null || (me.width * me.height != me.grid.Length)) return;
            float w = me.width;
            float h = me.height;
            float r = me.radius;
            for (int i = 0; i < (int)w; i++)
            {
                for (int j = 0; j < (int)h; j++)
                {
                    float offset = j % 2 == 0 ? r / 4 : 3 * r / 4;
                    Vector3 pos = Vector2.zero;
                    pos.x += -(h * r) / 2 + (i * r) + offset; // for hex like structure
                    pos.y += (w * (r - r / 7)) / 2 - (j * (r - r / 7));
                    if (Vector3.Distance(selectedPos, new Vector3(pos.x,0,pos.y)) < 1.7)
                    {
                        me.grid[i, j] = paintTileType;
                    }
                }
            }
        }
        private void DrawMenu()
        {
            if (mapEditor == null) return;
            Handles.BeginGUI();
            if (GUILayout.Button(mapEditor.menuConfig.grass/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.GRASS;
            }
            if (GUILayout.Button(mapEditor.menuConfig.dirt/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.DIRT;
            }
            if (GUILayout.Button(mapEditor.menuConfig.water/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.WATER;
            }
            if (GUILayout.Button(mapEditor.menuConfig.blocks/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.BLOCKED;
            }
            Handles.EndGUI();
        }
        private void DrawHexagon(Vector3 _pos, Color _default, bool _ignoreSelection = false)
        {
            Vector3[] hexagon = new Vector3[] {
                    new Vector3(0f,0f,1.7f)+_pos,
                    new Vector3(1.7f,0f,1.2f)+_pos,
                    new Vector3(1.7f,0f,-1.2f)+_pos,
                    new Vector3(0f,0f,-1.7f)+_pos,
                    new Vector3(-1.7f,0f,-1.2f)+_pos,
                    new Vector3(-1.7f,0f,1.2f)+_pos
                };
            Handles.color = _default;
            Handles.DrawAAConvexPolygon(hexagon);
            Handles.color = Color.red;
            Handles.DrawPolyLine(hexagon);
            Handles.color = Color.white;
        }
        private float GetHypotenuse(Vector3 _rayOrigin, Vector3 _rayDir)
        {
            float Hypotenuse = Vector3.Distance(_rayOrigin, new Vector3(_rayOrigin.x, 0, _rayOrigin.z)) / Mathf.Sin((180 - (Vector3.Angle((new Vector3(_rayOrigin.x, 0, _rayOrigin.z) - _rayOrigin).normalized, _rayDir) + 90)) * Mathf.Deg2Rad);
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
                EditorSceneManager.sceneLoaded += (scene, mode) => {
                };
            }
        }
        private void OnEnable()
        {
            //Selection.SetActiveObjectWithContext();
            //SelectEditorObject();
            FocusPosition(((MapEditor)target).transform.position);
            //Debug.Log("OnEnable");
            Tools.hidden = true;
            SceneView.lastActiveSceneView.drawGizmos = false;
            SceneView.lastActiveSceneView.Focus();
            //WindowPlugin.instance.Focus();
            WindowPlugin.Init();
            mapEditor = (MapEditor)target;
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
            Debug.Log(Selection.activeObject);
            if (Selection.activeObject != null && Selection.activeGameObject == null) return;
            Selection.activeObject = target;
        }
        public void FocusPosition(Vector3 pos)
        {
            SceneView.lastActiveSceneView.Frame(new Bounds(pos, Vector3.one), false);
        }
    }
    public class WindowPlugin : EditorWindow
    {

        public static WindowPlugin instance = null;

        [MenuItem("TriBall/MapEditorWindow %W")]
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
            InspectorPlugin ip = InspectorPlugin.instance;
            MapEditor me = ip.mapEditor;
            if (ip==null || me==null) return;
            GUILayout.Label("Map Settings");
            GUILayout.BeginHorizontal();
            GUILayout.Label("Radius");
            me.radius = EditorGUILayout.FloatField(me.radius);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Width");
            me.width = EditorGUILayout.IntField(me.width);
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label("Height");
            me.height = EditorGUILayout.IntField(me.height);
            GUILayout.EndHorizontal();
            if (GUILayout.Button("Generate Grid"))
            {
                me.GenerateMap();
            }
            if (GUILayout.Button("Load Map"))
            {
                me.LoadMap();
            }
            if (GUILayout.Button("Save Map"))
            {
                me.SaveMap();
            }
            //if (GUILayout.Button("Red Color"))
            //{
            //    //WindowPlugin.instance.Focus();
            //    InspectorPlugin.instance.selectionColor = Color.red;
            //}
            //if (GUILayout.Button("Yellow Color"))
            //{
            //    //WindowPlugin.instance.Focus();
            //    InspectorPlugin.instance.selectionColor = Color.yellow;
            //}
        }
    }
    [CreateAssetMenu(fileName = "TriBallMenuConfig", menuName = "TriBall/Menu Config")]
    public class TriBallMenuConfig : ScriptableObject
    {
        public Texture grass;
        public Texture dirt;
        public Texture water;
        public Texture blocks;
    }
#endif
}