using UnityEngine;

public class CameraController : MonoBehaviour
{
    public float moveMultiplier = 1;
    public float zoomMultiplier = 10;
    public Rigidbody cameraRigidbody;


    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetButton("Fire1"))
        {
            movement.x = - Input.GetAxis("Mouse X") * moveMultiplier;
            movement.z = - Input.GetAxis("Mouse Y") * moveMultiplier;
        }

        movement.y = - Input.GetAxis("Mouse ScrollWheel") * zoomMultiplier;
        cameraRigidbody.position += movement * (1 + Mathf.Max(.001f, cameraRigidbody.position.y) / 16);
    }
}
