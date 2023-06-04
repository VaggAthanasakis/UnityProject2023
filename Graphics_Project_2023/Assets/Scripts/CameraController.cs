 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    private bool dragPanMoveActive;
    private Vector2 lastMousePosition;
    [SerializeField] private bool useEdgeScrolling = false;

    private void Update() {
        HandleCameraRotation();
        HandleCameraMovement();
        HandleCameraMovementDragPan();
        HandleCameraMovementEdgeScrolling();
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
        transform.position += moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraMovementEdgeScrolling() {
        if (useEdgeScrolling) {
            Vector3 inputDirection = new Vector3(0, 0, 0);
            /* Handle edge scrolling */
            int edgeScrollSize = 10;

            if (Input.mousePosition.x < edgeScrollSize) { // if the mouse is at the left side -> move left
                inputDirection.x = -1f;
            }
            if (Input.mousePosition.y < edgeScrollSize) { // if the mouse is at the buttom -> move down
                inputDirection.z = -1f;
            }
            if (Input.mousePosition.x > Screen.width - edgeScrollSize) { // if the mouse is at the right side -> move right
                inputDirection.x = 1f;
            }
            if (Input.mousePosition.y < Screen.height - edgeScrollSize) { // if the mouse is at the top -> move up
                inputDirection.z = 1f;
            }

            // move direction based on the object rotation (transform.forward)
            // we use .z because we are at 3d environment
            Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
            float moveSpeed = 20f;
            transform.position += moveDirection * moveSpeed * Time.deltaTime;
        }
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
            Debug.Log(mouseMovementDelta);
            lastMousePosition = Input.mousePosition;
        }

        // move direction based on the object rotation (transform.forward)
        // we use .z because we are at 3d environment
        Vector3 moveDirection = transform.forward * inputDirection.z + transform.right * inputDirection.x;
        float moveSpeed = 20f;
        transform.position += moveDirection * moveSpeed * Time.deltaTime;


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

    


}
