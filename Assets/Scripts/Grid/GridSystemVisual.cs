using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        int gridHeight = LevelGrid.Instance.GetHight();

        gridSystemVisualSingleArray = new GridSystemVisualSingle[gridWidth, gridHeight];
        for (int x = 0; x < gridWidth; x++){
            for (int z = 0; z < gridHeight; z++){
                GridPosition gridPosition = new GridPosition(x,z);
                Transform gridSystemVisualSingleTransform =
                    Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(gridPosition), Quaternion.identity);
                gridSystemVisualSingleArray[x,z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
    }

    private void Update() {
        UpdateGridVisual();
    }

    public void HideAllGridPosition(){
        int gridWidth = LevelGrid.Instance.GetWidth();
        int gridHeight = LevelGrid.Instance.GetHight();

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

        BaseAction selectedUnit = UnitActionSystem.Instance.GetSelectedAction();

        ShowGridPositionList(
            selectedUnit.GetValidActionGridPositionList()
        );
    }
}
