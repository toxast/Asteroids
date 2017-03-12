using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//freeze with 1 should not change the object behaviour
//freeze with 2 should freeze the object 2 times
//freeze with 0.5 should unfreeze previous call with 2.
//freeze 0 will not be called, minimum is minFreeze (0.01f)
public interface IFreezble { 
    void Freeze(float multiplier); 
}
