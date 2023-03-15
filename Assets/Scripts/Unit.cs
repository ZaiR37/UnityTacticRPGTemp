using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Unit : MonoBehaviour
{
    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;

    [SerializeField] private bool isEnemy;

    private GridPosition gridPosition;
    private HealthSystem healthSystem;
    private MoveAction moveAction;
    private ShootAction shootAction;
    private SpinAction spinAction;
    private BaseAction[] baseActionArray;
    
    private int actionPoints = 2;
    [SerializeField] private int ACTION_POINTS_MAX = 2;

    private void Awake(){
        healthSystem = GetComponent<HealthSystem>();
        moveAction = GetComponent<MoveAction>();
        spinAction = GetComponent<SpinAction>();
        shootAction = GetComponent<ShootAction>();
        baseActionArray = GetComponents<BaseAction>();
    }

    private void Start(){
        actionPoints = ACTION_POINTS_MAX;

        gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);

        LevelGrid.Instance.AddUnitAtGridPosition(gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystem_OnTurnChanged;
        healthSystem.OnDead += HealthSystem_OnDead;

        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update(){
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        
        if (newGridPosition != gridPosition){
            GridPosition oldGridPosition = gridPosition;
            gridPosition = newGridPosition;
            
            LevelGrid.Instance.UnitMovedGridPosition(this, oldGridPosition, newGridPosition); // Unit changed Grid Position
        }
    }

    public void Damage(int damageAmount){
        healthSystem.Damage(damageAmount);
    }

    public bool CanSpendActionPointsToTakeAction(BaseAction baseAction){
        return (actionPoints >= baseAction.GetActionPointsCost());
    }
        
    private void SpendActionPoints(int amount){
        actionPoints -= amount;
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool TrySpendActionPointsToTakeAction(BaseAction baseAction){
        if (!CanSpendActionPointsToTakeAction(baseAction)) return false;
    
        SpendActionPoints(baseAction.GetActionPointsCost());
        return true; 
    }

    private void TurnSystem_OnTurnChanged(object sender, EventArgs e){
        if(IsEnemy() && !TurnSystem.Instance.IsPlayerTurn() ||
            (!IsEnemy()) && TurnSystem.Instance.IsPlayerTurn())
        {
            actionPoints = ACTION_POINTS_MAX;
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private void HealthSystem_OnDead(object sender, EventArgs e){
        LevelGrid.Instance.RemoveUnitAtGridPosition(gridPosition, this);
        Destroy(gameObject);

        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }

    public MoveAction GetMoveAction() => moveAction;
    public SpinAction GetSpinAction() => spinAction;
    public ShootAction GetShootAction() => shootAction;
    public GridPosition GetGridPosition() => gridPosition;
    public BaseAction[] GetBaseActionArray() => baseActionArray;
    public Vector3 GetWorldPosition() => transform.position;

    public int GetActionPoints() => actionPoints;
    public bool IsEnemy() => isEnemy;
    public float GetHealthNormalized() => healthSystem.GetHealthNormalized();
    
    
}
