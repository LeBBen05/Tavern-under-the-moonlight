using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro; // 텍스트 매시 프로 사용 시

public class LTH_Slot : MonoBehaviour, IPointerClickHandler
{
    public ItemData itemData;
    public int currentCount;

    [Header("UI References")]
    public Image iconImage;
    public TextMeshProUGUI countText;

    // 슬롯 UI 업데이트
    public void UpdateSlot(ItemData newData, int newCount)
    {
        //데이터를 변수에 저장
        this.itemData = newData;
        this.currentCount = newCount;

        // UI 업데이트
        if (itemData != null)
        {
            if (iconImage != null)
            {
                iconImage.sprite = itemData.itemIcon;
                iconImage.gameObject.SetActive(true);
            }
            if (countText != null) countText.text = currentCount.ToString();
        }
        else
        {
            if (iconImage != null) iconImage.gameObject.SetActive(false);
            if (countText != null) countText.text = "";
        }
    }

    // 마우스 클릭 이벤트 (우클릭 시 인벤토리 이동)
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (itemData != null && IsQuickSlot())
            {
                // 인벤토리에 추가 시도
                LTH_InventoryManager.Instance.AddItem(itemData, currentCount);

                // 현재 퀵슬롯 비우기
                UpdateSlot(null, 0);
                Debug.Log($"{itemData.itemName}이(가) 인벤토리로 복귀했습니다.");
            }
        }
    }

    private bool IsQuickSlot()
    {
        if (LTH_InventoryManager.Instance == null) return false;
        foreach (var qs in LTH_InventoryManager.Instance.quickSlots)
        {
            if (qs == this) return true;
        }
        return false;
    }

    public void ChangeCount(int amount)
    {
        currentCount += amount;
        UpdateSlot(itemData, currentCount);
    }
}