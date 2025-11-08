using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class Player_simple : MonoBehaviour
{
    // Start is called before the first frame update
    public float speed;
    float hAxis;
    float vAxis;
    Vector3 moveVec;

    void Start()
    {

    }
    void Update()
    {
        // Keyboard WASD / Arrow keys
        Vector2 kb = Vector2.zero;
        if (Keyboard.current != null)
        {
            kb.x = (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed ? 1f : 0f)
                 - (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed ? 1f : 0f);
            kb.y = (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed ? 1f : 0f)
                 - (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed ? 1f : 0f);
        }

        // Gamepad left stick fallback
        Vector2 gp = Vector2.zero;
        if (Gamepad.current != null)
            gp = Gamepad.current.leftStick.ReadValue();

        Vector2 input = gp != Vector2.zero ? gp : kb;
        moveVec = new Vector3(input.x, 0f, input.y).normalized;

        transform.position += moveVec * speed * Time.deltaTime;
    }

    // Update is called once per frame
    /*
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * 5f * Time.deltaTime;    
    }*/
}
