using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
public class SpinningEnemy : Enemy
{
    [SerializeField] float movementDuration=2f;
    [SerializeField] float idleTime;
    [SerializeField] Transform target;
    Vector3 startTarget;
    float numberOfSpins = 1;
    bool isActive = true;
    protected override void Awake()
    {
        base.Awake();
        startTarget = transform.position;
    }
    protected void Start()
    {
        numberOfSpins = Mathf.Ceil(Vector2.Distance(target.position, startTarget)/5);
        StartCoroutine(Move());
    }
    IEnumerator Move()
    {
        while (isActive)
        {
            yield return new WaitForSeconds(idleTime);
            transform.DOMove(target.position, movementDuration).SetEase(Ease.Linear);
            transform.eulerAngles = Vector3.zero;
            DOTween.To(() => transform.eulerAngles, x => transform.eulerAngles = x, new Vector3(0,0,360*numberOfSpins),movementDuration).SetEase(Ease.Linear);
            yield return new WaitForSeconds(movementDuration);
            yield return new WaitForSeconds(idleTime);
            transform.eulerAngles = Vector3.zero;
            DOTween.To(() => transform.eulerAngles, x => transform.eulerAngles = x, new Vector3(0, 0, -360*numberOfSpins), movementDuration).SetEase(Ease.Linear);
            yield return transform.DOMove(startTarget, movementDuration).SetEase(Ease.Linear);
            yield return new WaitForSeconds(movementDuration);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        if (target != null)
        {
            Gizmos.DrawRay(transform.position, target.position - transform.position);
        }

    }

}
