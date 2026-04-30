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

    public GameObject TodaysUI;

    int selectedSlotIndex = -1;
    int currentMenuIndex = -1;

    public bool iSAliveClick = false;
    public bool isAliveAdd = false;

    ItemData[] slotItems;
    int[] slotCounts;

    RecipeData[] slotRecipes;
    int[] slotCookCounts;

    void Start()
    {
        ClearRecipeSlots();

        int size = slotTexts.Length;

        slotItems = new ItemData[size];
        slotCounts = new int[size];
        slotRecipes = new RecipeData[size];
        slotCookCounts = new int[size];

        for (int i = 0; i < size; i++)
        {
            slotTexts[i].text = "메뉴추가하기";
            MenuTexts[i].text = "메뉴추가하기";
        }
    }

    void Update()
    {
        ClickMenuUI.SetActive(iSAliveClick);
        AddButton.SetActive(isAliveAdd);
        /*
        if (Input.GetKeyDown(KeyCode.Q))
        {
            RemoveMenu();
        }
        */
    }

    public void OnClickSlot(int index)
    {
        selectedSlotIndex = index;
        iSAliveClick = true;
    }

    public void MenuClick(int index)
    {
        if (index < 0 || index >= recipes.Length) return;

        currentMenuIndex = index;
        isAliveAdd = true;

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

    // 함수 정상 분리됨
    public void DecreaseMenuCount(ItemData servedItem)
    {
        for (int i = 0; i < slotItems.Length; i++)
        {
            if (slotItems[i] == servedItem && slotCounts[i] > 0)
            {
                slotCounts[i]--;

                if (slotCounts[i] <= 0)
                {
                    slotItems[i] = null;
                    slotTexts[i].text = "메뉴추가하기";
                    MenuTexts[i].text = "메뉴추가하기";
                }
                else
                {
                    slotTexts[i].text = slotItems[i].itemName + " x" + slotCounts[i];
                    MenuTexts[i].text = slotItems[i].itemName + " x" + slotCounts[i];
                }

                Debug.Log($"서빙 완료: {servedItem.itemName}, 남은 수량: {slotCounts[i]}");
                break;
            }
        }
    }

    //이제 정상 위치
    public void Back()
    {
        iSAliveClick = false;
        isAliveAdd = false;

        currentMenuIndex = -1;
        selectedSlotIndex = -1;

        ClearRecipeSlots();

        Debug.Log("뒤로가기 (UI 닫힘)");
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

            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
            {
                foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
                {
                    if (slot.itemData.itemName == ing.requriedItem.itemName)
                    {
                        int remove = Mathf.Min(need, slot.currentCount);
                        slot.ChangeCount(-remove);
                        need -= remove;
                        if (need <= 0) break;
                    }
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
        if (result == null)
        {
            Debug.LogError("결과 아이템 없음!");
            return;
        }

        int total = recipe.servingCount * cookCount;

        LTH_InventoryManager.Instance.AddItem(result, total);

        slotItems[selectedSlotIndex] = result;
        slotCounts[selectedSlotIndex] = total;

        slotRecipes[selectedSlotIndex] = recipe;
        slotCookCounts[selectedSlotIndex] = cookCount;

        slotTexts[selectedSlotIndex].text = result.itemName + " x" + total;
        MenuTexts[selectedSlotIndex].text = result.itemName + " x" + total;

        iSAliveClick = false;
        isAliveAdd = false;
        currentMenuIndex = -1;

        ClearRecipeSlots();
    }

    public void CancelSelectedRecipe()
    {
        if (selectedSlotIndex < 0) return;

        if (slotItems[selectedSlotIndex] == null) return;

        ItemData item = slotItems[selectedSlotIndex];
        int count = slotCounts[selectedSlotIndex];

        // 결과 제거
        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == item)
            {
                slot.ChangeCount(-count);
                break;
            }
        }

        // 슬롯 초기화
        slotItems[selectedSlotIndex] = null;
        slotCounts[selectedSlotIndex] = 0;

        slotTexts[selectedSlotIndex].text = "메뉴추가하기";
        MenuTexts[selectedSlotIndex].text = "메뉴추가하기";

        selectedSlotIndex = -1;

        Debug.Log("선택 메뉴 취소 완료");
    }

    public void LoadScene()
    {
        if (!iSAliveClick)
        {
            if (TodaysUI != null)
            {
                TodaysUI.SetActive(false);
            }
        }

    }

    public void RemoveMenu()
    {
        for (int i = slotCounts.Length - 1; i >= 0; i--)
        {
            if (slotCounts[i] > 0)
            {
                foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
                {
                    if (slot.itemData.itemName == slotItems[i].itemName)
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
                    slotTexts[i].text = slotItems[i].itemName + " x" + slotCounts[i];
                    MenuTexts[i].text = slotItems[i].itemName + " x" + slotCounts[i];
                }

                Debug.Log("음식소거");
                return;
            }
        }
    }
}