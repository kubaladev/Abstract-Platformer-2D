using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CanvasGroup))]
public class FadeUI : MonoBehaviour
{
    enum UIAction
    {
       None,
        FadeOut,
        FadeIn
    }
    CanvasGroup _canvasGroup;
    [SerializeField] float _fadeoutTime = 1f;
    [SerializeField] UIAction _action = UIAction.None;
    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }
    private void Start()
    {
        switch (_action)
        {
            case UIAction.FadeOut: FadeOut();  break;
            case UIAction.FadeIn: FadeIn(); break;
        }

    }
    public void FadeOut()
    {
        _canvasGroup.DOFade(0, _fadeoutTime);
    }
    public void FadeIn()
    {
        _canvasGroup.DOFade(1, _fadeoutTime);
    }
}
