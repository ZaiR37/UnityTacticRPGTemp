using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    private const int ACTION_POINTS_MAX = 2;

    public static event EventHandler OnAnyActionPointsChanged;

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

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
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

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction){
        return (actionPoints >= baseAction.GetActionPointsCost());
    }
        
    private void SpendActionPoints(int amount){
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e){
        actionPoints = ACTION_POINTS_MAX;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction){
        if (!CanSpendActionPointsToTakeAction(baseAction)) return false;
    
        SpendActionPoints(baseAction.GetActionPointsCost());
        return true; 
    }
}
