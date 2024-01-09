using System;
using TMPro;
using UnityEngine;

public class UIView : MonoBehaviour
{
    [SerializeField] private Canvas StartGameCanvas;
    [SerializeField] private TextMeshProUGUI EnemiesCounterText;

    private int _enemiesCounter;
    public Action StartGamePressed;

    public void OnStartGamePressed()
    {
        StartGamePressed?.Invoke();
        SetStartGameCanvasEnabled(false);
    }
    
    public void SetStartGameCanvasEnabled(bool status) => StartGameCanvas.enabled = status;

    public void AddEnemyToCounter(int value)
    {
        _enemiesCounter += value;
        EnemiesCounterText.SetText(_enemiesCounter.ToString());
    }

    public void ClearEnemiesCounter()
    {
        _enemiesCounter = 0;
        EnemiesCounterText.text = _enemiesCounter.ToString();
    }
}
