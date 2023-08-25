using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IAcceptsOutsideForces 
{
    public void SetContinoiusForce(Vector2 force);
    public void UnsetContinoiusForce(Vector2 force);
    public void ApplyImmediateForce(Vector2 force, bool resetsVelocity);

    public void FollowObject(Rigidbody2D followedRB);
    public void UnFollowObject(Rigidbody2D followedRB);

}
