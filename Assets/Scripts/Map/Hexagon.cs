using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon
{
    public int posX;
    public int posZ;
    public int type;
    public int subType;
    public GameObject hexagonGO;
    
    //0: free, 1: blocked, 2:runner 1, 3: hunter 1, 4: guardian 1, 5: runner 2, 6: hunter 2, 7: guardian 2, 8 : ball
    public int typeGamePlay;

    public Hexagon(int posX, int posZ, int type)
    {
        this.posX = posX;
        this.posZ = posZ;
        this.type = type;
        this.subType = 0;
        this.typeGamePlay = 0;

    }
    public Hexagon(int posX, int posZ, int type, int typeGamePlay)
    {
        this.posX = posX;
        this.posZ = posZ;
        this.type = type;
        this.subType = 0;
        this.typeGamePlay = typeGamePlay;
    }

    public Hexagon(HexagonSave hexagonSave)
    {
        this.posX = hexagonSave.posX;
        this.posZ = hexagonSave.posZ;
        this.type = hexagonSave.type;
        this.subType = hexagonSave.subType;
        this.typeGamePlay = hexagonSave.typeGamePlay;
    }

    public void AddGamePlayObject(ObjectMap objectToAdd)
    {

        typeGamePlay |= 1 << objectToAdd.typeObjectGamePlay;


    }

    public void RemoveGamePlayObject(ObjectMap objectToAdd)
    {

        typeGamePlay ^= 1 << objectToAdd.typeObjectGamePlay;
    }

    public bool CanAddGamePlayObject(ObjectMap objectToAdd)
    {
        if(objectToAdd.typeObjectGamePlay>=2 && objectToAdd.typeObjectGamePlay <= 7)
        {
            if ((0x1 & typeGamePlay >> 1) == 1)
            {
                ErrorEditor._instance.SetError("Can't add a character on bloked tile.");
                return false;
            }


            bool hasAlreadyCharacterOnTile = false;
            for (int i = 2; i < 8; i++)
            {
                
                if ((0x1 & (typeGamePlay >> i)) == 1)
                {
                    hasAlreadyCharacterOnTile = true;
                }
            }

            if(hasAlreadyCharacterOnTile)
            {
                ErrorEditor._instance.SetError("There is already a character on this tile.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 8)
        {
            if((0x1 & typeGamePlay >> 1)==1)
            {
                ErrorEditor._instance.SetError("Can't add a ball on bloked tile.");
                return false;
            }
            else if ((0x1 & typeGamePlay >> 8) == 1)
            {
                ErrorEditor._instance.SetError("There is already a ball on this tile.");
                return false;
            }
        }
        else if(objectToAdd.typeObjectGamePlay == 1)
        {
            for (int i = 2; i <= 8; i++)
            {

                if ((0x1 & (typeGamePlay >> i)) == 1)
                {
                    ErrorEditor._instance.SetError("You can't block a tile with something on it.");
                    return false;
                }
            }

            if ((0x1 & (typeGamePlay >> 1)) == 1)
            {
                
                return false;
            }
        }
        return true;
    }
}
