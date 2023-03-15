using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{

    public event EventHandler onStartMoving;
    public event EventHandler onStopMoving;

    [SerializeField] private int maxMoveDistance = 4;

    private float stoppingDistance = .05f;
    private float moveSpeed = 4f;
    private float rotateSpeed = 15f;
    private bool stupidBug = true;

    private Vector3 targetPosition;

    protected override void Awake(){
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update(){
        
        if(!isActive) return;
        
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance){
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else{
            isActive = false;
            onStopMoving?.Invoke(this, EventArgs.Empty);
            
            ActionComplete();
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }


    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);

        onStartMoving?.Invoke(this, EventArgs.Empty);
        
        ActionStart(onActionComplete);
    }


    public override List<GridPosition> GetValidActionGridPositionList(){
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition;

        if(stupidBug){
            unitGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
            stupidBug = false;
        }
        else {
            unitGridPosition = unit.GetGridPosition();
        }

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++){
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++){
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;

                // Same Grid Position where the unit is already at
                if (unitGridPosition == testGridPosition) continue;

                // Grid Position already occupied with another Unit
                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition){
        int targetCountAtGridPosition = unit.GetAction<ShootAction>().GetTargetCountAtPosition(gridPosition);

        return new EnemyAIAction{
            gridPosition = gridPosition,
            actionValue = targetCountAtGridPosition * 10,
        };
    }


    public override string GetActionName() => "Move";

}
