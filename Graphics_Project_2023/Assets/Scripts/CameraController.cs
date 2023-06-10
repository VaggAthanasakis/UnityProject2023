 using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
public class CameraController : MonoBehaviour {

    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    private float targetFieldOfView = 50;
    private float zoomFieldOfViewMax = 80;
    private float zoomFieldOfViewMin = 20;
    private float minX = 5;
    private float maxX = 85;
    private float minZ = 5;
    private float maxZ = 85;

    [SerializeField] private CinemachineVirtualCamera virtualCamera;

    private void Update() {
        HandleCameraRotation();
        HandleCameraMovement();
        HandleCameraMovementDragPan();
        HandleCameraZoom();
    }

    private void HandleCameraMovement() {
        Vector3 inputDirection = new Vector3(0, 0, 0);

        if (Input.GetKey(KeyCode.W)) inputDirection.z = 1f;
        if (Input.GetKey(KeyCode.S)) inputDirection.z = -1f;
        if (Input.GetKey(KeyCode.A)) inputDirection.x = -1f;
        if (Input.GetKey(KeyCode.D)) inputDirection.x = 1f;

        // move direction based on the object rotation (transform.forward)
        // we use .z because we are at 3d environment
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        float moveSpeed = 20f;
        /* fix the camere within a range */
        Vector3 tmp = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        float clampedX = Mathf.Clamp(tmp.x, minX, maxX);
        float clampedZ = Mathf.Clamp(tmp.z, minZ, maxZ);
        transform.position = new Vector3(clampedX,transform.position.y, clampedZ);
        /**/
        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementDragPan() {
        Vector3 inputDirection = new Vector3(0, 0, 0);
        /* mouse drag camera move with right click */
        if (Input.GetMouseButtonDown(1)) {
            dragPanMoveActive = true;
            lastMousePosition = Input.mousePosition; //store the start position of the drag
        }
        if (Input.GetMouseButtonUp(1)) {
            dragPanMoveActive = false;
        }

        if (dragPanMoveActive) {
            Vector2 mouseMovementDelta = (Vector2)Input.mousePosition - lastMousePosition; // How much the mouse moved in the last frame

            float dragPanSpeed = 0.02f;
            inputDirection.x = -mouseMovementDelta.x * dragPanSpeed;
            inputDirection.z = -mouseMovementDelta.y * dragPanSpeed;
            //Debug.Log(mouseMovementDelta);
            lastMousePosition = Input.mousePosition;
        }

        // move direction based on the object rotation (transform.forward)
        // we use .z because we are at 3d environment
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        float moveSpeed = 20f;
        /* fix the camera do be inside a range */
        Vector3 tmp = transform.position + moveDirection * moveSpeed * Time.deltaTime;
        float clampedX = Mathf.Clamp(tmp.x, minX, maxX);
        float clampedZ = Mathf.Clamp(tmp.z, minZ, maxZ);
        transform.position = new Vector3(clampedX, transform.position.y, clampedZ);

        //transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    /* Handle camera rotation */
    private void HandleCameraRotation() {
        // the camera is always  behind this gameobject, so if we want rotaton we just rotate this gameobject
        float rotateDirection = 0f;
        float rotateSpeed = 100f;

        if (Input.GetKey(KeyCode.Q)) {
            rotateDirection = 1f;
        }
        if (Input.GetKey(KeyCode.E)) {
            rotateDirection = -1f;
        }

        transform.eulerAngles += new Vector3(0, rotateDirection * rotateSpeed * Time.deltaTime,0);

    }

    /* Handle camera zoom  using the mouse scroll */
    private void HandleCameraZoom() {
        if (Input.mouseScrollDelta.y > 0 && targetFieldOfView >= zoomFieldOfViewMin) { // zoom in
            targetFieldOfView -= 5;
        }
        if (Input.mouseScrollDelta.y < 0 && targetFieldOfView <= zoomFieldOfViewMax) { // zoom out
            targetFieldOfView += 5;
        }
        // apply the zoom smoothly
        float zoomSpeed = 10f;
        virtualCamera.m_Lens.FieldOfView = Mathf.Lerp(virtualCamera.m_Lens.FieldOfView,targetFieldOfView,Time.deltaTime * zoomSpeed);
    }

}
