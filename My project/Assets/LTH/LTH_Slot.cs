using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class LTH_Slot : MonoBehaviour
{
    public ItemData itemData;
    public int currentCount;

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI countText;

    // 슬롯 내용 업데이트 및 UI 반영
    public void UpdateSlot(ItemData newItem, int amount)
    {
        itemData = newItem;
        currentCount = amount;
        iconImage.sprite = itemData.itemIcon;
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
        // countText가 연결되어 있는지 반드시 확인 후 실행
        if (countText != null)
        {
            // 수량이 1개 이하이거나 아이템 타입이 장비라면 숫자를 숨김
            if (currentCount <= 1 || (itemData != null && itemData.itemType == SMS_ItemType.Equipment))
            {
                countText.gameObject.SetActive(false);
            }
            else
            {
                countText.gameObject.SetActive(true);
                countText.text = currentCount.ToString();
            }
        }
    }
}