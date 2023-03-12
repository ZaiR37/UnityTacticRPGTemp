using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set;}


    [SerializeField] private Transform gridSystemVisualSinglePrefab;
 
    private GridSystemVisualSingle[,] gridSystemVisualSingleArray;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There's more than one GridSystemVisual! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }        
        Instance = this;

    }

    private void Start() {
        int gridWidth = LevelGrid.Instance.GetWidth();
        int gridHeight = LevelGrid.Instance.GetHeight();

        gridSystemVisualSingleArray = new GridSystemVisualSingle[gridWidth, gridHeight];
        
        for (int x = 0; x < gridWidth; x++){
            for (int z = 0; z < gridHeight; z++){
                GridPosition gridPosition = new GridPosition(x,z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }

        UnitActionSystem.Instance.onSelectedActionChanged += UnitActionSystem_OnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGrid_OnAnyUnitMoveGridPosition;
        UpdateGridVisual();
    }

    public void HideAllGridPosition(){
        int gridWidth = LevelGrid.Instance.GetWidth();
        int gridHeight = LevelGrid.Instance.GetHeight();

        for (int x = 0; x < gridWidth; x++){
            for (int z = 0; z < gridHeight; z++){
                gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList){
        foreach(GridPosition gridPosition in gridPositionList){
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show();
        }
    }

    public void UpdateGridVisual(){
        HideAllGridPosition();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();


        ShowGridPositionList(
            selectedAction.GetValidActionGridPositionList()
        );
    }


    public void UnitActionSystem_OnSelectedActionChanged (object sender, EventArgs e) => UpdateGridVisual();
    public void LevelGrid_OnAnyUnitMoveGridPosition (object sender, EventArgs e) => UpdateGridVisual();

}
