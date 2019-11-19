using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class MapGenerator : MonoBehaviour
{
    public Transform moveContainer;

    public Transform tilesContainer;
    public Transform decoContainer;

    public string[] namesTiles;
    public int[] numberKindTiles;
    public Material[] materialKind;

    Map map;

    float valueSizeHexagon = Mathf.Sqrt(3) / 2f;

    public GameObject planeGround;
    public Light sun;

    public enum TypeMapGenerator { Editor, Game}
    public TypeMapGenerator typeMapGenerator;

    public EditorMap editorMap;


    public void Update()
    {
        if (moveContainer.localPosition.x < -this.transform.localScale.x * ((map.width - 0.5f) * 2 * valueSizeHexagon) / 2f) moveContainer.localPosition = new Vector3(-this.transform.localScale.x * ((map.width - 0.5f) * 2 * valueSizeHexagon) / 2f, 0, moveContainer.localPosition.z);
        if (moveContainer.localPosition.x > this.transform.localScale.x * ((map.width - 0.5f) * 2 * valueSizeHexagon) / 2f) moveContainer.localPosition = new Vector3(this.transform.localScale.x * ((map.width - 0.5f) * 2 * valueSizeHexagon) / 2f, 0, moveContainer.localPosition.z);

        if (moveContainer.localPosition.z < -1.5f * this.transform.localScale.x * (map.height - 1) / 2f) moveContainer.localPosition = new Vector3(moveContainer.localPosition.x, 0, -1.5f * this.transform.localScale.x * (map.height - 1) / 2f);
        if (moveContainer.localPosition.z > 1.5f * this.transform.localScale.x * (map.height - 1) / 2f) moveContainer.localPosition = new Vector3(moveContainer.localPosition.x, 0, 1.5f * this.transform.localScale.x * (map.height - 1) / 2f);

    }

    public void GenerateMap(Map map, bool init =true)
    {
        this.map = map;
        InitMap(init);

    }

    void InitMap(bool init)
    {
        planeGround.GetComponent<Renderer>().material = materialKind[map.environment];
        GenerateLightSettings(map.lightSettingMap);
        DestroyMap();
        for (int i = 0; i < map.width; i++)
        {
            for (int j = 0; j < map.height; j++)
            {
                Hexagon hexagon = map.hexagons[i, j];
                if (init)
                {
                    int randomTile = Random.Range(0, numberKindTiles[hexagon.type]);
                    hexagon.subType = randomTile;
                }
                InstanciateHexagone(map.hexagons[i, j]);
            }
        }
        foreach (ObjectDeco objectDeco in map.objects)
        {
            Vector3 localPosition = objectDeco.gameObject.transform.localPosition;
            Vector3 localEulerAngles = objectDeco.gameObject.transform.localEulerAngles;
            Vector3 localScale = objectDeco.gameObject.transform.localScale;
            objectDeco.gameObject.transform.SetParent(decoContainer);
            objectDeco.gameObject.transform.localPosition = localPosition;
            objectDeco.gameObject.transform.localEulerAngles = localEulerAngles;
            objectDeco.gameObject.transform.localScale = localScale;

            objectDeco.gameObject.GetComponent<ObjectMap>().SetCurrentHexagon(map.hexagons[objectDeco.hexagonX, objectDeco.hexagonZ]);

        }
    }

    public void InstanciateHexagone(Hexagon hexagon)
    {
        

        int tile = hexagon.subType;

        GameObject hexagonGo = GameObject.Instantiate(Resources.Load("Hexagone/" + namesTiles[hexagon.type] + "/" + tile) as GameObject, tilesContainer);
        hexagon.hexagonGO = hexagonGo;
        Vector3 positionHexagon;
        if (hexagon.posZ % 2 == 0)
        {
            positionHexagon = new Vector3(2 * valueSizeHexagon * hexagon.posX, 0, 1.5f * hexagon.posZ);
        }
        else
        {
            positionHexagon = new Vector3(2 * valueSizeHexagon * (hexagon.posX + 0.5f), 0, 1.5f * hexagon.posZ);
        }
        hexagonGo.transform.localPosition = positionHexagon + new Vector3(-((map.width - 0.5f) * 2 * valueSizeHexagon) / 2f, 0, -1.5f * (map.height - 1) / 2f);

        hexagonGo.GetComponent<HexagonController>().hexagon = hexagon;

        if (typeMapGenerator == TypeMapGenerator.Editor) hexagonGo.GetComponent<HexagonController>().editorMap = editorMap;

    }



    void DestroyMap()
    {
        for (int i = tilesContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(tilesContainer.GetChild(i).gameObject);
        }
        for (int i = decoContainer.childCount - 1; i >= 0; i--)
        {
            Destroy(decoContainer.GetChild(i).gameObject);
        }
    }

    void GenerateLightSettings(string lightSettingMap)
    {
        if (lightSettingMap == "Dawn")
        {
            RenderSettings.ambientSkyColor = new Color(1.678431f, 0.4784314f, 0.1960784f);
            RenderSettings.ambientEquatorColor = new Color(0, 0.5960785f, 1.129412f);
            RenderSettings.ambientGroundColor = new Color(1.905882f, 1.129412f, 0);
            sun.color = new Color(1f, 0.9060025f, 0.8066038f);
            sun.intensity = 0.9f;
        }
        else if (lightSettingMap == "Day")
        {
            RenderSettings.ambientSkyColor = new Color(0.8627451f, 0.8627451f, 3.435294f);
            RenderSettings.ambientEquatorColor = new Color(1.552941f, 1.552941f, 1.552941f);
            RenderSettings.ambientGroundColor = new Color(1.890196f, 1.184314f, 0.1411765f);
            sun.color = new Color(0.9811321f, 0.9541074f, 0.745105f);
            sun.intensity = 1.2f;
        }
        else if (lightSettingMap == "Night")
        {
            RenderSettings.ambientSkyColor = new Color(0.4392157f, 0.4411765f, 0.454902f);
            RenderSettings.ambientEquatorColor = new Color(0.3832707f, 0.5416614f, 0.6828016f);
            RenderSettings.ambientGroundColor = new Color(0.7917949f, 0.4731457f, 0);
            sun.color = new Color(0.3968494f, 0.4470379f, 0.9245283f);
            sun.intensity = 0.9f;
        }
    }

   

}
