using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseMovement : MonoBehaviour
{
    public GameObject playerCamera;
    public float mouseSensitivity = 400f;
    public float nonSensitivity = 3f;
    public bool DeltaTime = false;
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GetComponent<PlayerInventory>().isOpen)
        {
            if (DeltaTime)
            {
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
                transform.Rotate(Vector3.up * mouseX);
                playerCamera.transform.Rotate(Vector3.left * mouseY);
            }
            else
            {
                float mouseX = Input.GetAxis("Mouse X") * nonSensitivity;
                float mouseY = Input.GetAxis("Mouse Y") * nonSensitivity;
                transform.Rotate(Vector3.up * mouseX);
                playerCamera.transform.Rotate(Vector3.left * mouseY);
            }
        }

    }
}
