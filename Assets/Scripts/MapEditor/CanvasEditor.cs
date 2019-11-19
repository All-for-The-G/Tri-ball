using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasEditor : MonoBehaviour
{
    public static bool lockInteraction = false;

    public EditorMap editorMap;

    public RectTransform panelAddObject;

    public Transform containterObject;
    public Transform containterOnglet;

    List<string> categories;
    List<List<string>> objectsCategories;

    string currentCategorie = "";

    

    void Start()
    {
        categories = new List<string>();
        categories.Add("Game");
        categories.Add("Plants");
        categories.Add("Rocks");
        categories.Add("Trees");

        objectsCategories = new List<List<string>>();

        List<string> categoryCharacters = new List<string>();
        categoryCharacters.Add("Runner Red");
        categoryCharacters.Add("Hunter Red");
        categoryCharacters.Add("Guardian Red");
        categoryCharacters.Add("Runner Blue");
        categoryCharacters.Add("Hunter Blue");
        categoryCharacters.Add("Guardian Blue");
        categoryCharacters.Add("Ball");
        categoryCharacters.Add("BlockedTile");
        objectsCategories.Add(categoryCharacters);

        List<string> categoryPlants = new List<string>();
        categoryPlants.Add("Plant 1");
        categoryPlants.Add("Plant 2");
        categoryPlants.Add("Plant 3");
        categoryPlants.Add("Mushroom 1");
        categoryPlants.Add("Mushroom 2");
        categoryPlants.Add("Mushroom 3");
        objectsCategories.Add(categoryPlants);

        List<string> categoryRocks = new List<string>();
        categoryRocks.Add("Little Rock 1");
        categoryRocks.Add("Little Rock 2");
        objectsCategories.Add(categoryRocks);

        List<string> categoryTrees = new List<string>();
        categoryTrees.Add("Tree 1");
        categoryTrees.Add("Tree 2");
        objectsCategories.Add(categoryTrees);


        InitOnglets();
    }

    private void Update()
    {
       if(currentCategorie.Length==0)
        {
            panelAddObject.anchoredPosition = new Vector2(Mathf.Lerp(panelAddObject.anchoredPosition.x, -panelAddObject.sizeDelta.x, Time.deltaTime * 3f), panelAddObject.anchoredPosition.y);
            if(editorMap.currentObjectToAdd!=null) Destroy(editorMap.currentObjectToAdd);
            editorMap.currentObjectToAdd = null;
        }
       else
        {
            panelAddObject.anchoredPosition = new Vector2(Mathf.Lerp(panelAddObject.anchoredPosition.x, 0f, Time.deltaTime * 3f), panelAddObject.anchoredPosition.y);
        }
    }

    public void InitOnglets()
    {
        for (int i = containterOnglet.childCount - 1; i >= 1; i--)
        {
            Destroy(containterOnglet.GetChild(i).gameObject);
        }

        for (int i = containterObject.childCount - 1; i >= 1; i--)
        {
            Destroy(containterObject.GetChild(i).gameObject);
        }

        GameObject template = containterOnglet.GetChild(0).gameObject;
        template.SetActive(true);
        int  j = 0;
        foreach(string category in categories)
        {
            GameObject categoryGO = GameObject.Instantiate(template, template.transform.parent);
            categoryGO.GetComponent<Image>().color = Color.gray;
            int idCategory = j;
            string categoryName = category;

            categoryGO.GetComponent<Button>().onClick.AddListener(delegate { ClickOncategory(idCategory, categoryName); });

            categoryGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            Sprite imageCategory = Resources.Load<Sprite>("Props/Categories/" + category);

            categoryGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = imageCategory;
            //categoryGO.transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio = imageCategory.;


            j++;
        }
        template.SetActive(false);
    }

    public void UpdateCategory(int idCategory)
    {
        for(int i=0; i< containterOnglet.childCount; i++)
        {
            Transform childCategoryReset = containterOnglet.GetChild(i);
            childCategoryReset.GetComponent<Image>().color = Color.gray;
            childCategoryReset.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
        }

        Transform childCategory = containterOnglet.GetChild(idCategory + 1);
        childCategory.GetComponent<Image>().color = Color.white;
        childCategory.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1f);

        for (int i = containterObject.childCount - 1; i >= 1; i--)
        {
            Destroy(containterObject.GetChild(i).gameObject);
        }

        GameObject template = containterObject.GetChild(0).gameObject;
        template.SetActive(true);
        int j = 0;
        foreach (string categoryObjectName in objectsCategories[idCategory])
        {
            GameObject categoryObjectGO = GameObject.Instantiate(template, template.transform.parent);
            categoryObjectGO.GetComponent<Image>().color = Color.gray;

            int idObject = j;
            categoryObjectGO.GetComponent<Button>().onClick.AddListener(delegate { SelectObject(idObject); });

            categoryObjectGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);

            Sprite imageCategory = Resources.Load<Sprite>("Props/"+ currentCategorie+"/"+j+ "Sprite");

            categoryObjectGO.transform.GetChild(0).GetChild(0).GetComponent<Image>().sprite = imageCategory;
            categoryObjectGO.transform.GetChild(1).GetComponent<Text>().text = categoryObjectName;
            //categoryGO.transform.GetChild(0).GetChild(0).GetComponent<AspectRatioFitter>().aspectRatio = imageCategory.;


            j++;
        }
        template.SetActive(false);

    }
    public void ClickOncategory(int idCategory, string category)
    {
        if (!lockInteraction)
        {
            if (currentCategorie == category) currentCategorie = "";
            else currentCategorie = category;
            UpdateCategory(idCategory);
        }
    }

    public void SelectObject(int idObject)
    {
        if(!lockInteraction)
        {
            if (editorMap.currentObjectToAdd != null) Destroy(editorMap.currentObjectToAdd);


            for (int i = 0; i < containterObject.childCount; i++)
            {
                Transform childCategoryReset = containterObject.GetChild(i);
                childCategoryReset.GetComponent<Image>().color = Color.gray;
                childCategoryReset.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 0.5f);
            }

            Transform childCategory = containterObject.GetChild(idObject + 1);
            childCategory.GetComponent<Image>().color = Color.white;
            childCategory.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(1, 1, 1, 1f);


            editorMap.currentObjectToAdd = GameObject.Instantiate(Resources.Load("Props/" + currentCategorie + "/" + idObject) as GameObject);
            editorMap.currentObjectToAdd.transform.position = new Vector3(0, -100f, 0);
            editorMap.currentObjectName = currentCategorie + "/" + idObject;
            editorMap.scaleFactor = 1f;
        }
        
    }

}
