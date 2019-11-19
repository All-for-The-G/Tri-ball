using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectDeco
{
    public GameObject gameObject;
    public string name;
    public int hexagonX;
    public int hexagonZ;

    public ObjectDeco (GameObject gameObject, string name)
    {
        this.name = name;
        this.gameObject = gameObject;
    }

    public ObjectDeco(ObjectSave objectSave)
    {
        this.name = objectSave.name;
        this.gameObject = GameObject.Instantiate(Resources.Load("Props/" + objectSave.name) as GameObject);
        this.gameObject.transform.localPosition = objectSave.pos.toVector3();
        this.gameObject.transform.localEulerAngles = objectSave.angles.toVector3();
        this.gameObject.transform.localScale = objectSave.size.toVector3();
        this.hexagonX = objectSave.hexagonX;
        this.hexagonZ = objectSave.hexagonZ;
    }
}
