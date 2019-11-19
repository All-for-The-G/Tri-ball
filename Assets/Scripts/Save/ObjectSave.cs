using System.Xml;
using System.Xml.Serialization;

public class ObjectSave
{
    [XmlAttribute("name")]
    public string name;


    public Vector3Save pos;


    public Vector3Save angles;


    public Vector3Save size;

    [XmlAttribute("hexagonX")]
    public int hexagonX;

    [XmlAttribute("hexagonZ")]
    public int hexagonZ;

    public ObjectSave()
    {

    }

    public ObjectSave(ObjectDeco objectDeco)
    {
        this.name = objectDeco.name;
        this.pos = new Vector3Save(objectDeco.gameObject.transform.localPosition);
        this.angles = new Vector3Save(objectDeco.gameObject.transform.localEulerAngles);
        this.size = new Vector3Save(objectDeco.gameObject.transform.localScale);
        this.hexagonX = objectDeco.hexagonX;
        this.hexagonZ = objectDeco.hexagonZ;
    }
}