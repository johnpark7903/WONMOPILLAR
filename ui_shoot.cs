using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ui_shoot : MonoBehaviour, IPointerClickHandler
//, IPointerDownHandler, IPointerUpHandler
{
    // Start is called before the first frame update
    public Player_ikim player;

    public void OnPointerClick(PointerEventData eventData)
    {
        // player.uiFDown = true;
        player.Attack2();
    }

    // public void OnPointerDown(PointerEventData eventData)
    // {
    //     player.uiFDown = true; // 플레이어 스크립트의 uiFDown 변수 활성화
    //     Debug.Log("uiFDown = " + player.uiFDown);
    // }

    // public void OnPointerUp(PointerEventData eventData)
    // {
    //     player.uiFDown = false; // 버튼 뗄 때 비활성화
    //     Debug.Log("uiFDown = " + player.uiFDown);
    // }
//     public void OnClickShoot()
//     {
//         player.uiFDown = true; // 슈팅 입력 플래그 활성화 (단발)
//     }
}
