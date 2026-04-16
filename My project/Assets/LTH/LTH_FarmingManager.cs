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

        // groundTilemap(Tilemap field) 기준의 셀 좌표 가져오기
        Vector3Int cellPos = groundTilemap.WorldToCell(mousePos);

        // [핵심 로직] 해당 타일맵에 타일이 있는지 확인
        var tile = groundTilemap.GetTile(cellPos);

        if (tile == null)
        {
            // 타일이 없는 곳(허공이나 다른 타일맵)이면 아무것도 하지 않고 리턴
            Debug.Log("Tilemap field가 아닌 곳에는 심을 수 없습니다.");
            return;
        }

        // 1. 수확 로직: 이미 심어진 작물이 있다면
        if (plantedCrops.ContainsKey(cellPos))
        {
            LTH_Crop targetCrop = plantedCrops[cellPos];
            if (targetCrop.IsFullyGrown)
            {
                HarvestCrop(cellPos, targetCrop);
            }
            return;
        }

        // 2. 심기 로직: 타일이 있고(null이 아님), 손에 씨앗이 있을 때
        if (selectedItem != null && selectedItem.itemType == SMS_ItemType.Seed)
        {
            PlantSeed(cellPos, selectedItem);
        }
    }

    private void PlantSeed(Vector3Int cellPos, ItemData seedData)
    {
        // 1. 타일의 중심 위치를 가져오되, Z축은 반드시 0으로 맞춤 (카메라에 보이게 함)
        Vector3 spawnPos = groundTilemap.GetCellCenterWorld(cellPos);
        spawnPos.z = 0f;

        // 2. 작물 프리팹 생성
        GameObject newCropObj = Instantiate(cropPrefab, spawnPos, Quaternion.identity);

        // 3. 레이어 설정 (땅 타일보다 앞에 보이도록 숫자를 높임)
        SpriteRenderer sr = newCropObj.GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.sortingLayerName = "Crops"; // 유니티에 Crops 레이어가 있다면 설정
            sr.sortingOrder = 5;           // 타일맵(보통 0)보다 높은 숫자로 설정
        }

        LTH_Crop newCrop = newCropObj.GetComponent<LTH_Crop>();
        if (newCrop != null)
        {
            newCrop.Initialize(seedData);
            plantedCrops.Add(cellPos, newCrop);
            Debug.Log($"{seedData.itemName}을(를) {cellPos}에 심었습니다.");
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