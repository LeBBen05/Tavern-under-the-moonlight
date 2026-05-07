using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Te_InventoryTester : MonoBehaviour
{
    public Te_ItemData apple; // 인스펙터에서 Apple 데이터를 드래그해 넣으세요.

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
        { // 스페이스바 누르면 추가
            Te_InventoryManager.Instance.AddItem(apple, 1);
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            Te_InventoryManager.Instance.RemoveItem(apple, 1);
            Debug.Log("아이템 감소 테스트");
        }
    }
}