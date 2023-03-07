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
            isActive = false;
        }
    }
    
    public void Spin(){
        isActive = true;
        totalSpinAmount = 0;
    }
}
