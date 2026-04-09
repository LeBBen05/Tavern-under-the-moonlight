using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 게임 내 모든 좌석의 상태를 관리하고, 빈 자리를 찾아주는 매니저 클래스입니다.
/// </summary>
public class SeatManager : MonoBehaviour
{
    [Header("좌석 데이터 관리")]
    [Tooltip("씬에 배치된 모든 좌석(Seat) 오브젝트를 여기에 등록하세요.")]
    public List<Seat> allSeats = new List<Seat>();

    /// <summary>
    /// 현재 비어있는(isOccupied가 false인) 좌석 중 하나를 무작위로 선택하여 반환합니다.
    /// </summary>
    /// <returns>사용 가능한 좌석(Seat), 만약 모든 좌석이 찼다면 null을 반환합니다.</returns>
    public Seat GetAvailableSeat()
    {
        // 1. 람다식(Lambda)을 사용하여 전체 좌석 중 'isOccupied'가 false인 좌석들만 골라 리스트를 만듭니다.
        // s => !s.isOccupied : "s(각 좌석)의 isOccupied 상태가 거짓인 것만 찾아라"라는 의미입니다.
        List<Seat> available = allSeats.FindAll(s => !s.isOccupied);

        // 2. 비어있는 좌석이 최소 하나 이상 있는지 확인합니다.
        if (available.Count > 0)
        {
            // 3. 사용 가능한 좌석 리스트(available)에서 랜덤한 인덱스를 뽑아 반환합니다.
            // Random.Range(min, max)에서 정수형은 max가 포함되지 않으므로 Count를 그대로 넣으면 안전합니다.
            int randomIndex = Random.Range(0, available.Count);
            return available[randomIndex];
        }

        // 4. 모든 좌석이 사용 중이라면 아무것도 반환하지 않습니다(null).
        // 스포너(Spawner)에서는 이 null 값을 보고 손님 소환 여부를 결정하게 됩니다.
        return null;
    }
}