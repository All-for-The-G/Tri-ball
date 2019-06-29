using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridEditor : MonoBehaviour
{
    [SerializeField] private string mapFolder;
    [SerializeField] private Grid grid;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider widthSlider;

    private Hexagon.TileType activeType;
    private string saveFileName;
    private string loadFileName;

    private readonly string saveFileVersion = "0.1";

    private void Awake () 
    {
        SelectType(0);
    }

    private void Update () 
    {
        if (Input.GetMouseButton(0) &&
            !EventSystem.current.IsPointerOverGameObject()) 
        {
            HandleInput();
        }
    }

    private void HandleInput () 
    {
        Ray inputRay = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(inputRay, out hit)) 
        {
            grid.ChangeCellType(hit.point, activeType);
        }
    }

    public void SelectType (int index) 
    {
        activeType = (Hexagon.TileType) index;
    }

    public void UpdateSaveFileName (string filename)
    {
        saveFileName = filename;
    }

    public void UpdateLoadFileName (string filename)
    {
        loadFileName = filename;
    }

    public void UpdateBlankGrid ()
    {
        int height = (int)heightSlider.value;
        int width = (int)widthSlider.value;
        grid.GenerateEmptyGrid(height, width);
    }

    public void SaveGrid ()
    {
        if (saveFileName == null)
        {
            saveFileName = "MyArena";
        }
        
        //TODO: confirm that this works on Linux and MacOS. Pesky slashes.
        string directoryPath = Path.Combine(Application.persistentDataPath, mapFolder);
        Directory.CreateDirectory(directoryPath);
        string path = Path.Combine(directoryPath, saveFileName+".xml");
        using (XmlWriter xmlWriter = XmlWriter.Create(path))
        {
            xmlWriter.WriteStartDocument();
            xmlWriter.WriteStartElement("data");
            xmlWriter.WriteStartElement("save-version");
            xmlWriter.WriteAttributeString("v", saveFileVersion);
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("grid");
            xmlWriter.WriteStartElement("dimensions");
            xmlWriter.WriteAttributeString("height", grid.Height.ToString());
            xmlWriter.WriteAttributeString("width", grid.Width.ToString());
            xmlWriter.WriteEndElement();
            xmlWriter.WriteStartElement("hexagons");
            foreach (Hexagon hexagon in grid.Cells)
            {
                int type = (int)hexagon.HexagonData.Type;
                xmlWriter.WriteStartElement("hexagon");
                xmlWriter.WriteAttributeString("type", type.ToString());
                xmlWriter.WriteAttributeString("x", hexagon.HexagonData.Coordinates.X.ToString());
                xmlWriter.WriteAttributeString("z", hexagon.HexagonData.Coordinates.Z.ToString());
                xmlWriter.WriteAttributeString("xIndex", hexagon.HexagonData.XIndex.ToString());
                xmlWriter.WriteAttributeString("zIndex", hexagon.HexagonData.ZIndex.ToString());
                xmlWriter.WriteEndElement();
            }
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndElement();
            xmlWriter.WriteEndDocument();
        }
    }

    public void LoadGrid ()
    {
        string directoryPath = Path.Combine(Application.persistentDataPath, mapFolder);
        Directory.CreateDirectory(directoryPath);
        string path = Path.Combine(directoryPath, loadFileName+".xml");

        int height = 0;
        int width = 0;
        List<HexagonData> hexagonData = new List<HexagonData>();
        
        using (XmlReader xmlReader = XmlReader.Create(path))
        {
            xmlReader.Read(); //auto generated xml crap
            xmlReader.Read(); //the outer element
            xmlReader.Read(); //the version element
            string version = xmlReader.GetAttribute("v");
            if (version != saveFileVersion)
            {
                throw new Exception(loadFileName+".xml is of an incompatible version! " + version );
            }
            while (xmlReader.Read())
            {
                if (xmlReader.IsStartElement())
                {
                    switch (xmlReader.Name)
                    {
                        case "dimensions":
                            Int32.TryParse(xmlReader["height"], out height);
                            Int32.TryParse(xmlReader["width"], out width);
                            break;
                        case "hexagon":
                            int type, x, z, xIndex, zIndex;
                            
                            Int32.TryParse(xmlReader["type"], out type);
                            Int32.TryParse(xmlReader["x"], out x);
                            Int32.TryParse(xmlReader["z"], out z);
                            Int32.TryParse(xmlReader["xIndex"], out xIndex);
                            Int32.TryParse(xmlReader["zIndex"], out zIndex);
                            hexagonData.Add(new HexagonData(new HexCoordinates(x, z), (Hexagon.TileType) type, xIndex, zIndex));
                            break;
                    }
                }
            }
        }
        
        grid.GenerateGrid(height, width, hexagonData);
    }
}
