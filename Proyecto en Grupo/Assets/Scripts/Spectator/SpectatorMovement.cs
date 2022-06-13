using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorMovement : MonoBehaviour
{
    private float speed;
    public float defaultSpeed = 1.5f;

    private CharacterController controller;
    private bool lockedCursor;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        lockedCursor = true;
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
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (lockedCursor)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                lockedCursor = false;
            } else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                lockedCursor = true;
            }
            
        }

    }
}
