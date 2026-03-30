using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEditor.Progress;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace CraftingSystem
{
    [CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Object/CraftingRecipe", order = int.MaxValue)]
    public class CraftingRecipe : ScriptableObject
    {
        [Header("제작에 필요한 재료 아이템들")]
        [SerializeField] public CraftingItemInfo[] reqItems;

        [Header("제작 결과물 아이템")]
        [SerializeField] public CraftingItemInfo resultItem;

        [Space(30)]
        [Header("요구되는 레벨")]
        [SerializeField] public int reqLevel;

        [Header("제작에 걸리는 시간")]
        [SerializeField] public float craftingTime;

        [Header("아이콘을 표시할 이미지")]
        [SerializeField] public Sprite buttonSprite;
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CraftingRecipe))]
    public class CMJCookTest : Editor
    {
        CraftingRecipe recipe;

        void OnEnable()
        {
            recipe = (CraftingRecipe)target;
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            // 버튼 생성
            if (GUILayout.Button("이름 자동 변경"))
            {
                // 이름 변경
                //AssetDatabase.RenameAsset(AssetDatabase.GetAssetPath(recipe), $"RECIPE__{recipe.resultItem.item.ID.ToString()}");
                Debug.Log("레시피가 생성되었습니다!");
            }
        }
    }
#endif    

    [System.Serializable]
    public struct CraftingItemInfo
    {
        [SerializeField] public Item item;
        [SerializeField] public int count;
    }
}