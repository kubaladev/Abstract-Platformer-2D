using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gem : Collectible
{
    [SerializeField] int _score =1;
    public static event Action<int> OnGemCollected;
    public override void Collect()
    {
        OnGemCollected?.Invoke(_score);
        base.Collect();
    }
}
