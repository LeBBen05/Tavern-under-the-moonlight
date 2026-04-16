using UnityEngine;
using UnityEngine.Tilemaps;
using System.Collections.Generic;

public class LTH_FarmingManager : MonoBehaviour
{
    public Tilemap groundTilemap;
    public GameObject cropPrefab;

    private Dictionary<Vector3Int, LTH_Crop> plantedCrops = new Dictionary<Vector3Int, LTH_Crop>();

    public void ExecuteInteraction(ItemData selectedItem)
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0;
        Vector3Int cellPos = groundTilemap.WorldToCell(mousePos);

        // 1. 수확 로직
        if (plantedCrops.ContainsKey(cellPos))
        {
            LTH_Crop targetCrop = plantedCrops[cellPos];

            if (targetCrop.IsFullyGrown)
            {
                HarvestCrop(cellPos, targetCrop);
            }
            else
            {
                Debug.Log("아직 자라는 중입니다.");
            }
            return;
        }

        // 2. 심기 로직
        if (selectedItem != null)
        {
            // SMS_ItemType.Seed를 사용하여 타입 체크
            if (selectedItem.itemType == SMS_ItemType.Seed)
            {
                PlantSeed(cellPos, selectedItem);
            }
        }
    }

    private void PlantSeed(Vector3Int cellPos, ItemData seedData)
    {
        Vector3 spawnPos = groundTilemap.GetCellCenterWorld(cellPos);
        GameObject newCropObj = Instantiate(cropPrefab, spawnPos, Quaternion.identity);
        LTH_Crop newCrop = newCropObj.GetComponent<LTH_Crop>();

        if (newCrop != null)
        {
            newCrop.Initialize(seedData);
            plantedCrops.Add(cellPos, newCrop);
            Debug.Log($"{seedData.itemName}을(를) 심었습니다.");
        }
    }

    private void HarvestCrop(Vector3Int cellPos, LTH_Crop crop)
    {
        ItemData yieldItem = crop.GetYieldItem();
        int amount = crop.GetYieldAmount();

        if (LTH_InventoryManager.Instance != null)
        {
            LTH_InventoryManager.Instance.AddItem(yieldItem, amount);
            Debug.Log($"{yieldItem.itemName} 수확 완료!");
        }

        plantedCrops.Remove(cellPos);
        Destroy(crop.gameObject);
    }
}