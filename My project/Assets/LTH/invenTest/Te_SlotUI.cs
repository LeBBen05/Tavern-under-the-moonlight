using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Te_SlotUI : MonoBehaviour
{
    public int slotIndex;             // 매니저 리스트의 몇 번째 데이터인지
    public Image itemIcon;            // 자식 오브젝트의 아이콘 이미지
    public TextMeshProUGUI countText; // 자식 오브젝트의 수량 텍스트

    public void UpdateSlotUI()
    {
        // 매니저에서 내 번호에 맞는 데이터를 가져옴
        InventorySlot data = Te_InventoryManager.Instance.slots[slotIndex];

        if (data.item != null)
        {
            // 아이템이 있으면 표시
            itemIcon.sprite = data.item.icon;
            itemIcon.gameObject.SetActive(true);

            // 수량이 2개 이상일 때만 숫자 표시
            if (data.count > 1)
            {
                countText.text = data.count.ToString();
                countText.gameObject.SetActive(true);
            }
            else
            {
                countText.gameObject.SetActive(false);
            }
        }
        else
        {
            // 빈 슬롯이면 모두 숨김
            itemIcon.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
        }
    }
}