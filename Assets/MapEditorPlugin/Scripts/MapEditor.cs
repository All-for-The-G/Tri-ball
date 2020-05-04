using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System;
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
        [HideInInspector] public float radius = 1.9f;
        [HideInInspector] public int width = 10;
        [HideInInspector] public int height = 10;
        [HideInInspector] public TileType[,] grid = new TileType[0,0];
        //public Vector3[] hexagonPos = new Vector3[] { };
        [HideInInspector] public string mapEditorScenePath = "Assets/MapEditorPlugin/MapEditorPlugin_demo_scene.unity";
#if UNITY_EDITOR
        public Transform mapGarbage;
        public Transform debugCube;
        public MapAssets mapAssets;
        public TriBallMenuConfig menuConfig;
        public bool captureMode = true;
        public bool mapDebug = false;
#endif
        private void Start()
        {
            Init();
        }
        private void Init()
        {
#if UNITY_EDITOR
            if (0<mapGarbage.childCount)
            {
                Debug.Log("Garbage was filled last time!");
                List<GameObject> trash = new List<GameObject>();
                for (int i = 0; i < mapGarbage.childCount; i++)
                {
                    trash.Add(mapGarbage.GetChild(mapGarbage.childCount-1).gameObject);
                }
                trash.ForEach((t)=> { DestroyImmediate(t); });
            }
#endif
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
    [Serializable]
    public class TriBallMenuConfig
    {
        public Texture grass = null;
        public Texture dirt = null;
        public Texture water = null;
        public Texture blocks = null;
        public Texture editorLogo = null;
    }

    [Serializable]
    public class MapAssets
    {
        public GameObject HexagonPrefab;
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
        public Color selectionColor = Color.white;
        public TileType paintTileType = TileType.DEFAULT;
        private List<GameObject> allHexagones = new List<GameObject>();
        private List<GameObject> hexagonePool = new List<GameObject>();
        private bool mouseDown = false;
        InspectorPlugin()
        {
            instance = this;
        }
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            mapEditor = (MapEditor)target;
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
            mapEditor = (MapEditor)target;
            if (mapEditor==null && mapEditor.debugCube==null) return;
            Event guiEvent = Event.current;

            if (guiEvent.type == EventType.Repaint)
            {
                if(mapEditor.grid.Length == 0)
                {
                    DrawHexagon(/*selectedPos*/Vector3.zero, Color.green, true);
                }
                mapEditor.debugCube.gameObject.SetActive(mouseDown);
                if (mouseDown)
                {
                    DrawHexagon(selectedPos/*Vector3.zero*/, selectionColor, true);
                    mapEditor.debugCube.transform.position = selectedPos;
                    Collider[] colliders = Physics.OverlapSphere(selectedPos, mapEditor.radius);
                    if (0 < colliders.Length)
                    {
                        Renderer rr = null;
                        float minDistance = mapEditor.radius * 2;
                        for (int i=0;i<colliders.Length;i++)
                        {
                            float distance = Vector3.Distance(selectedPos,colliders[i].transform.position);
                            if (distance<=minDistance)
                            {
                                minDistance = distance;
                                rr = colliders[i]?.GetComponent<Renderer>();
                            }
                        }
                        if (rr!=null)
                        {
                            rr.material.color = selectionColor;
                        }
                    }
                }
                if(mapEditor.mapDebug)
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
                mouseDown = true;
            }
            else
            {
                mouseDown = false;
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
                        tileColor = Color.red;
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
                    if (Vector3.Distance(selectedPos, new Vector3(pos.x,0,pos.y)) < 1.1)
                    {
                        me.grid[i, j] = paintTileType;
                    }
                }
            }
        }
        private void DrawMenu()
        {
            if (mapEditor == null)
            {
                Debug.Log(nameof(mapEditor)+" is Null");
                mapEditor = (MapEditor)target;
            }
            if (mapEditor == null)
            {
                Debug.Log(nameof(mapEditor) + " is still Null");
                return;
            }
            if (mapEditor.menuConfig == null) { Debug.Log("menuConfig is " + mapEditor.menuConfig); return; }
            Handles.BeginGUI();
            if (GUILayout.Button(mapEditor.menuConfig.grass/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.GRASS;
                selectionColor = Color.green;
            }
            if (GUILayout.Button(mapEditor.menuConfig.dirt/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.DIRT; ;
                selectionColor = Color.red;
            }
            if (GUILayout.Button(mapEditor.menuConfig.water/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.WATER; ;
                selectionColor = Color.blue;
            }
            if (GUILayout.Button(mapEditor.menuConfig.blocks/*"Reset Area"*/, GUILayout.Width(100), GUILayout.Height(100)))
            {
                paintTileType = TileType.BLOCKED; ;
                selectionColor = Color.black;
            }
            Rect newRect = new Rect();
            newRect.Set(110, 20, 100, 300);
            GUILayout.BeginArea(newRect);
            InspectorPlugin ip = InspectorPlugin.instance;
            if (ip == null) return;
            MapEditor me = ip.mapEditor;
            if (me == null) return;
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
                ClearHexagones();
                me.GenerateMap();
                GenerateHexagonGrid();
            }
            if (GUILayout.Button("Load Map"))
            {
                me.LoadMap();
            }
            if (GUILayout.Button("Save Map"))
            {
                me.SaveMap();
            }
            GUILayout.EndArea();
            Handles.EndGUI();
        }
        private void DrawHexagon(Vector3 _pos, Color _default, bool _ignoreSelection = false)
        {
            Vector3[] hexagon = new Vector3[] {
                    new Vector3(0.86f,0f,0.5f)+_pos,
                    new Vector3(0.86f,0f,-0.5f)+_pos,
                    new Vector3(0.0f,0f,-1f)+_pos,
                    new Vector3(-0.86f,0f,-0.5f)+_pos,
                    new Vector3(-0.86f,0f,0.5f)+_pos,
                    new Vector3(0.0f,0f,1.0f)+_pos
                };
            _default.a = .4f;
            Handles.color = _default;
            Handles.DrawAAConvexPolygon(hexagon);
            Handles.color = Color.red; hexagon = new Vector3[] {
                    new Vector3(0.86f,0f,0.5f)+_pos,
                    new Vector3(0.86f,0f,-0.5f)+_pos,
                    new Vector3(0.0f,0f,-1f)+_pos,
                    new Vector3(-0.86f,0f,-0.5f)+_pos,
                    new Vector3(-0.86f,0f,0.5f)+_pos,
                    new Vector3(0.0f,0f,1.0f)+_pos,
                    new Vector3(0.86f,0f,0.5f)+_pos
                };
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
            mapEditor = (MapEditor)target;
            //Selection.SetActiveObjectWithContext();
            //SelectEditorObject();
            //Debug.Log("OnEnable");
            Tools.hidden = true;
            SceneView.lastActiveSceneView.drawGizmos = false;
            SceneView.lastActiveSceneView.Focus();
            //WindowPlugin.instance.Focus();
            //WindowPlugin.Init();
            FocusPosition(((MapEditor)target).transform.position);
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
            //Debug.Log(Selection.activeObject);
            if (Selection.activeObject != null && Selection.activeGameObject == null) return;
            if (mapEditor == null) return;
            if(mapEditor.captureMode)
            Selection.activeObject = target;
        }
        private void FocusPosition(Vector3 pos)
        {
            SceneView.lastActiveSceneView.Frame(new Bounds(pos, Vector3.one), false);
        }
        private void ClearHexagones()
        {
            if ((allHexagones.Count + hexagonePool.Count)<1000)
            {
                allHexagones.ForEach((hexa) => { hexagonePool.Add(hexa); hexa.SetActive(false); });
                allHexagones.Clear();
            }
            allHexagones.ForEach((hexa) => { Destroy(hexa); });
            allHexagones.Clear();
        }
        private void CreateNewHexagon(Vector3 _position)
        {
            MapEditor me = (MapEditor)target;
            if (me == null || me.mapAssets == null || me.mapAssets.HexagonPrefab == null || me.mapGarbage == null) return;
            if (0<hexagonePool.Count)
            {
                GameObject hexaPooled = hexagonePool[hexagonePool.Count - 1];
                hexagonePool.Remove(hexaPooled);
                hexaPooled.SetActive(true);
                hexaPooled.transform.SetParent(me.mapGarbage);
                hexaPooled.transform.localScale = Vector3.one;
                hexaPooled.transform.position = _position;
                allHexagones.Add(hexaPooled);

                Renderer rrp = hexaPooled.GetComponent<Renderer>();
                var tempMaterial = new Material(rrp.sharedMaterial);
                tempMaterial.color = Color.white;
                rrp.material = tempMaterial;
                return;
            }
            GameObject newHexa = Instantiate(me.mapAssets.HexagonPrefab);
            newHexa.transform.SetParent(me.mapGarbage);
            newHexa.transform.localScale = Vector3.one;
            newHexa.transform.position = _position;
            allHexagones.Add(newHexa);
            Renderer rr = newHexa.GetComponent<Renderer>();
            var tm = new Material(rr.sharedMaterial);
            tm.color = Color.white;
            rr.material = tm;
        }
        private void GenerateHexagonGrid()
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
                    //DrawHexagon(new Vector3(pos.x, 0, pos.y), tileColor);
                    CreateNewHexagon(new Vector3(pos.x, 0, pos.y));
                }
            }
        }
    }
    public class WindowPlugin : EditorWindow
    {

        public static WindowPlugin instance = null;
        public static bool isInitialize = false;
        //[MenuItem("TriBall/MapEditorWindow %W")]
        [MenuItem("TriBall/MapEditorWindow")]
        public static void Init()
        {
            GetWindow<WindowPlugin>("Map Editor Window");
            isInitialize = true;
        }

        public WindowPlugin()
        {
            instance = this;
        }

        void OnGUI()
        {
            InspectorPlugin ip = InspectorPlugin.instance;
            if (ip == null) return;
            MapEditor me = ip.mapEditor;
            if (me == null) return;
            GUILayout.Button(me.menuConfig.editorLogo, GUILayout.Width(100), GUILayout.Height(100));
            GUILayout.Label("Tri-Ball map editor plugin v1.0");
            /*
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
            */
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
#endif
}