using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

[System.Serializable]
public class ItemImageData
{
    public ItemData item;
    public SMS_FishSize fishSize;
    public Image image;
}

public class CMJCookScene : MonoBehaviour
{
    [System.Serializable]
    public class RecipeResultData
    {
        public RecipeData recipe;
        public ItemData resultItem;
    }

    [Header("요리 개수 입력")]
    public CMJCountController countController;

    [Header("레시피 결과 연결")]
    public RecipeResultData[] recipeResults;

    [Header("UI")]
    public GameObject ClickMenuUI;
    public GameObject AddButton;

    [Header("메뉴판 슬롯")]
    public Text[] slotTexts;
    public Text[] MenuTexts;

    [Header("레시피 데이터")]
    public RecipeData[] recipes;

    [Header("재료 이미지 매핑")]
    public ItemImageData[] itemImages;

    [Header("레시피 슬롯 UI")]
    public Image[] recipeSlotImages;

    [Header("장사 시작 연결")]
    public Spawner spawner;      // 인스펙터에서 NPCSpawner 드래그
    public GameObject TodaysUI;  // 요리 UI 전체 부모 객체


    int selectedSlotIndex = -1;
    int currentMenuIndex = -1;

    public bool iSAliveClick = false;
    public bool isAliveAdd = false;

    //핵심: 슬롯별 데이터
    ItemData[] slotItems;
    int[] slotCounts;

    void Start()
    {
        ClearRecipeSlots();

        slotItems = new ItemData[slotTexts.Length];
        slotCounts = new int[slotTexts.Length];

        // 초기 텍스트 설정
        for (int i = 0; i < slotTexts.Length; i++)
        {
            slotTexts[i].text = "메뉴추가하기";
            MenuTexts[i].text = "메뉴추가하기";
        }
    }

