using System.Xml;
using System.Xml.Serialization;

public class HexagonSave
{
    [XmlAttribute("posX")]
    public int posX ;
    [XmlAttribute("posZ")]
    public int posZ;
    [XmlAttribute("type")]
    public int type;
    [XmlAttribute("subType")]
    public int subType;
    [XmlAttribute("typeGamePlay")]
    public int typeGamePlay;

    public HexagonSave(Hexagon hexagon)
    {
        this.posX = hexagon.posX;
        this.posZ = hexagon.posZ;
        this.type = hexagon.type;
        this.subType = hexagon.subType;
        this.typeGamePlay = hexagon.typeGamePlay;
    }

    public HexagonSave()
    {

    }
}