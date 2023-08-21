using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
public class ScoreManager : MonoBehaviour
{
    int _score = 0;
    [SerializeField] TMP_Text _scoreTxt;
    [SerializeField] List<GameObject> _lifeVisuals;

    private void Start()
    {
        Enemy.OnEnemyKilled += UpdateScore;
        Gem.OnGemCollected += UpdateScore;
        PlayerCombat.OnPlayerLifeChanged += UpdateLife;
    }
    void UpdateScore(int gainedScore)
    {
        _score += gainedScore;
        _scoreTxt.text = $"{_score}";
    }
    void UpdateLife(int currentLife)
    {
        for(int i=0; i < _lifeVisuals.Count; i++)
        {
            if (i < currentLife) _lifeVisuals[i].SetActive(true);
            else _lifeVisuals[i].SetActive(false);
        }
    }
    private void OnDestroy()
    {
        Enemy.OnEnemyKilled -= UpdateScore;
        Gem.OnGemCollected -= UpdateScore;
        PlayerCombat.OnPlayerLifeChanged -= UpdateLife;
    }
}
