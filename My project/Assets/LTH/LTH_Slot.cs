using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LTH_Slot : MonoBehaviour
{
    public LTH_ItemData itemData;
    public int currentCount;

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI countText;

    // 슬롯 내용 업데이트 및 UI 반영
    public void UpdateSlot(LTH_ItemData newItem, int amount)
    {
        itemData = newItem;
        currentCount = amount;
        iconImage.sprite = itemData.icon;
        RefreshUI();
    }

    // 수량 변경 (음수 입력 시 감소)
    public void ChangeCount(int amount)
    {
        currentCount += amount;

        if (currentCount <= 0)
        {
            // 인벤토리 매니저 리스트에서 먼저 제거 후 파괴
            LTH_InventoryManager.Instance.RemoveSlotFromList(this);
            Destroy(gameObject);
        }
        else
        {
            RefreshUI();
        }
    }

    private void RefreshUI()
    {
        countText.text = currentCount.ToString();
    }
}