using UnityEngine;
using UnityEngine.UI;
using System;

public class SuperAttackButton : MonoBehaviour
{
    public static Action onEnableButton;
    public static Action onDisableButton;
    public static Action onSuperAttackPressed;

    [SerializeField] private Text timer;
    [SerializeField] private GameObject button;
    private float _timerNumber;
    private float _cooldown;

    private void Awake()
    {
        onEnableButton += EnableButton;
        onDisableButton += DisableButton;
        onSuperAttackPressed += StartNewIteration;
        _timerNumber = 0;
        _cooldown = 2f;
    }

    private void Update()
    {
        if (_timerNumber <= _cooldown)
        {
            timer.text = Convert.ToInt32(_timerNumber).ToString();
            _timerNumber += 1 * Time.deltaTime;
        }
    }

    private void EnableButton()
    {
        if (button != null)
            button.SetActive(true);
    }
    private void DisableButton()
    {
        if (button != null)
            button.SetActive(false);
    }
    private void StartNewIteration() => _timerNumber = 0;

}
