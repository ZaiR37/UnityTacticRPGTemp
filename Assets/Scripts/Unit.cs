using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private MoveAction moveAction;
    private SpinAction spinAction;
    private BaseAction[] baseActionArray;
    
    private int actionPoints = 2;
    [SerializeField] private int ACTION_POINTS_MAX = 2;

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

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction){
        return (actionPoints >= baseAction.GetActionPointsCost());
    }
        
    private void SpendActionPoints(int amount){
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e){

        if(IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
            (!IsEnemy()) && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction){
        if (!CanSpendActionPointsToTakeAction(baseAction)) return false;
    
        SpendActionPoints(baseAction.GetActionPointsCost());
        return true; 
    }

    public MoveAction GetMoveAction() => moveAction;
    public SpinAction GetSpinAction() => spinAction;
    public GridPosition GetGridPosition() => gridPosition;
    public BaseAction[] GetBaseActionArray() => baseActionArray;
    public Vector3 GetWorldPosition() => transform.position;

    public int GetActionPoints() => actionPoints;
    public bool IsEnemy() => isEnemy;
    
    public void Damage() => Debug.Log( transform + " damaged!");
}
