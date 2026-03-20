using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 이 스크립트는 레시피 데이터에 대한 스크립트 입니다.
/// </summary>
public enum SMS_RecipeRequirementType
{
    SpecificItem,   //특정 아이템이 필요한 경우
    AnyFish //특정 크기의 물고기가 있을 경우
}

[System.Serializable]
public struct SMS_RecipeIngredient
{
    public SMS_RecipeRequirementType rcqType;
    
    //특정 식재료를 요구하는 경우
    public ItemData requriedItem;   //필요한 식재료

    //물고기 크기를 요구할 때 쓸 빈칸
    public SMS_FishSize RfishSize;
    public int amount;   //필요 개수

}

[CreateAssetMenu(fileName = " New Reicpe", menuName = "Reicpe")]
public class RecipeData : ScriptableObject
{
    [Header("기본 레시피 정보")]
    public string recipeID; //레시피 아이디
    public string recipeName;   //레시피 이름
    public Sprite reicpeIcon;   //레시피 아이콘
    public int sellPrice;   //그릇 당 판매가
    public int servingCount; // 한 번 요리 시 나오는 그릇 개수


    [Header("레시피: 요리에 필요한 재료")]
    public List<SMS_RecipeIngredient> ingredients;  //요리에 들어가는 재료 배열

    
}
