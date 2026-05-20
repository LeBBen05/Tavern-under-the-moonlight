using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMJPlayerEnime : MonoBehaviour
{
    private Animator anim;

    private Vector3 lastPosition;
    private Vector3 moveDirection;

    // 이동 스크립트 참조
    private LTH_PlayerMove playerMove;

    // Carry 상태 저장
    private bool isCarrying = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        playerMove = GetComponent<LTH_PlayerMove>();

        lastPosition = transform.position;
    }

    void Update()
    {
        // 현재 이동 방향 계산
        moveDirection = transform.position - lastPosition;

        // 이동 중인지 확인
        bool isMoving = moveDirection.magnitude > 0.001f;

        anim.SetBool("IsMoving", isMoving);

        // 이동 방향 저장
        if (isMoving)
        {
            moveDirection.Normalize();

            anim.SetFloat("MoveX", moveDirection.x);
            anim.SetFloat("MoveY", moveDirection.y);

            anim.SetFloat("LastMoveX", moveDirection.x);
            anim.SetFloat("LastMoveY", moveDirection.y);
        }

        // 현재 위치 저장
        lastPosition = transform.position;

        // ===== 낚시 상태 =====
        if (playerMove != null)
        {
            bool isFishing = !playerMove.enabled;

            anim.SetBool("IsFishing", isFishing);
        }

        // ===== Carry 상태 =====
        anim.SetBool("IsCarrying", isCarrying);
    }

    // =========================
    // Carry ON/OFF
    // =========================
    public void SetCarrying(bool value)
    {
        isCarrying = value;
    }
}