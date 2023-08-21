using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CanvasGroup _endPanel;
    private void Start()
    {
        PlayerCombat.OnPlayerKilled += LevelFailed;
    }
    void LevelFailed()
    {
        _endPanel.DOFade(1, 1f).OnComplete( () => { DOTween.KillAll(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
    }
}
