using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using SimpleFileBrowser;
using UnityEngine.EventSystems;

public class EditorMap : MonoBehaviour
{

    public bool lockInteraction = false;

    Map map;
    public MapGenerator mapGenerator;

    public InputField inputFieldWidth;
    public InputField inputFieldHeight;
    public Dropdown dropDownEnvironment;
    public Dropdown dropDownLight;

    public GameObject currentObjectToAdd = null;
    public string currentObjectName = "";

    public float scaleFactor = 1f;

    GameObject gameObjectToModify;
    bool moveObject = false;
    Vector3 savePositionObject;

    public Text textCheckMap;

    // Start is called before the first frame update
    void Start()
    {

        FileBrowser.SetFilters(true, new FileBrowser.Filter("Map", ".xml"));
        FileBrowser.SetDefaultFilter(".xml");
        

        map = new Map(8, 8, "Day");
        mapGenerator.GenerateMap(map);
        CheckMap();
    }

    // Update is called once per frame
    void Update()
    {
        
        if(!lockInteraction && !EventSystem.current.IsPointerOverGameObject()) 
        {
            if (currentObjectToAdd != null)
            {
                currentObjectToAdd.transform.localScale = mapGenerator.transform.localScale * scaleFactor * currentObjectToAdd.GetComponent<ObjectMap>().getScaleStart();

                if (!currentObjectToAdd.GetComponent<ObjectMap>().isGameElement)
                {
                    if (Input.GetKey(KeyCode.R)) currentObjectToAdd.transform.Rotate(90f * Time.deltaTime * Vector3.up);
                    if (Input.GetKey(KeyCode.F)) currentObjectToAdd.transform.Rotate(-90f * Time.deltaTime * Vector3.up);

                    if (Input.GetKey(KeyCode.T)) scaleFactor += Time.deltaTime;
                    if (Input.GetKey(KeyCode.G)) scaleFactor -= Time.deltaTime;
                }
                    

                RaycastHit hit;
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                int layerMask = 1 << 8;
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                {
                    currentObjectToAdd.SetActive(true);

                    if(currentObjectToAdd.GetComponent<ObjectMap>().isGameElement) currentObjectToAdd.transform.position = hit.transform.position + new Vector3(0,hit.transform.GetComponent<Collider>().bounds.size.y,0);
                    else currentObjectToAdd.transform.position = hit.point;




                    if (Input.GetMouseButtonDown(0))
                    {

                        bool isPositionOk = true;
                        if (currentObjectToAdd.GetComponent<ObjectMap>().isGameElement)
                        {
                            Hexagon hexagon = hit.transform.GetComponent<HexagonController>().hexagon;
                            isPositionOk = map.CanAddGamePlayObject(currentObjectToAdd.GetComponent<ObjectMap>(), false) && hexagon.CanAddGamePlayObject(currentObjectToAdd.GetComponent<ObjectMap>());
                            
                        }
                        if (isPositionOk)
                        {
                            Vector3 saveEulerAngles = currentObjectToAdd.transform.eulerAngles;
                            GameObject instanceObject = GameObject.Instantiate(currentObjectToAdd, mapGenerator.decoContainer);
                            instanceObject.transform.position = currentObjectToAdd.transform.position;
                            instanceObject.transform.localScale = Vector3.one * scaleFactor * currentObjectToAdd.GetComponent<ObjectMap>().getScaleStart();
                            instanceObject.transform.eulerAngles = saveEulerAngles;


                            instanceObject.GetComponent<ObjectMap>().ChangePosition(hit.transform.GetComponent<HexagonController>().hexagon);


                            map.AddObjectDeco(instanceObject, currentObjectName);
                            CheckMap();
                        }

                            
                    }



                    // Do something with the object that was hit by the raycast.
                }
                else
                {
                    currentObjectToAdd.SetActive(false);
                }

                ResetObjectToModify();
            }
            else
            {
                if (!moveObject)
                {
                    RaycastHit hit;
                    Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                    int layerMask = 1 << 9;
                    if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            ResetObjectToModify();

                            gameObjectToModify = hit.transform.gameObject;

                            gameObjectToModify.GetComponent<ObjectMap>().SetOutline(true);
                        }
                    }
                }


                if (gameObjectToModify != null)
                {


                    if (!gameObjectToModify.GetComponent<ObjectMap>().isGameElement)
                    {
                        if (Input.GetKey(KeyCode.R)) gameObjectToModify.transform.Rotate(90f * Time.deltaTime * Vector3.up);
                        if (Input.GetKey(KeyCode.F)) gameObjectToModify.transform.Rotate(-90f * Time.deltaTime * Vector3.up);

                        if (Input.GetKey(KeyCode.T)) gameObjectToModify.transform.localScale = (gameObjectToModify.transform.localScale.x + Time.deltaTime) * Vector3.one;
                        if (Input.GetKey(KeyCode.G)) gameObjectToModify.transform.localScale = (gameObjectToModify.transform.localScale.x - Time.deltaTime) * Vector3.one;
                    }

                    if (Input.GetKeyDown(KeyCode.Y))
                    {
                        moveObject = true;
                        savePositionObject = gameObjectToModify.transform.localPosition;
                    }
                    


                    if (moveObject)
                    {
                        RaycastHit hit;
                        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                        int layerMask = 1 << 8;
                        if (Physics.Raycast(ray, out hit, Mathf.Infinity, layerMask))
                        {
                            if(gameObjectToModify.GetComponent<ObjectMap>().isGameElement) gameObjectToModify.transform.position = hit.transform.position  + new Vector3(0,hit.transform.GetComponent<Collider>().bounds.size.y,0);
                            else gameObjectToModify.transform.position = hit.point;
                            if (Input.GetMouseButtonDown(0))
                            {
                                bool isPositionOk = true;
                                if(gameObjectToModify.GetComponent<ObjectMap>().isGameElement)
                                {
                                    Hexagon hexagon = hit.transform.GetComponent<HexagonController>().hexagon;
                                    isPositionOk = map.CanAddGamePlayObject(gameObjectToModify.GetComponent<ObjectMap>(), true) && hexagon.CanAddGamePlayObject(gameObjectToModify.GetComponent<ObjectMap>());
                                    if (isPositionOk)
                                    {
                                       
                                        hexagon.AddGamePlayObject(gameObjectToModify.GetComponent<ObjectMap>());
                                    }
                                }
                                if (isPositionOk)
                                {
                                    gameObjectToModify.GetComponent<ObjectMap>().ChangePosition(hit.transform.GetComponent<HexagonController>().hexagon);
                                    moveObject = false;
                                    CheckMap();
                                }
                            }
                        }
                    }

                    if (Input.GetKeyDown(KeyCode.Delete))
                    {
                        map.RemoveFromDeco(gameObjectToModify);
                        gameObjectToModify.GetComponent<ObjectMap>().Destroy();
                        ResetObjectToModify();

                    }
                    else if (Input.GetMouseButtonDown(1))
                    {
                        ResetObjectToModify();
                    }




                }

            }

