using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class EnemyFadeout : MonoBehaviour
{
    public static float fadeTime =1f;
    SpriteRenderer _spriteRenderer;
    private void Awake()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }
    private void Start()
    {
        Fadeout();
    }
    public void Fadeout()
    {
        _spriteRenderer.DOFade(0, fadeTime).OnComplete(() => Destroy(this.gameObject,0.5f));
    }
}
