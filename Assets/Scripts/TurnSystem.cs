using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class TurnSystem : MonoBehaviour
{
    public static TurnSystem Instance { get; private set;}

    public event EventHandler OnTurnChanged;

    private int turnNumber = 1;
    private bool isPlayerTurn = true;

    private void Awake() {
        if (Instance != null){
            Debug.LogError("There's more than one TurnSystem! " + transform + " - " + Instance);
            Destroy(gameObject);
            return;
        }        
        Instance = this;
    }

    public void NextTurn(){
        if(UnitActionSystem.Instance.IsBusy()) return;
        isPlayerTurn = !isPlayerTurn;
        if(isPlayerTurn) turnNumber++;
        OnTurnChanged?.Invoke(this, EventArgs.Empty);
    }

    public bool IsPlayerTurn() => isPlayerTurn;
    public int GetTurnNumber() => turnNumber;
}
