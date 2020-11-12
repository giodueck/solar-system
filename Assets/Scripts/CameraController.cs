using UnityEngine;

[ExecuteInEditMode]
[RequireComponent(typeof(Rigidbody))]
public class CameraController : MonoBehaviour
{
    public float moveMultiplier = 1;
    public float zoomMultiplier = 10;
    public Rigidbody cameraRigidbody;

    private void Awake()
    {
        cameraRigidbody = GetComponent<Rigidbody>();
        cameraRigidbody.useGravity = false;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetButton("Fire1"))
        {
            movement.x = -Input.GetAxis("Mouse X") * moveMultiplier;
            movement.z = -Input.GetAxis("Mouse Y") * moveMultiplier;
        }

        // Zoom
        movement.y = -Input.GetAxis("Mouse ScrollWheel") * zoomMultiplier;
        // Movement
        cameraRigidbody.position += movement * (1 + Mathf.Max(.001f, cameraRigidbody.position.y) / 16);
        // Cap minimum height at 0
        if (cameraRigidbody.position.y < 0)
        {
            cameraRigidbody.position += new Vector3(0, -cameraRigidbody.position.y, 0);
        }
    }
}