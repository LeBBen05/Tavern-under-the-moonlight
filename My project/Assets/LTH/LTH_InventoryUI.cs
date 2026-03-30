using UnityEngine;

public class LTH_InventoryUI : MonoBehaviour
{
    public GameObject inventoryUI;
    private bool isOpen = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isOpen = !isOpen;
        inventoryUI.SetActive(isOpen);
        Time.timeScale = isOpen ? 0f : 1f;

        // 마우스 커서 제어 (선택)
        Cursor.visible = isOpen;
        Cursor.lockState = isOpen ? CursorLockMode.None : CursorLockMode.Locked;
    }
}