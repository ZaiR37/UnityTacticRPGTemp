using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitActionSystem : MonoBehaviour
{
    public static UnitActionSystem Instance { get; private set;}

    public event EventHandler onSelectedUnitChanged;
    public event EventHandler onSelectedActionChanged;
    public event EventHandler<bool> OnBusyChanged;
    public event EventHandler onActionStarted;
    
    [SerializeField] private Unit selectedUnit;
    [SerializeField] private LayerMask unitLayerMask;

    private BaseAction selectedAction;
    private bool isBusy;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There's more than one UnitActionSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }        
        Instance = this;
    }

    private void Start() {
        SetSelectedUnit(selectedUnit);
    }

    private void Update() {
        if (isBusy) return;
        if (!TurnSystem.Instance.IsPlayerTurn()) return;
        if (EventSystem.current.IsPointerOverGameObject()) return;
        if (TryHandleUnitSelection()) return;

        HandleSelectedAction();
    }

    private void HandleSelectedAction(){
        if (!Input.GetMouseButtonDown(0)) return;

        GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

        if (!selectedAction.IsValidActionGridPosition(mouseGridPosition)) return;
        if (!selectedUnit.TrySpendActionPointsToTakeAction(selectedAction)) return;

        SetBusy();
        selectedAction.TakeAction(mouseGridPosition, ClearBusy);
        onActionStarted?.Invoke(this, EventArgs.Empty);
    }

    private bool TryHandleUnitSelection(){
        if (!Input.GetMouseButtonDown(0)) return false;

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) return false;
        if (!raycastHit.transform.TryGetComponent<Unit>(out Unit unit)) return false;
        if (unit == selectedUnit) return false;
        if (unit.IsEnemy()) return false;

        SetSelectedUnit(unit);
        onActionStarted?.Invoke(this, EventArgs.Empty);
        return true;
    }

    private void SetSelectedUnit(Unit unit){
        selectedUnit = unit;
        SetSelectedAction(unit.GetMoveAction());
        onSelectedUnitChanged?.Invoke(this, EventArgs.Empty);
    }

    public void SetSelectedAction(BaseAction baseAction){
        selectedAction = baseAction;
        onSelectedActionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void SetBusy(){
        isBusy = true;
        OnBusyChanged?.Invoke(this, isBusy);
    }
    private void ClearBusy(){
        isBusy = false;
        OnBusyChanged?.Invoke(this, isBusy);
    }

    public bool IsBusy() => isBusy;
    public Unit GetSelectedUnit() => selectedUnit;
    public BaseAction GetSelectedAction() => selectedAction;
}
