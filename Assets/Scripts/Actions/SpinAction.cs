using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float totalSpinAmount;
    private float maxSpin = 360f;

    private void Update() {
        if (!isActive) return;

        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0, spinAddAmount, 0);
        totalSpinAmount += spinAddAmount;

        if (totalSpinAmount >= maxSpin){
            ActionComplete();
        }
    }
    
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        totalSpinAmount = 0;
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList(){
        GridPosition unitGridPosition = unit.GetGridPosition();
        
        return new List<GridPosition>{
            unitGridPosition
        };

    }
    
    public override string GetActionName() => "Spin";
}
