using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


public class ui_run : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
//, IPointerClickHandler
{
    // Start is called before the first frame update
    public Player_ikim player;

    // public void OnPointerClick(PointerEventData eventData)
    // {
    //     //player.Dodge2();
    //     player.RequestDodge();
    // }
    public void OnPointerDown(PointerEventData eventData)
    {
        player.requestDodge = true; // 플레이어 스크립트의 uiJDown 변수 활성화
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        return;//player.requestDodge = false; // 버튼 뗄 때 비활성화
    }

    // public void OnClickJump()
    // {
    //     player.uiJDown = true; // 점프 입력 플래그 활성화 (단발)
    // }

}
