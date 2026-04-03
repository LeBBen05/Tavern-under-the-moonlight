using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#region 데이터 구조

[System.Serializable]
public class ItemImageData
{
    public ItemData item;          // 레시피용 ItemData
    public SMS_FishSize fishSize;  // 물고기 크기
    public Image image;            // Sprite를 가져오기 위한 UI 이미지
}

#endregion

public class CMJCookScene : MonoBehaviour
{
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

    int selectedSlotIndex = -1;
    int currentMenuIndex = -1;

    public bool iSAliveClick = false;
    public bool isAliveAdd = false;

    void Start()
    {
        ClearRecipeSlots();
    }

    void Update()
    {
        ClickMenuUI.SetActive(iSAliveClick);
        AddButton.SetActive(isAliveAdd);
    }

    //슬롯 클릭
    public void OnClickSlot(int index)
    {
        selectedSlotIndex = index;
        iSAliveClick = true;
    }

    //메뉴 클릭
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

    //레시피 표시
    void ShowRecipe(RecipeData recipe)
    {
        ClearRecipeSlots();

        for (int i = 0; i < recipe.ingredients.Count; i++)
        {
            var ing = recipe.ingredients[i];

            recipeSlotImages[i].gameObject.SetActive(true);

            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
            {
                recipeSlotImages[i].sprite = GetItemSprite(ing.requriedItem);
            }
            else if (ing.rcqType == SMS_RecipeRequirementType.AnyFish)
            {
                recipeSlotImages[i].sprite = GetFishSprite(ing.RfishSize);
            }
        }
    }

    //슬롯 초기화
    void ClearRecipeSlots()
    {
        for (int i = 0; i < recipeSlotImages.Length; i++)
        {
            recipeSlotImages[i].gameObject.SetActive(false);
            recipeSlotImages[i].sprite = null;
        }
    }

    //Sprite 가져오기
    Sprite GetItemSprite(ItemData item)
    {
        foreach (var data in itemImages)
        {
            if (data.item == item && data.image != null)
            {
                return data.image.sprite;
            }
        }
        return null;
    }

    Sprite GetFishSprite(SMS_FishSize size)
    {
        foreach (var data in itemImages)
        {
            if (data.fishSize == size && data.image != null)
            {
                return data.image.sprite;
            }
        }
        return null;
    }

    ItemData ConvertItem(LTH_ItemData lthItem)
    {
        if (lthItem == null) return null;

        foreach (var data in itemImages)
        {
            if (data == null || data.item == null) continue;

            //핵심 수정: itemName 비교
            if (data.item.itemName.Trim().ToLower() ==
                lthItem.itemName.Trim().ToLower())
            {
                return data.item;
            }
        }

        return null;
    }

    //아이템 개수 확인
    int GetItemCount(ItemData item)
    {

        int count = 0;

        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            if (ConvertItem(slot.itemData) == item)
            {
                count += slot.currentCount;
            }
            Debug.Log("인벤토리 아이템: " + slot.itemData.itemName);

            ItemData converted = ConvertItem(slot.itemData);

            if (converted == null)
            {
                Debug.LogError("매칭 실패!");
            }
            else
            {
                Debug.Log("매칭 성공: " + converted.itemName);
            }
        }

        return count;
    }

    //물고기 개수 확인
    int GetFishCount(SMS_FishSize size)
    {
        int count = 0;

        foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
        {
            foreach (var data in itemImages)
            {
                if (data.item != null &&
                    ConvertItem(slot.itemData) == data.item &&
                    data.fishSize == size)
                {
                    count += slot.currentCount;
                }
            }
        }

        return count;
    }

    //제작 가능 여부
    bool CanCook(RecipeData recipe)
    {
        foreach (var ing in recipe.ingredients)
        {
            if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem)
            {
                if (GetItemCount(ing.requriedItem) < ing.amount)
                    return false;
            }
            else if (ing.rcqType == SMS_RecipeRequirementType.AnyFish)
            {
                if (GetFishCount(ing.RfishSize) < ing.amount)
                    return false;
            }
        }
        return true;
    }

    //재료 차감
    void ConsumeIngredients(RecipeData recipe)
    {
        foreach (var ing in recipe.ingredients)
        {
            int need = ing.amount;

            foreach (var slot in LTH_InventoryManager.Instance.activeSlots)
            {
                // 특정 아이템
                if (ing.rcqType == SMS_RecipeRequirementType.SpecificItem &&
                    ConvertItem(slot.itemData) == ing.requriedItem)
                {
                    int remove = Mathf.Min(need, slot.currentCount);
                    slot.ChangeCount(-remove);
                    need -= remove;

                    if (need <= 0) break;
                }

                // 물고기
                else if (ing.rcqType == SMS_RecipeRequirementType.AnyFish)
                {
                    foreach (var data in itemImages)
                    {
                        if (data.item != null &&
                            ConvertItem(slot.itemData) == data.item &&
                            data.fishSize == ing.RfishSize)
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
    }

    //메뉴 추가
    public void Add()
    {
        if (currentMenuIndex < 0 || selectedSlotIndex < 0) return;

        RecipeData recipe = recipes[currentMenuIndex];

        if (!CanCook(recipe))
        {
            Debug.Log("재료 부족!");

            iSAliveClick = false;
            isAliveAdd = false;

            currentMenuIndex = -1;
            selectedSlotIndex = -1;

            ClearRecipeSlots();
            return;
        }

        // 재료 차감
        ConsumeIngredients(recipe);

        // 메뉴판 표시
        slotTexts[selectedSlotIndex].text =
            recipe.recipeName + " x" + recipe.servingCount;

        Debug.Log("요리 성공: " + recipe.recipeName);

        iSAliveClick = false;
        isAliveAdd = false;

        currentMenuIndex = -1;
        selectedSlotIndex = -1;

        ClearRecipeSlots();
    }

    //뒤로가기
    public void Back()
    {
        iSAliveClick = false;
        isAliveAdd = false;
        currentMenuIndex = -1;

        ClearRecipeSlots();
    }

    // 씬 이동
    public void LoadScene()
    {
        if (!iSAliveClick)
        {
            SceneManager.LoadScene("TestMapScene");
        }
    }
}