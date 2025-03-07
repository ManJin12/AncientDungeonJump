using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Condition : MonoBehaviour
{
    public float curValue;
    public float maxValue;
    public float startValue;
    public float passiveValue;

    public Image uiBar;
    public TextMeshProUGUI uiText;
    
    void Start()
    {
        curValue = startValue;
    }

    
    void Update()
    {
        uiBar.fillAmount = GetBarValue();
        uiText.text = $"{curValue} / {maxValue}";
    }

    public void Add(float amount)
    {
        curValue = Mathf.Min(curValue +  amount, maxValue);
    }

    public void Subtract(float amount)
    {
        curValue = Mathf.Max(curValue - amount, 0.0f);
    }

    public float GetBarValue()
    {
        return curValue / maxValue;
    }

}
