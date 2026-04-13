using UnityEngine;
using UnityEngine.UI;

public class CMJCountController : MonoBehaviour
{
    public InputField inputField;

    public int min = 1;
    public int max = 10;

    int currentValue = 1;

    void Start()
    {
        SetValue(1); // 기본값
    }

    // + 버튼
    public void OnClickPlus()
    {
        if (currentValue < max)
        {
            currentValue++;
            UpdateUI();
        }
    }

    // - 버튼
    public void OnClickMinus()
    {
        if (currentValue > min)
        {
            currentValue--;
            UpdateUI();
        }
    }

    // 직접 입력 대응 (선택)
    public void OnValueChanged()
    {
        int value;

        if (int.TryParse(inputField.text, out value))
        {
            value = Mathf.Clamp(value, min, max);
            currentValue = value;
        }
        else
        {
            currentValue = min;
        }

        UpdateUI();
    }

    void SetValue(int value)
    {
        currentValue = Mathf.Clamp(value, min, max);
        UpdateUI();
    }

    void UpdateUI()
    {
        inputField.text = currentValue.ToString();
    }

    // 다른 스크립트에서 값 가져갈 때
    public int GetValue()
    {
        return currentValue;
    }
}