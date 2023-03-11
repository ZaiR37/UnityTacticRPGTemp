using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{

    public event EventHandler<OnShootEventArgs> OnShoot;

    public class OnShootEventArgs : EventArgs{
        public Unit targetUnit;
        public Unit shootingUnit;
    }

    private enum State{
        Aiming,
        Shooting,
        Cooloff,
    }

    private State state;

    private float stateTimer;
    private float aimingStateTime = 1f;
    private float shootingStateTime = 0.1f;
    private float coolOfStateTime = 0.5f;
    private float rotateSpeed = 10f;

    [SerializeField] private int maxShootDistance = 4;
    [SerializeField] private int shootDamage = 40;

    private bool canShootBullet;
    private Unit targetUnit;


    private void Update() {
        if (!isActive) return;

        stateTimer -= Time.deltaTime;

        switch (state){
            case State.Aiming:
                Vector3 aimDirection = (targetUnit.GetWorldPosition() - unit.GetWorldPosition()).normalized;
                transform.forward = Vector3.Lerp(transform.forward, aimDirection, Time.deltaTime * rotateSpeed);
                break;

            case State.Shooting:
                if(!canShootBullet) break;
                canShootBullet = false;
                Shoot();
                break;

            case State.Cooloff:
                break;
        }

        if(stateTimer <= 0f) NextState();
    }

    private void NextState(){
        switch (state){
            case State.Aiming:
                state = State.Shooting;
                stateTimer = shootingStateTime;
                canShootBullet = true;
                break;

            case State.Shooting:
                state = State.Cooloff;
                stateTimer = coolOfStateTime;
                break;

            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Shoot(){
        targetUnit.Damage(shootDamage);
        OnShoot?.Invoke(this, new OnShootEventArgs{
            targetUnit = targetUnit,
            shootingUnit = unit
        });
    }



    public override List<GridPosition> GetValidActionGridPositionList(){
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = unit.GetGridPosition();

        for (int x = -maxShootDistance; x <= maxShootDistance; x++){
            for (int z = -maxShootDistance; z <= maxShootDistance; z++){

                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidGridPosition(testGridPosition)) continue;


                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);

                if(testDistance > maxShootDistance) continue;

                // Grid Position is empty, no Unit
                if (!LevelGrid.Instance.HasAnyUnitOnGridPosition(testGridPosition)) continue;

                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);

                if(targetUnit.IsEnemy() == unit.IsEnemy()) continue;

                validGridPositionList.Add(testGridPosition);
            }
        }

        return validGridPositionList;
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete){
        ActionStart(onActionComplete);

        targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        state = State.Aiming;
        stateTimer = aimingStateTime;
    }

    public override string GetActionName() => "Shoot";
}
