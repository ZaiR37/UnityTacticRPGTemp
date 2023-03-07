using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveAction : BaseAction
{

    [SerializeField] private Animator unitAnimator;
    [SerializeField] private int maxMoveDistance = 4;

    private float stoppingDistance = .05f;
    private float moveSpeed = 4f;
    private float rotateSpeed = 15f;

    private Vector3 targetPosition;

    protected override void Awake(){
        base.Awake();
        targetPosition = transform.position;
    }

    private void Update(){
        
        if(!isActive) return;
        
        Vector3 moveDirection = (targetPosition - transform.position).normalized;

        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance){
            unitAnimator.SetBool("IsWalking", true);

            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
        else{
            unitAnimator.SetBool("IsWalking", false);
            isActive = false;
        }

        transform.forward = Vector3.Lerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }


    public void Move(GridPosition gridPosition){
        this.targetPosition = LevelGrid.Instance.GetWorldPosition(gridPosition);
        isActive = true;
    }

    public bool IsValidActionGridPosition(GridPosition gridPosition){
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(){
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxMoveDistance; x <= maxMoveDistance; x++){
            for (int z = -maxMoveDistance; z <= maxMoveDistance; z++){
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)){
                    continue;
                }

                if (unitGridPosition == testGridPosition){
                    // Same Grid Position where the unit is already at
                    continue;
                }

                if (LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)){
                    // Grid Position already occupied with another Unit
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

}
