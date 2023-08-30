using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IKillableByJump
{
    public void TakeHitFromJump(int damage);
}
