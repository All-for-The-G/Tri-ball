using System.Xml;
using System.Xml.Serialization;

public class Vector3Save
{
    [XmlAttribute("x")]
    public float x;
    [XmlAttribute("y")]
    public float y;
    [XmlAttribute("z")]
    public float z;

    public Vector3Save(UnityEngine.Vector3 v)
    {
        this.x = v.x;
        this.y = v.y;
        this.z = v.z;
    }

    public Vector3Save()
    {

    }

    public UnityEngine.Vector3 toVector3()
    {
        return new UnityEngine.Vector3(x, y, z);
    }

    public UnityEngine.Vector3 toVector3InvertYAndZ()
    {
        return new UnityEngine.Vector3(x, z, y);
    }

}