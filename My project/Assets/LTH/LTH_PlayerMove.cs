using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class LTH_PlayerMove : MonoBehaviour
{
    public float LTH_moveSpeed = 5f;
    private Vector3 LTH_targetPositon;
    public bool LTH_isMoving = false;


    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            //클릭 했을 때 그 자리가 UI가 있다면
            if(EventSystem.current.IsPointerOverGameObject())
            {
                return;
            }
            LTH_targetPositon = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            LTH_targetPositon.z = 0f;

            LTH_isMoving = true;
        }

        if (LTH_isMoving)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                LTH_targetPositon,
                LTH_moveSpeed * Time.deltaTime
            );

            if (Vector3.Distance(transform.position, LTH_targetPositon) < 0.05f)
            {
                LTH_isMoving = false;
            }
        }
    }
}
