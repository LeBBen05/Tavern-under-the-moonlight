using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Te_SlotUI : MonoBehaviour
{
    [Header("슬롯 정보")]
    public int slotIndex; // 매니저가 생성 시 부여 (0, 1, 2...)

    [Header("UI 요소 연결")]
    public Image iconImage;           // 유니티 인스펙터에서 아이콘 Image 연결
    public TextMeshProUGUI countText; // 유니티 인스펙터에서 수량 Text 연결

    public void UpdateSlotUI()
    {
        // 매니저의 slots 리스트에서 내 번호에 맞는 데이터를 가져옴
        var slotData = Te_InventoryManager.Instance.slots[slotIndex];

        // 1. 슬롯에 아이템 데이터(ItemData)가 있는지 확인
        if (slotData.item != null)
        {
            // 제공해주신 ItemData의 변수명 'itemIcon'을 사용합니다.
            iconImage.sprite = slotData.item.itemIcon;
            iconImage.gameObject.SetActive(true);

            // 2. 수량이 1보다 크면 숫자 표시, 아니면 숨김
            if (slotData.count > 1)
            {
                countText.text = slotData.count.ToString();
                countText.gameObject.SetActive(true);
            }
            else
            {
                countText.gameObject.SetActive(false);
            }
        }
        else
        {
            // 아이템이 없는 빈 슬롯 처리
            iconImage.gameObject.SetActive(false);
            countText.gameObject.SetActive(false);
        }
    }
}