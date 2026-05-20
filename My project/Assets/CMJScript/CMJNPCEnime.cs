using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CMJNPCEnime : MonoBehaviour
{
    private Animator anim;

    private Vector3 lastPosition;
    private Vector3 moveDirection;

    // 마지막 방향 저장
    private bool lastLookUp = false;

    void Start()
    {
        anim = GetComponent<Animator>();

        lastPosition = transform.position;
    }

    void Update()
    {
        // 이동 방향 계산
        moveDirection = transform.position - lastPosition;

        bool isWalk = moveDirection.magnitude > 0.001f;

        anim.SetBool("IsMove", isWalk);

        if (isWalk)
        {
            moveDirection.Normalize();

            // ★ Blend Tree 방향 전달
            anim.SetFloat("MoveX", moveDirection.x);
            anim.SetFloat("MoveY", moveDirection.y);

            // 마지막 방향 저장
            if (moveDirection.y > 0.1f)
            {
                lastLookUp = true;
            }
            else if (moveDirection.y < -0.1f)
            {
                lastLookUp = false;
            }
        }

        // Animator 전달
        anim.SetBool("IsUp", lastLookUp);

        // 현재 위치 저장
        lastPosition = transform.position;
    }

    // =========================
    // 앉기 상태
    // =========================
    public void SetSitting(bool value)
    {
        anim.SetBool("IsSit", value);

        if (value)
        {
            anim.SetBool("IsMove", false);
        }
    }

    // =========================
    // 먹기 상태
    // =========================
    public void SetEating(bool value)
    {
        anim.SetBool("IsEat", value);

        if (value)
        {
            anim.SetBool("IsMove", false);
        }
    }
    public void SetWalkingDirection(Vector3 dir)
    {
        dir.Normalize();

        anim.SetFloat("MoveX", dir.x);
        anim.SetFloat("MoveY", dir.y);

        if (dir.y > 0.1f)
        {
            lastLookUp = true;
        }
        else if (dir.y < -0.1f)
        {
            lastLookUp = false;
        }

        anim.SetBool("IsUp", lastLookUp);
    }
}