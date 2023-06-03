 using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    [SerializeField] Transform cameraTransform;

    public float movementSpeed;
    public float movementTime;

    public Vector3 newPosition;

    public Vector3 dragStartPosition;
    public Vector3 dragCurrentPosition;
    public Vector3 zoomAmount;
    public Vector3 newZoom;
    public Vector3 rotateStartPosition;
    public Vector3 rotateCurrentPosition;
    private Quaternion newRotation;

    // Start is called before the first frame update
    void Start() {
        //this.newPosition = transform.position;
        //this.newRotation = transform.rotation;
        //newZoom = cameraTransform.localPosition; // local in order to stay relative to the rig
    }

    // Update is called once per frame
    void Update() {
        HandleMoventInput();
        HandleMouseInput();
        /* Potition the camera */
       // transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementSpeed);
        //HandleMoventInput();
    }

    private void HandleMouseInput() {
        /* Fix the Camera in order to zoom */
        if (Input.mouseScrollDelta.y != 0) {
            newZoom = Input.mouseScrollDelta.y * zoomAmount;
        }
        /* Position the camera where we want with the right click */
        if (Input.GetMouseButtonDown(1)) {
            Plane plane = new Plane(Vector3.up,Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray,out entry)) {
                dragStartPosition = ray.GetPoint(entry);   
            }
        }
        // If the mouse button is still pushed 
        if (Input.GetMouseButtonDown(1)) {
            Plane plane = new Plane(Vector3.up, Vector3.zero);
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            float entry;
            if (plane.Raycast(ray, out entry)) {
                Debug.Log("Here");
                dragCurrentPosition = ray.GetPoint(entry);
                this.newPosition = transform.position + dragStartPosition - dragCurrentPosition;
            }
        }

        /* Rotate the camera using the middle mouse button */
        if (Input.GetMouseButtonDown(2)) {
            rotateStartPosition = Input.mousePosition;
        }
        //if the middle mouse button is still pushed
        if (Input.GetMouseButtonDown(2)) {
            rotateCurrentPosition = Input.mousePosition;
            Vector3 difference = rotateStartPosition - rotateCurrentPosition;
            rotateStartPosition = rotateCurrentPosition;
            newRotation *= Quaternion.Euler(Vector3.up * (-difference.x / 5f));

        }

    }


   private void HandleMoventInput() {
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }

}
