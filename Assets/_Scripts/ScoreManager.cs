using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    int _score = 0;
    [SerializeField] TMP_Text _scoreTxt;
    private void Start()
    {
        Enemy.OnEnemyKilled += UpdateScore;
    }
    void UpdateScore(int gainedScore)
    {
        _score += gainedScore;
        _scoreTxt.text = $"{_score}";
    }
}
