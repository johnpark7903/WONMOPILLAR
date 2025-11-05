using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shop : MonoBehaviour
{
    // Start is called befo
    // re the first frame update
    public RectTransform uiGroup;
    public Animator anim;
    Player_ikim enterPlayer;

    public void Enter(Player_ikim player)
    {
        // enterPlayer = player;
        // uiGroup.anchoredPosition = Vector3.zero;
        // 상점 입장 시
        if (player == null) return;
            enterPlayer = player;

        if (uiGroup != null)
        {
            uiGroup.gameObject.SetActive(true);
            uiGroup.anchoredPosition = Vector3.zero;
        }   
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    // Update is called once per frame
    public void Exit()
    {
        //anim.SetTrigger("doHello");
        //uiGroup.anchoredPosition = Vector3.down * 1000;
        // 상점 나갈 시
        if (uiGroup != null)
        uiGroup.gameObject.SetActive(false);
        // uiGroup.gameObject.SetActive(false);
        enterPlayer = null; 
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}
