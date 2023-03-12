using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set;}

    [Serializable]
    public struct GridVisualTypeMaterial{
        public GridVisualType gridVisualType;
        public Material material;
    }

    public enum GridVisualType{
        White, Blue, Red, RedSoft, Yellow
    }

    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
 
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

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType){
        List<GridPosition> gridPositionList = new List<GridPosition>();

        for (int x = -range; x <= range; x++){
            for (int z = -range; z <= range; z++){
                GridPosition testGridPosition =  gridPosition + new GridPosition(x, z);

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > range) continue;

                gridPositionList.Add(testGridPosition);
            }
        }
        
        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList, GridVisualType gridVisualType){
        foreach(GridPosition gridPosition in gridPositionList){
            gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    public void UpdateGridVisual(){
        HideAllGridPosition();

        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();

        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType = GridVisualType.White;

        switch (selectedAction){
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case ShootAction shootAction:
                gridVisualType = GridVisualType.Red;

                ShowGridPositionRange(selectedUnit.GetGridPosition(),shootAction.GetMaxShootDistance(), GridVisualType.RedSoft);
                break;

            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
        }

        ShowGridPositionList(
            selectedAction.GetValidActionGridPositionList(), gridVisualType
        );
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType){
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList){
            if (gridVisualTypeMaterial.gridVisualType != gridVisualType) continue;
            return gridVisualTypeMaterial.material;
        }
        Debug.LogError("Could not find GridVisualTypeMaterial for GridVisualType " + gridVisualType);
        return null;
    }

    public void UnitActionSystem_OnSelectedActionChanged (object sender, EventArgs e) => UpdateGridVisual();
    public void LevelGrid_OnAnyUnitMoveGridPosition (object sender, EventArgs e) => UpdateGridVisual();

}
