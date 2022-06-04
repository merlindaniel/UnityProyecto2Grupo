using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement : MonoBehaviour
{
    private float speed;
    public float defaultSpeed = 1.5f;

    public CharacterController controller;

    public float mouseSensitivity = 100.0f;
    float xRotation = 0f;


    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked; 
        speed = defaultSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        // Movement
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");

        Vector3 move = transform.right * x + transform.forward * z;
        controller.Move(move * speed * Time.deltaTime);

        if (Input.GetKey(KeyCode.LeftShift))
        {
            speed = defaultSpeed * 1.5f;
        }
        if (Input.GetKeyUp(KeyCode.LeftShift))
        {
            speed = defaultSpeed;
        }
    }
}
