using System.Collections;
using System.Collections.Generic;
//ing System.Numerics;
// using System.Reflection.Metadata.Ecma335;
// using System.Threading.Tasks.Dataflow;
using UnityEngine;

public class player_Move : MonoBehaviour
{
    // Start is called before the first frame update
    public FixedJoystick joystick;
    public float speed = 5f;
    private CharacterController controller;
    void Start()
    {
        controller = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        MovePlayer();
    }

    void MovePlayer()
    {
        // Vector3 direction = new Vector3(joystick.Horizontal, 0, joystick.Vertical);
        // controller.Move(direction * speed * Time.deltaTime);
        Vector3 Move = transform.right * joystick.Horizontal + transform.forward * joystick.Vertical;
        controller.Move(Move * speed * Time.deltaTime);
    }
        
    
}
