using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Unit : MonoBehaviour
{

    private GridPosition gridPosition;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] baseActionArray;
    private int actionPoints = 2;

    private void Awake(){
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start(){
        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);
    }

    private void Update(){
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        if (newGridPosition != gridPosition){
            // Unit changed Grid Position
            LevelGrid.Instance.UnitMovedGridPosition(this, gridPosition, newGridPosition);
            gridPosition = newGridPosition;
        }
    }

    public MoveAction GetMoveAction() => moveAction;
    public SpinAction GetSpinAction() => spinAction;
    public GridPosition GetGridPosition() => gridPosition;
    public BaseAction[] GetBaseActionArray() => baseActionArray;
    
    public int GetActionPoints() => actionPoints;

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction) => (actionPoints >= baseAction.GetActionPointsCost());
    private void SpendActionPoints(int amount) => actionPoints -= amount;

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction){
        if (!CanSpendActionPointsToTakeAction(baseAction)) return false;
    
        SpendActionPoints(baseAction.GetActionPointsCost());
        return true; 
    }

}
