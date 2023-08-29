using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

public class LevelManager : MonoBehaviour
{
    [SerializeField] CanvasGroup _endPanel;
    [SerializeField] CanvasGroup _finishPanel;
    public static LevelManager Instance;
    private void Awake()
    {
        if(Instance == null)
        Instance = this;
        else
        {
            Destroy(this);
            Debug.LogWarning($"Multiple instances of {this.name}, destroying one instance.");
        }
    }
    private void Start()
    {
        PlayerCombat.OnPlayerKilled += LevelFailed;
    }
    void LevelFailed()
    {
        _endPanel.DOFade(1, 1f).OnComplete( () => { DOTween.KillAll(); SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); });
    }
    public void LevelComplete()
    {
        _finishPanel.gameObject.SetActive(true);
        _finishPanel.DOFade(1, 1.5f);
    }
    private void OnDestroy()
    {
        PlayerCombat.OnPlayerKilled -= LevelFailed;
    }
}
