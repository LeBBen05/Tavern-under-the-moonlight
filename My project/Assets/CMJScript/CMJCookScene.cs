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

    [Header("레시피 데이터")]
    public RecipeData[] recipes;

    [Header("재료 이미지 매핑")]
    public ItemImageData[] itemImages;

    [Header("레시피 슬롯 UI (4칸)")]
    public Image[] recipeSlotImages;

    [Header("메뉴판 띄우기")]
    public Text[] MenuTexts;

    int selectedSlotIndex = -1;
    int currentMenuIndex = -1;

    public bool iSAliveClick = false;
    public bool isAliveAdd = false;
    public GameObject TodaysUI;

    void Start()
    {
        ClearRecipeSlots();
    }

    void Update()
    {
        ClickMenuUI.SetActive(iSAliveClick);
        AddButton.SetActive(isAliveAdd);
    }

    // 슬롯 클릭
    public void OnClickSlot(int index)
    {
        selectedSlotIndex = index;
        iSAliveClick = true;
    }

    // 메뉴 클릭
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

    // 레시피 UI 표시
    void ShowRecipe(RecipeData recipe)
    {
        ClearRecipeSlots();

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var ing = recipe.ingredients[i];

            recipeSlotImages[i].gameObject.SetActive(true);

            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
                recipeSlotImages[i].sprite = GetItemSprite(ing.requriedItem);
            else if (ing.rcqType == SMS_RecipeRequirementType.AnyFish)
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

    // 아이템 개수 체크
    int GetItemCount(ItemData item)
    {
        int count = 0;

        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (slot.itemData == item)
            {
                count += slot.currentCount;
            }
        }

        return count;
    }

    // 물고기 개수 체크
    int GetFishCount(SMS_FishSize size)
    {
        int count = 0;

        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            foreach (var data in itemImages)
            {
                if (data.item == null) continue;

                if (slot.itemData == data.item && data.fishSize == size)
                {
                    count += slot.currentCount;
                }
            }
        }

        return count;
    }

    // 제작 가능 여부
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
            else if (ing.rcqType == SMS_RecipeRequirementType.AnyFish)
            {
                if (GetFishCount(ing.RfishSize) < need)
                    return false;
            }
        }
        return true;
    }

    // 재료 차감
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

    // 결과 아이템 찾기
    ItemData GetResultItem(RecipeData recipe)
    {
        foreach (var data in recipeResults)
        {
            if (data.recipe == recipe)
                return data.resultItem;
        }
        return null;
    }

    // 요리 실행
    public void Add()
    {
        if (currentMenuIndex < 0 || selectedSlotIndex < 0) return;

        RecipeData recipe = recipes[currentMenuIndex];

        //개수 가져오기
        int cookCount = countController.GetValue();

        //제작 가능 체크
        if (!CanCook(recipe, cookCount))
        {
            Debug.Log("재료 부족!");
            return;
        }

        //재료 차감
        ConsumeIngredients(recipe, cookCount);

        //결과 생성
        ItemData result = GetResultItem(recipe);

        if (result != null)
        {
            int totalAmount = recipe.servingCount * cookCount;

            LTH_InventoryManager.Instance.AddItem(
                result,
                totalAmount
            );
        }
        else
        {
            Debug.LogError("결과 아이템 연결 안됨!");
        }

        // 메뉴판 표시
        slotTexts[selectedSlotIndex].text =
            recipe.recipeName + " x" + (recipe.servingCount * cookCount);
        MenuTexts[selectedSlotIndex].text =
            recipe.recipeName + " x" + (recipe.servingCount * cookCount);

        Debug.Log("요리 성공: " + recipe.recipeName + " x" + cookCount);

        iSAliveClick = false;
        isAliveAdd = false;

        currentMenuIndex = -1;
        selectedSlotIndex = -1;

        ClearRecipeSlots();
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
}