    void Update()
    {
        ClickMenuUI.SetActive(iSAliveClick);
        AddButton.SetActive(isAliveAdd);

        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveMenu();
        }
    }

    public void OnClickSlot(int index)
    {
        selectedSlotIndex = index;
        iSAliveClick = true;
    }

    public void MenuClick(int index)
    {
        if (index < 0 || index >= recipes.Length) return;

        if (currentMenuIndex == index)
        {
            ClearRecipeSlots();
            currentMenuIndex = -1;
            isAliveAdd = false;
            return;
        }

        isAliveAdd = true;
        currentMenuIndex = index;

        ShowRecipe(recipes[index]);
    }

    void ShowRecipe(RecipeData recipe)
    {
        ClearRecipeSlots();

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var ing = recipe.ingredients[i];

            recipeSlotImages[i].gameObject.SetActive(true);

            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
                recipeSlotImages[i].sprite = GetItemSprite(ing.requriedItem);
            else
                recipeSlotImages[i].sprite = GetFishSprite(ing.RfishSize);
        }
    }

    void ClearRecipeSlots()
    {
        for (int i = 0; i < recipeSlotImages.Length; i++)
        {
            recipeSlotImages[i].gameObject.SetActive(false);
            recipeSlotImages[i].sprite = null;
        }
    }

    Sprite GetItemSprite(ItemData item)
    {
        foreach (var data in itemImages)
        {
            if (data.item == item && data.image != null)
                return data.image.sprite;
        }
        return null;
    }

    Sprite GetFishSprite(SMS_FishSize size)
    {
        foreach (var data in itemImages)
        {
            if (data.fishSize == size && data.image != null)
                return data.image.sprite;
        }
        return null;
    }

    int GetItemCount(ItemData item)
    {
        int count = 0;

        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == item)
                count += slot.currentCount;
        }

        return count;
    }

    int GetFishCount(SMS_FishSize size)
    {
        int count = 0;

        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            foreach (var data in itemImages)
            {
                if (data.item == null) continue;

                if (slot.itemData == data.item && data.fishSize == size)
                    count += slot.currentCount;
            }
        }

        return count;
    }

    bool CanCook(RecipeData recipe, int cookCount)
    {
        foreach (var ing in recipe.ingredients)
        {
            int need = ing.amount * cookCount;

            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
            {
                if (GetItemCount(ing.requriedItem) < need)
                    return false;
            }
            else
            {
                if (GetFishCount(ing.RfishSize) < need)
                    return false;
            }
        }
        return true;
    }

    void ConsumeIngredients(RecipeData recipe, int cookCount)
    {
        foreach (var ing in recipe.ingredients)
        {
            int need = ing.amount * cookCount;

            foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
            {
                if (slot.itemData == ing.requriedItem)
                {
                    int remove = Mathf.Min(need, slot.currentCount);
                    slot.ChangeCount(-remove);
                    need -= remove;

                    if (need <= 0) break;
                }
            }
        }
    }

    ItemData GetResultItem(RecipeData recipe)
    {
        foreach (var data in recipeResults)
        {
            if (data.recipe == recipe)
                return data.resultItem;
        }
        return null;
    }

    public void Add()
    {
        
        if (currentMenuIndex < 0 || selectedSlotIndex < 0) return;

        RecipeData recipe = recipes[currentMenuIndex];
        int cookCount = countController.GetValue();



        if (!CanCook(recipe, cookCount))
        {
            Debug.Log("재료 부족!");
            return;
        }

        ConsumeIngredients(recipe, cookCount);

        ItemData result = GetResultItem(recipe);



        if (result != null)
        {
            int total = recipe.servingCount * cookCount;

            // 인벤토리 추가
            LTH_InventoryManager.Instance.AddItem(result, total);

            // 슬롯 데이터 처리
            if (slotItems[selectedSlotIndex] == result)
            {
                slotCounts[selectedSlotIndex] += total;
            }
            else
            {
                slotItems[selectedSlotIndex] = result;
                slotCounts[selectedSlotIndex] = total;
            }

            // UI 업데이트
            slotTexts[selectedSlotIndex].text =
                result.itemName + " x" + slotCounts[selectedSlotIndex];

            MenuTexts[selectedSlotIndex].text =
                result.itemName + " x" + slotCounts[selectedSlotIndex];

            if (spawner != null && spawner.isOpen)
            {
                // 영업 중이라면? 즉시 손님 대기열에 추가!
                spawner.AddToQueue(result, total);
                
                Debug.Log("<color=lime>[영업 중]</color> 실시간 손님 추가 완료!");

                spawner.ShuffleQueue();
            }
            else
            {
                // 영업 전이라면? 그냥 슬롯에만 담김 (나중에 StartBusiness에서 한꺼번에 처리)
                Debug.Log("<color=white>[영업 전]</color> 메뉴 예약 완료!");
            }

        }



        Debug.Log("result: " + result);
        iSAliveClick = false;
        isAliveAdd = false;

        currentMenuIndex = -1;
        //selectedSlotIndex = -1;

        ClearRecipeSlots();
    }

    void RemoveMenu()
    {
        for (int i = slotCounts.Length - 1; i >= 0; i--)
        {
            if (slotCounts[i] > 0)
            {
                // 인벤토리 1개 제거
                foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
                {
                    if (slot.itemData == slotItems[i])
                    {
                        slot.ChangeCount(-1);
                        break;
                    }
                }

                slotCounts[i]--;

                if (slotCounts[i] <= 0)
                {
                    slotItems[i] = null;
                    slotTexts[i].text = "메뉴추가하기";
                    MenuTexts[i].text = "메뉴추가하기";
                }
                else
                {
                    slotTexts[i].text =
                        slotItems[i].itemName + " x" + slotCounts[i];

                    MenuTexts[i].text =
                        slotItems[i].itemName + " x" + slotCounts[i];
                }

                Debug.Log("음식소거");
                return;
            }
        }

        Debug.Log("삭제할 메뉴 없음");
    }

    public void Back()
    {
        iSAliveClick = false;
        isAliveAdd = false;
        currentMenuIndex = -1;

        ClearRecipeSlots();
    }

    public void LoadScene()
    {
        if (!iSAliveClick)
        {
            TodaysUI.SetActive(false);
        }
    }
    /// <summary>
    /// '영업 시작' 버튼을 눌렀을 때 호출될 함수입니다.
    /// </summary>
    // CMJCookScene.cs 파일의 맨 아래쪽 혹은 적당한 위치에 붙여넣으세요.

    public void StartBusiness()
    {
        Debug.Log("<color=yellow>[System]</color> 영업 시작 버튼이 클릭되었습니다!");

        if (spawner == null)
        {
            Debug.LogError("Spawner가 연결되지 않았습니다!");
            return;
        }

        // 1. 스포너 대기열 초기화
        spawner.customerQueue.Clear();

        // 2. 현재 요리된 슬롯의 데이터를 스포너로 전달
        int totalCount = 0;
        for (int i = 0; i < slotCounts.Length; i++)
        {
            if (slotItems[i] != null && slotCounts[i] > 0)
            {
                spawner.AddToQueue(slotItems[i], slotCounts[i]);
                totalCount += slotCounts[i];
            }
        }

        if (totalCount <= 0)
        {
            Debug.LogWarning("요리된 음식이 없어 영업을 시작할 수 없습니다!");
            return;
        }

        // 3. 순서 랜덤하게 섞기
        spawner.ShuffleQueue();

        // 4. 시간 다시 흐르게 하기 (CMJCookB에서 멈춘 시간을 복구)
        Time.timeScale = 1f;

        // 5. 요리 UI만 끄기 (씬 이동 절대 금지!)
        if (TodaysUI != null) TodaysUI.SetActive(false);

        Debug.Log($"<color=cyan>[System]</color> 총 {totalCount}명의 예약 손님과 함께 영업을 시작합니다!");
    }
    /// <summary>
    /// 서빙이 완료되었을 때 호출하여 UI 슬롯의 숫자를 하나 줄입니다.
    /// </summary>
    public void DecreaseMenuCount(ItemData servedItem)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            // 1. 해당 슬롯의 아이템이 서빙된 아이템과 같고, 개수가 남아있다면
            if (slotItems[i] == servedItem && slotCounts[i] > 0)
            {
                // 2. 개수 감소
                slotCounts[i]--;

                // 3. UI 업데이트
                if (slotCounts[i] <= 0)
                {
                    // 개수가 0이면 슬롯 초기화
                    slotItems[i] = null;
                    slotTexts[i].text = "메뉴추가하기";
                    MenuTexts[i].text = "메뉴추가하기";
                }
                else
                {
                    // 남아있으면 개수 갱신
                    slotTexts[i].text = slotItems[i].itemName + " x" + slotCounts[i];
                    MenuTexts[i].text = slotItems[i].itemName + " x" + slotCounts[i];
                }

                Debug.Log($"<color=orange>[UI]</color> {servedItem.itemName} 남은 수량: {slotCounts[i]}");
                break; // 찾았으니 루프 종료
            }
        }

    }
    public void CancelSelectedRecipe()
    {
        // 선택 안 했으면 종료
        if (selectedSlotIndex < 0)
        {
            Debug.Log("선택된 슬롯 없음");
            return;
        }

        // 슬롯에 음식 없으면 종료
        if (slotItems[selectedSlotIndex] == null || slotCounts[selectedSlotIndex] <= 0)
        {
            Debug.Log("취소할 메뉴 없음");
            return;
        }

        ItemData item = slotItems[selectedSlotIndex];
        int count = slotCounts[selectedSlotIndex];

        //인벤토리에서 전부 제거
        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == item)
            {
                slot.ChangeCount(-count);
                break;
            }
        }

        //슬롯 초기화
        slotItems[selectedSlotIndex] = null;
        slotCounts[selectedSlotIndex] = 0;

        slotTexts[selectedSlotIndex].text = "메뉴추가하기";
        MenuTexts[selectedSlotIndex].text = "메뉴추가하기";

        Debug.Log("선택한 메뉴 취소 완료");

        //선택 해제
        selectedSlotIndex = -1;
    }
    /// <summary>
    /// [추가] 영업 중에 실시간으로 메뉴를 요리하여 손님 대기열을 보충합니다.
    /// </summary>
    public void AddMenuMidGame()
    {
        // 1. 기본 체크 (메뉴와 슬롯이 선택되어야 함)
        if (currentMenuIndex < 0 || selectedSlotIndex < 0) return;

        RecipeData recipe = recipes[currentMenuIndex];
        int cookCount = countController.GetValue();

        // 2. 재료 확인
        if (!CanCook(recipe, cookCount))
        {
            Debug.Log("<color=red>재료 부족!</color> 실시간 추가를 할 수 없습니다.");
            return;
        }

        // 3. 재료 소모 및 결과 아이템 획득
        ConsumeIngredients(recipe, cookCount);
        ItemData result = GetResultItem(recipe);

        if (result != null)
        {
            int total = recipe.servingCount * cookCount;

            // 4. 인벤토리 추가
            LTH_InventoryManager.Instance.AddItem(result, total);

            // 5. 슬롯 데이터 처리 (기존 UI 업데이트 로직 재사용)
            if (slotItems[selectedSlotIndex] == result)
            {
                slotCounts[selectedSlotIndex] += total;
            }
            else
            {
                slotItems[selectedSlotIndex] = result;
                slotCounts[selectedSlotIndex] = total;
            }

            // 6. UI 텍스트 갱신
            slotTexts[selectedSlotIndex].text = result.itemName + " x" + slotCounts[selectedSlotIndex];
            MenuTexts[selectedSlotIndex].text = result.itemName + " x" + slotCounts[selectedSlotIndex];

            // 7. ★ 핵심: 스포너가 영업 중이라면 대기열에 손님을 즉시 추가!
            if (spawner != null && spawner.isOpen)
            {
                spawner.AddToQueue(result, total);
                spawner.ShuffleQueue(); // 새로 들어온 손님 섞기
                Debug.Log($"<color=lime>[Mid-Game]</color> {result.itemName} {total}명 추가! 손님이 계속 옵니다.");
            }
        }

        // 8. 요리 UI 정리
        iSAliveClick = false;
        isAliveAdd = false;
        currentMenuIndex = -1;
        ClearRecipeSlots();
    }
}

