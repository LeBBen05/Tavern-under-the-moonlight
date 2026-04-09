using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 개별 좌석의 상태와 해당 좌석으로 오기 위한 전용 경로 데이터를 담는 클래스입니다.
/// </summary>
public class Seat : MonoBehaviour
{
    [Header("좌석 상태")]
    [Tooltip("현재 이 좌석에 손님이 앉아있거나, 배정되어 이동 중인지 나타냅니다.")]
    public bool isOccupied = false;

    [Header("경로 설정")]
    [Tooltip("입구에서 이 좌석까지 오기 위해 거쳐야 하는 길목(Waypoints)들을 순서대로 넣어주세요.")]
    public List<Transform> pathToThisSeat = new List<Transform>();

    /// <summary>
    /// 손님이 퇴장할 때 호출하여 좌석을 다시 빈 상태로 만듭니다.
    /// </summary>
    public void ReleaseSeat()
    {
        // 좌석 점유 상태 해제
        isOccupied = false;

        // 여기에 필요하다면 '좌석 비우기' 애니메이션이나 효과음을 넣을 수 있습니다.
        // Debug.Log(gameObject.name + " 좌석이 비었습니다.");
    }
}