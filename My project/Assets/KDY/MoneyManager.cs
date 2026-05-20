using UnityEngine;
using UnityEngine.UI;

public class MoneyManager : MonoBehaviour
{
    public static MoneyManager Instance;

    [Header("소지금 데이터")]
    // ★ [핵심 수정] 여기에 static을 붙여서 씬이 바뀌어도 데이터가 초기화되지 않고 유지되게 만듭니다!
    private static int playerMoney = 500;

    [Header("UI 연결")]
    public Text moneyText;

    public int CurrentMoney => playerMoney;

    void Awake()
    {
        // 싱글톤 세팅: 새 씬에 올 때마다 그 씬에 있는 새로운 UI를 통제하도록 인스턴스를 갱신합니다.
        Instance = this;
    }

    void Start()
    {
        // 씬이 새로 열릴 때마다, 저장되어 있던 진짜 돈 수치를 이 씬의 UI에 띄워줍니다.
        UpdateMoneyUI();
    }

    public void AddMoney(int amount)
    {
        playerMoney += amount;
        UpdateMoneyUI();
        Debug.Log($"<color=lime>[돈 획득]</color> +{amount}전 (현재 잔액: {playerMoney}전)");
    }

    public bool UseMoney(int amount)
    {
        if (playerMoney >= amount)
        {
            playerMoney -= amount;
            UpdateMoneyUI();
            Debug.Log($"<color=yellow>[돈 소비]</color> -{amount}전 (현재 잔액: {playerMoney}전)");
            return true;
        }

        Debug.LogWarning("잔액이 부족합니다!");
        return false;
    }

    public void UpdateMoneyUI()
    {
        if (moneyText != null)
        {
            moneyText.text = $"{playerMoney} 전";
        }
    }
}