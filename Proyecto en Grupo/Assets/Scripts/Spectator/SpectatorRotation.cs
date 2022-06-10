using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpectatorRotation : MonoBehaviour
{
    public float mouseSensitivity = 100.0f;
    float xRotation = 0f;

    Transform parent;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        parent = transform.parent;
    }

    // Update is called once per frame
    void Update()
    {
        // Camera
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -65f, 65f);
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        parent.Rotate(Vector3.up * mouseX);
    }
}
