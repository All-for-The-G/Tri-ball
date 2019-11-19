using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;
using System.IO;
using System.Text;



[XmlRoot("Map")]
public class SaveContainerMap
{
    [XmlAttribute("width")]
    public int width;
    [XmlAttribute("height")]
    public int height;
    [XmlAttribute("lightSettingMap")]
    public string lightSettingMap;
    [XmlAttribute("environment")]
    public int environment;

    
    [XmlArray("Tiles"), XmlArrayItem("Tile")]
    public HexagonSave[] hexagons;

    [XmlArray("Objects"), XmlArrayItem("Object")]
    public ObjectSave[] objects;

    /*Method to save all the rail networking and its environment in a XML file */
    public void Save(string path)
    {
        var serializer = new XmlSerializer(typeof(SaveContainerMap));
        using (StreamWriter stream = new StreamWriter(path, false, Encoding.GetEncoding("UTF-8")))
        {
            //XmlTextWriter xmlWriter = new XmlTextWriter(stream, System.Text.Encoding.ASCII);
            serializer.Serialize(stream, this);
        }
    }

    /*Method to load a XML file to be able to construct a rail networking and all its environment */ 
    public static SaveContainerMap Load(string path)
    {
        
            var serializer = new XmlSerializer(typeof(SaveContainerMap));
        using (var stream = new FileStream(path, FileMode.Open))
        {
            return serializer.Deserialize(stream) as SaveContainerMap;
        }
    }

    //Loads the xml directly from the given string. Useful in combination with www.text.
    public static SaveContainerMap LoadFromText(string text)
    {
        var serializer = new XmlSerializer(typeof(SaveContainerMap));
        return serializer.Deserialize(new StringReader(text)) as SaveContainerMap;
    }

    public SaveContainerMap()
    {

    }

    public SaveContainerMap(Map map)
    {
        this.width = map.width;
        this.height = map.height;
        this.environment = map.environment;
        this.lightSettingMap = map.lightSettingMap;

        this.hexagons = new HexagonSave[map.GetNbHexagons()];
        for(int i=0; i< this.width; i++)
        {
            for (int j = 0; j  < this.height; j++)
            {
                this.hexagons[i * this.height + j] = new HexagonSave(map.hexagons[i, j]);
            }
        }

        this.objects = new ObjectSave[map.objects.Count];
        for (int i=0; i<map.objects.Count; i++)
        {
            this.objects[i] = new ObjectSave(map.objects[i]);
        }
    }

}