            if (Input.GetMouseButtonDown(1))
            {
                if (currentObjectToAdd != null) currentObjectToAdd.GetComponent<ObjectMap>().Destroy();
                CheckMap();
                currentObjectToAdd = null;
            }
        }


        

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
        
        
        
    }

    public void ReGenerateMap()
    {
        map = new Map(int.Parse(inputFieldWidth.text), int.Parse(inputFieldHeight.text), dropDownLight.options[dropDownLight.value].text, dropDownEnvironment.value);
        mapGenerator.GenerateMap(map);
        CheckMap();
    }

    public void ChangeTileHexagon(Hexagon hexagon)
    {
        if(!EventSystem.current.IsPointerOverGameObject())
        {
            if (currentObjectToAdd == null)
            {
                int typeHexagon = dropDownEnvironment.value;

                hexagon.type = typeHexagon;
                hexagon.subType = 0;
                Destroy(hexagon.hexagonGO);
                mapGenerator.InstanciateHexagone(hexagon);
            }

        }
        
       

    }

    public void SaveMap()
    {
        StartCoroutine(ShowSaveDialogCoroutine());
    }

    IEnumerator ShowSaveDialogCoroutine()
    {
        CameraHexagone.lockCamera = true;
        lockInteraction = true;
        CanvasEditor.lockInteraction = true;

        yield return FileBrowser.WaitForSaveDialog(false, Application.dataPath, "Save Map", "Save");


        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success)
        {
            SaveContainerMap saveMap = new SaveContainerMap(map);
            saveMap.Save(FileBrowser.Result);
            CameraHexagone.lockCamera = false;
            lockInteraction = false;
            CanvasEditor.lockInteraction = false;
        }
    }

    public void LoadMap()
    {
        StartCoroutine(ShowLoadDialogCoroutine());
    }

    IEnumerator ShowLoadDialogCoroutine()
    {
        CameraHexagone.lockCamera = true;
        CanvasEditor.lockInteraction = true;
        lockInteraction = true;
        yield return FileBrowser.WaitForLoadDialog(false, Application.dataPath, "Load Map", "Load");


        Debug.Log(FileBrowser.Success + " " + FileBrowser.Result);

        if (FileBrowser.Success)
        {
            SaveContainerMap loadMap = SaveContainerMap.Load(FileBrowser.Result);
            map = new Map(loadMap);
            mapGenerator.GenerateMap(map, false);
            CameraHexagone.lockCamera = false;
            lockInteraction = false;
            CanvasEditor.lockInteraction = false;
            CheckMap();
        }
    }

    public void ResetObjectToModify()
    {
        if (moveObject) gameObjectToModify.transform.localPosition = savePositionObject;
        if(gameObjectToModify!=null) gameObjectToModify.GetComponent<ObjectMap>().SetOutline(false);
        gameObjectToModify = null;
        moveObject = false;
    }

    public void CheckMap()
    {
        int[] nbObjectsByType = map.GetNbByObjectType();
        
        if(nbObjectsByType[2]<1) textCheckMap.text = "<color=red>"+nbObjectsByType[2] + "/1 runner team red</color>\n";
        else textCheckMap.text = "<color=blue>" + nbObjectsByType[2] + "/1 runner team red</color>\n";

        if (nbObjectsByType[3] < 2) textCheckMap.text += "<color=red>" + nbObjectsByType[3] + "/2 hunters team red</color>\n";
        else textCheckMap.text += "<color=blue>" + nbObjectsByType[3] + "/2 hunters team red</color>\n";

        if (nbObjectsByType[4] < 4) textCheckMap.text += "<color=red>" + nbObjectsByType[4] + "/4 guardians team red</color>\n";
        else textCheckMap.text += "<color=blue>" + nbObjectsByType[4] + "/4 guardians team red</color>\n";

        if (nbObjectsByType[5] < 1) textCheckMap.text += "<color=red>" + nbObjectsByType[5] + "/1 runner team blue</color>\n";
        else textCheckMap.text += "<color=blue>" + nbObjectsByType[5] + "/1 runner team blue</color>\n";

        if (nbObjectsByType[6] < 2) textCheckMap.text += "<color=red>" + nbObjectsByType[6] + "/2 hunters team blue</color>\n";
        else textCheckMap.text += "<color=blue>" + nbObjectsByType[6] + "/2 hunters team blue</color>\n";

        if (nbObjectsByType[7] < 4) textCheckMap.text += "<color=red>" + nbObjectsByType[7] + "/4 guardians team blue</color>\n";
        else textCheckMap.text += "<color=blue>" + nbObjectsByType[7] + "/4 guardians team blue</color>\n";

        if (nbObjectsByType[8] < 3) textCheckMap.text += "<color=red>" + nbObjectsByType[8] + "/3 balls</color>\n";
        else textCheckMap.text += "<color=blue>" + nbObjectsByType[8] + "/3 balls</color>\n";
    }

}
