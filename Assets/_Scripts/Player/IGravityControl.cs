using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IGravityControl
{
    void ActivateGravity();
    void DeactivateGravity();
    bool IsGravityActive();
}
