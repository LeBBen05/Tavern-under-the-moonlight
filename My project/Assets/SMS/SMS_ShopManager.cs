using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class SMS_ShopManager : MonoBehaviour
{
    [Header("상점 설정")]
    public List<ItemData> itemSale; //상점에서 팔 아이템 목록을 인스펙터에 넣어주는 역할

    [Header("UI연결 - 좌측 연결창")]
    bool isShopOpen = false;
    public GameObject shopUIPrefeb; //전체 상점 UI
    public Transform slotContainer; //슬롯들이 생성될 부모객체 
    public GameObject shopSlotPrefeb;   //슬롯 프리펩
    public GameObject shopExit; //상점 나가기 버튼

    [Header("UI연결 - 우측 연결창")]
    public GameObject infoPanel;
    public Text infoText;
    public Text infoPriceText;
    public Text amountText;

    [Header("버튼 연결 설정")]
    public Button increaseBtn; //수량 +
    public Button decreaseBtn; //수량 -
    public Slider amountSlider; //수량 슬라이더
    public Button finalBuyBtn;  //최종 구매 버튼


    //테스트용 소지금
    public int playerMoney = 500;
    //아이템 데이터
    ItemData selectedItem;
    int crrAmount = 1;

    // Start is called before the first frame update
    void Start()
    {
        shopUIPrefeb.SetActive(isShopOpen);  //닫아 두기

        //버튼 이벤트 연결
        increaseBtn.onClick.AddListener(() => ChangeAmount(1));
        decreaseBtn.onClick.AddListener(() => ChangeAmount(-1));
        finalBuyBtn.onClick.AddListener(ExecutePurchase);

        if (amountSlider != null)
        {
            amountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        }
        //게임 시작 시 상점 목록 생성
        InitializeShop();
    }

    // Update is called once per frame
    void Update()
    {


    }

    private void OnMouseDown()
    {
        ToggleShop();
    }

    /// <summary>
    /// 상점 목록 생성
    /// </summary>
    void InitializeShop()
    {

        foreach (Transform child in slotContainer) Destroy(child.gameObject);

        for (int i = 0; i < itemSale.Count; i++)
        {
            ItemData item = itemSale[i]; // 리스트에서 i번째 아이템을 꺼내옴

            // ItemData에서 isSell이 true인 것만 팔고 싶다면 여기서 체크!
            if (item.isSell)
            {
                GameObject newSlot = Instantiate(shopSlotPrefeb, slotContainer);
                SMS_ShopSlotUI slotUI = newSlot.GetComponent<SMS_ShopSlotUI>();
                if (slotUI != null)
                {
                    slotUI.SetUpSlot(item, this);
                }
            }
        }
    }


    /// <summary>
    /// shop을 클릭하면 shopUI가 나타난다
    /// </summary>
    void ToggleShop()
    {
        isShopOpen = true;
        shopUIPrefeb.SetActive(isShopOpen);
        infoPanel.SetActive(false);

        if (isShopOpen)
        {
            Time.timeScale = 0f;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }


    }

    /// <summary>
    /// 상점에서 나가게 하는 버튼 함수
    /// </summary>
    public void ToggleShopExit()
    {
        isShopOpen = false;
        shopUIPrefeb.SetActive(isShopOpen);

        if (Time.time > 1.0f)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
            Time.timeScale = 1f;    //시간이 다시 돌아가게 함
        }


    }

    //////////////////////////////////////////////////////////
    ///오른쪽 정보 관련 로직
    //////////////////////////////////////////////////////////

    /// <summary>
    /// 슬롯을 클릭 했을 때 나타나는 오른쪽 정보창 킴
    /// </summary>
    /// <param name="item"></param>
    public void ShowItemInfo(ItemData item)
    {
        selectedItem = item;
        crrAmount = 1;

        //현재 돈으로 살 수 있는 최대 수량 계산(돈/가격)
        int maxAffordable = playerMoney / item.buyPrice;
        if (maxAffordable < 1) maxAffordable = 1;

        //슬라이더 기본 세팅 초기화
        if (amountSlider != null)
        {
            amountSlider.minValue = 1;
            amountSlider.maxValue = maxAffordable;
            amountSlider.value = 1;
        }

        infoPanel.SetActive(true);
        UpdateInfoUI(); //UI 글씨/그림 갱신
    }

    void ChangeAmount(int change)
    {
        if (selectedItem == null) return;   //아이템 버튼을 눌르지 않은 경우 x

        //현재 돈으로 살 수 있는 최대 개수 계산
        int maxAffordable = playerMoney / selectedItem.buyPrice;
        if (maxAffordable < 1) maxAffordable = 1;   //0개 구매 방지

        crrAmount += change;

        crrAmount = Mathf.Clamp(crrAmount, 1, maxAffordable);

        //버튼 조작과 슬라이더의 위치도 맞춰줌
        if (amountSlider != null)
        {
            amountSlider.value = crrAmount;
        }

        UpdateInfoUI();
    }
    //슬라이더의 핸들을 마우스로 드래그 했을 때 실행
    void OnSliderValueChanged(float value)
    {
        if (selectedItem == null) return;

        crrAmount = (int)value; //소수점을 정수로 변환
        UpdateInfoUI();
    }
    /// <summary>
    /// 정보창의 텍스트(가격, 수량)을 최신 상대로 바꿈
    /// </summary>
    void UpdateInfoUI()
    {
        if (selectedItem == null) return;

        infoText.text = selectedItem.ItemInfo;
        amountText.text = crrAmount.ToString();

        int totalPrice = selectedItem.buyPrice * crrAmount;
        infoPriceText.text = $"총 가격: {totalPrice} 전";
    }

    /// <summary>
    /// 최종 구매 처리 시스템
    /// </summary>
    void ExecutePurchase()
    {
        if (selectedItem == null) return;

        int totalPrice = selectedItem.buyPrice * crrAmount;

        if (playerMoney >= totalPrice)
        {
            playerMoney -= totalPrice;
            int totalGainAmount = crrAmount * selectedItem.buyAmount;

            if (LTH_InventoryManager.Instance != null && selectedItem != null)
            {
                LTH_InventoryManager.Instance.AddItem(selectedItem, totalGainAmount);
                Debug.Log($"{selectedItem.itemName} {totalGainAmount}개 구매 완료! 남은 돈: {playerMoney}");
            }
            else
            {
                Debug.LogError("인벤토리 에러 발생!!");
            }

            //물건을 사고 남은 돈으로 살 수 있는 최대 수량을 다시 계산해서 슬라이더 갱신
            int newMaxAffordable = playerMoney / selectedItem.buyPrice;
            if (newMaxAffordable < 1) newMaxAffordable = 1;

            if (amountSlider != null)
            {
                amountSlider.maxValue = newMaxAffordable;
            }

            //구매 후 변경된 최대 수량
            crrAmount = Mathf.Clamp(crrAmount, 1, newMaxAffordable);
            if (amountSlider != null) amountSlider.value = crrAmount;

            UpdateInfoUI();
        }
        else Debug.Log("$살 수 있는 돈이 부족합니다.");
    }
}
