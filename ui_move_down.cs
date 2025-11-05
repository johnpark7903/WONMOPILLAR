using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;




public class Ui_move_down : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    public Player_ikim player;

    public void OnPointerDown(PointerEventData eventData)
    {
        player.uiVAxis = -1f; // 플레이어 스크립트의 vAxis 변수 활성화
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        player.uiVAxis = 0f; // 버튼 뗄 때 비활성화
    }
    
    
    
    // Start is called before the first frame update
    // public GameObject player; // Hierarchy에서 할당
    // public void MoveUp() { player.transform.Translate(Vector3.up * 1f); }


}


/*
public class MoveController : MonoBehaviour
{
    public Button upButton;
    public Button downButton;
    public Button leftButton;
    public Button rightButton;
    public float moveAmount = 1f; // 이동거리

    void Start()
    {
        upButton.onClick.AddListener(MoveUp);
        downButton.onClick.AddListener(MoveDown);
        leftButton.onClick.AddListener(MoveLeft);
        rightButton.onClick.AddListener(MoveRight);
    }

    void MoveUp()
    {
        transform.Translate(Vector3.up * moveAmount);
    }
    void MoveDown()
    {
        transform.Translate(Vector3.down * moveAmount);
    }
    void MoveLeft()
    {
        transform.Translate(Vector3.left * moveAmount);
    }
    void MoveRight()
    {
        transform.Translate(Vector3.right * moveAmount);
    }
}
*/