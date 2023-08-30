using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Collectible : MonoBehaviour, ICollectible
{
    [SerializeField] float _floatingRange= 0.25f;
    [SerializeField] float _floatingTime = 2f;
    float _offset = 0;
    float _startYPos;
    Sequence _mySequence;
    private void Awake()
    {
        _startYPos = transform.position.y;
        _offset = transform.position.x/2 % _floatingTime;
        if (_offset < 0) _offset += _floatingTime;
    }

    public virtual void Collect()
    {
        _mySequence.Kill();
        Destroy(this.gameObject);
    }
    private void Start()
    {
        Invoke("SetupSequence", _offset);
    }
    void SetupSequence()
    {
        _mySequence = DOTween.Sequence();
        _mySequence.Append(transform.DOMoveY(_startYPos + _floatingRange, _floatingTime / 2).SetEase(Ease.Linear));
        _mySequence.Append(transform.DOMoveY(_startYPos - _floatingRange, _floatingTime).SetEase(Ease.Linear));
        _mySequence.Append(transform.DOMoveY(_startYPos, _floatingTime / 2).SetEase(Ease.Linear));
        _mySequence.OnComplete(RestartSequence);
    }
    void RestartSequence() 
    {
        _mySequence.Restart();
    }
}
