using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CMJMove : MonoBehaviour
{
    [Header("플레이어 설정")]
    public Transform player;   //플레이어 연결
    public float interactDistance = 3f; //상호작용 거리
    private void OnMouseDown()
    {
        if (IsPlayerNear())
            SceneManager.LoadScene("MainScene");
    }
    bool IsPlayerNear()
    {
        if (player == null) return false;

        float distance = Vector3.Distance(
            transform.position,
            player.position
        );

        return distance <= interactDistance;
    }
}
