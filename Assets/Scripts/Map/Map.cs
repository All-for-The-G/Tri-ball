using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Map
{
    public int width;
    public int height;
    public Hexagon[,] hexagons;
    public string lightSettingMap;
    public int environment;
    public List<ObjectDeco> objects;

    public Map(int width, int height, string lightSettingMap)
    {
        this.width = width;
        this.height = height;
        this.hexagons = new Hexagon[width, height];
        this.lightSettingMap = lightSettingMap;
        this.environment = 0;
        GenerateHexagons(0);

        objects = new List<ObjectDeco>();
    }

    public Map(int width, int height, string lightSettingMap, int type)
    {
        this.width = width;
        this.height = height;
        this.hexagons = new Hexagon[width, height];
        this.lightSettingMap = lightSettingMap;
        this.environment = type;
        GenerateHexagons(type);

        objects = new List<ObjectDeco>();
    }

    public Map (SaveContainerMap saveContainer)
    {
        this.width = saveContainer.width;
        this.height = saveContainer.height;
        this.environment = saveContainer.environment;
        this.lightSettingMap = saveContainer.lightSettingMap;

        this.hexagons = new Hexagon[this.width, this.height];
        for (int i = 0; i < this.width; i++)
        {
            for (int j = 0; j < this.height; j++)
            {
                this.hexagons[i, j] = new Hexagon(saveContainer.hexagons[i * this.height + j]);
            }
        }

        objects = new List<ObjectDeco>();
        for (int i = 0; i < saveContainer.objects.Length; i++)
        {
            objects.Add(new ObjectDeco(saveContainer.objects[i]));
        }
    }

    void GenerateHexagons(int type)
    {
        for(int i=0; i< width; i++)
        {
            for (int j = 0; j < height; j++)
            {
                this.hexagons[i, j] = new Hexagon(i, j, type);
            }
        }
    }

    public int GetNbHexagons()
    {
        return width * height;
    }

    public void AddObjectDeco(GameObject decoGO, string name)
    {
        ObjectDeco objectDeco = new ObjectDeco(decoGO, name);
        this.objects.Add(objectDeco);
    }

    public void RemoveFromDeco(GameObject gameObjectToDelete)
    {
        ObjectDeco objectDecoToRemove = null;
        foreach(ObjectDeco objectDeco in objects)
        {
            if (objectDeco.gameObject == gameObjectToDelete)
            {
                objectDecoToRemove = objectDeco;
                break;
            }
        }
        if (objectDecoToRemove != null) this.objects.Remove(objectDecoToRemove);
    }

    public bool CanAddGamePlayObject ( ObjectMap objectToAdd, bool isMovingPosition)
    {
        if (objectToAdd.typeObjectGamePlay == 2)
        {
            int nbRunnerA = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 2)  == 1) nbRunnerA++;
                }
            }
            if (nbRunnerA >= 1 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There is already 1 runner of this team on the map.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 5)
        {
            int nbRunnerB = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 5) == 1) nbRunnerB++;
                }
            }
            if (nbRunnerB >= 1 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There is already 1 runner of this team on the map.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 3)
        {
            int hunterA = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 3) == 1) hunterA++;
                }
            }
            if (hunterA >= 2 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There is already 2 hunters of this team on the map.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 6)
        {
            int hunterB = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 6) == 1) hunterB++;
                }
            }
            if (hunterB >= 2 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There is already 2 hunters of this team on the map.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 4)
        {
            int guardianA = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 4) == 1) guardianA++;
                }
            }
            if (guardianA >= 4 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There is already 4 guardian of this team on the map.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 7)
        {
            int guardianB = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 7) == 1) guardianB++;
                }
            }
            if (guardianB >= 4 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There is already 4 guardian of this team on the map.");
                return false;
            }
        }
        else if (objectToAdd.typeObjectGamePlay == 8)
        {
            int nbBalls = 0;

            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> 8) == 1) nbBalls++;
                }
            }
            if(nbBalls>=3 && !isMovingPosition)
            {
                ErrorEditor._instance.SetError("There are already 3 balls on the map.");
                return false;
            }
        }
        return true;
    }

    public int[] GetNbByObjectType()
    {
        int nbObjectsMax = 9;
        int[] objectByType = new int[nbObjectsMax];

        for(int typeObject =0; typeObject< nbObjectsMax; typeObject++)
        {
            for (int i = 0; i < this.width; i++)
            {
                for (int j = 0; j < this.height; j++)
                {
                    if ((0x1 & this.hexagons[i, j].typeGamePlay >> typeObject) == 1) objectByType[typeObject]++;
                }
            }
        }
        return objectByType;

    }


}
