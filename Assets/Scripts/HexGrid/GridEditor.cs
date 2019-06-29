using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GridEditor : MonoBehaviour
{
    [SerializeField] private Grid grid;
    [SerializeField] private Slider heightSlider;
    [SerializeField] private Slider widthSlider;

    private Hexagon.TileType activeType;

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

    public void UpdateBlankGrid()
    {
        int height = (int)heightSlider.value;
        int width = (int)widthSlider.value;
        grid.GenerateEmptyGrid(height, width);
    }
}
