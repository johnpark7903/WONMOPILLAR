using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_simple : MonoBehaviour
{
    // Start is called before the first frame update
    float hAxis;
    float vAxis;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        hAxis = Input.GetAxisRaw("Horizontal");
        vAxis = Input.GetAxisRaw("Vertical");
        Vector3 moveVec = new Vector3(hAxis, 0, vAxis).normalized;

        transform.position += moveVec * 5f * Time.deltaTime;    
    }
}
