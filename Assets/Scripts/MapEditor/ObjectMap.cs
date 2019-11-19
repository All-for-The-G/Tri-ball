using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectMap : MonoBehaviour
{
    public List<cakeslice.Outline> outlines;
    public string nameCategory;
    public string nameObject;

    public bool isGameElement;
    public int typeObjectGamePlay;

    Hexagon hexagonOnIt;

    float startSize = 0;

    public void Awake()
    {
        startSize = this.transform.localScale.x;
    }

    public float getScaleStart()
    {
        return startSize;
    }

    public void SetOutline(bool isOn)
    {
        foreach(cakeslice.Outline outline in outlines)
        {
            outline.enabled = isOn;
        }
    }

    public void SetCurrentHexagon(Hexagon hexagon)
    {
        
        this.hexagonOnIt = hexagon;
    }

    public void ChangePosition(Hexagon newHexagon)
    {

        if (this.isGameElement)
        {
            if(hexagonOnIt!=null)
            {
                hexagonOnIt.RemoveGamePlayObject(this);
            }
            newHexagon.AddGamePlayObject(this.GetComponent<ObjectMap>());
        }

        SetCurrentHexagon(newHexagon);
    }

    public void Destroy()
    {
        
        if(hexagonOnIt!=null && isGameElement)
        {
            hexagonOnIt.RemoveGamePlayObject(this);
        }

        Destroy(this.gameObject);
    }

}
