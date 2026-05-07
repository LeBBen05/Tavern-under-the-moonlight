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

    [Header("미리보기 텍스트")]
    public Text previewText;

    [Header("재료 정보 텍스트")]
    public Text[] ingredientTexts;

    int selectedSlotIndex = -1;
    int currentMenuIndex = -1;

    public bool iSAliveClick = false;
    public bool isAliveAdd = false;

    //핵심: 슬롯별 데이터
    ItemData[] slotItems;
    int[] slotCounts;

    RecipeData[] slotRecipes;
    int[] slotCookCounts;

    void Start()
    {
        ClearRecipeSlots();

        slotRecipes = new RecipeData[slotTexts.Length];
        slotCookCounts = new int[slotTexts.Length];

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

        UpdatePreview();

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

        int cookCount = countController.GetValue();

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var ing = recipe.ingredients[i];

            recipeSlotImages[i].gameObject.SetActive(true);
            ingredientTexts[i].gameObject.SetActive(true);

            int need = ing.amount * cookCount;
            int current = 0;

            string itemName = "";

            // 아이템 재료
            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
            {
                recipeSlotImages[i].sprite =
                    GetItemSprite(ing.requriedItem);

                current = GetItemCount(ing.requriedItem);
                itemName = ing.requriedItem.itemName;
            }
            // 물고기 재료
            else
            {
                recipeSlotImages[i].sprite =
                    GetFishSprite(ing.RfishSize);

                current = GetFishCount(ing.RfishSize);
                itemName = ing.RfishSize.ToString();
            }

            // 텍스트 표시
            ingredientTexts[i].text =
                $"{itemName}\n{current}/{need}";

            // 색 변경
            if (current >= need)
            {
                ingredientTexts[i].color = Color.green;
            }
            else
            {
                ingredientTexts[i].color = Color.red;
            }
        }

        // 남는 텍스트 숨기기
        for (int i = recipe.ingredients.Count; i < ingredientTexts.Length; i++)
        {
            ingredientTexts[i].gameObject.SetActive(false);
        }
    }

    void ClearRecipeSlots()
    {
        for (int i = 0; i < recipeSlotImages.Length; i++)
        {
            recipeSlotImages[i].gameObject.SetActive(false);
            recipeSlotImages[i].sprite = null;

            ingredientTexts[i].gameObject.SetActive(false);
            ingredientTexts[i].text = "";
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

    void UpdatePreview()
    {
        if (currentMenuIndex < 0)
        {
            previewText.text = "";
            return;
        }

        RecipeData recipe = recipes[currentMenuIndex];
        int cookCount = countController.GetValue();

        int total = recipe.servingCount * cookCount;

        previewText.text = $"총개수: {total}";
        if (currentMenuIndex >= 0)
        {
            ShowRecipe(recipes[currentMenuIndex]);
        }
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
            slotRecipes[selectedSlotIndex] = recipe;
            slotCookCounts[selectedSlotIndex] = cookCount;

            // UI 업데이트
            slotTexts[selectedSlotIndex].text =
                result.itemName + " x" + slotCounts[selectedSlotIndex];

            MenuTexts[selectedSlotIndex].text =
                result.itemName + " x" + slotCounts[selectedSlotIndex];

            if (spawner != null && spawner.isOpen)
            {
                // 이미 장사 중이라면, 스포너 대기열에 즉시 손님을 추가합니다.
                spawner.AddToQueue(result, total);
                spawner.ShuffleQueue();
                Debug.Log($"<color=lime>[실시간]</color> {result.itemName} 손님 {total}명 추가 완료!");
            }
        }

        iSAliveClick = false;
        isAliveAdd = false;

        currentMenuIndex = -1;
        selectedSlotIndex = -1;

        ClearRecipeSlots();
    }

    public void RemoveMenu()
    {
        for (int i = slotCounts.Length - 1; i >= 0; i--)
        {
            if (slotCounts[i] > 0)
            {
                int removeAmount = slotCounts[i];

                if (spawner != null && slotItems[i] != null)
                {
                    spawner.ForceRemoveMenu(slotItems[i]);
                }
                // 결과 음식 제거
                foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
                {
                    if (slot.itemData.itemName == slotItems[i].itemName)
                    {
                        slot.ChangeCount(-removeAmount);
                        break;
                    }
                }
                //핵심 조건
                int originalAmount = slotRecipes[i].servingCount * slotCookCounts[i];

                if (slotCounts[i] == originalAmount)
                {
                    RestoreIngredients(i);
                    Debug.Log("재료복구");
                }

                // 슬롯 초기화
                slotCounts[i] = 0;
                slotItems[i] = null;
                slotRecipes[i] = null;
                slotCookCounts[i] = 0;

                slotTexts[i].text = "메뉴추가하기";
                MenuTexts[i].text = "메뉴추가하기";

                Debug.Log("음식 삭제");
                return;
            }
        }
    }
    void RestoreIngredients(int index)
    {
        RecipeData recipe = slotRecipes[index];
        int cookCount = slotCookCounts[index];

        if (recipe == null) return;

        foreach (var ing in recipe.ingredients)
        {
            int amount = ing.amount * cookCount;

            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
            {
                LTH_InventoryManager.Instance.AddItem(ing.requriedItem, amount);
            }
        }
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
}
