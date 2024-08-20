using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class CameraOrbit : MonoBehaviour
{
    // The target object that the camera will orbit around
    public GameObject target;

    public GameManager gameManager;

    public float distance = 10.0f;
    
    // Sensitivity of the scroll wheel for zooming
    public float scrollSensitivity = 2f;
    
    // Minimum and maximum zoom distances
    public float minDistance = 20f; // Minimum zoom distance
    public float maxDistance = 500f; // Maximum zoom distance

    // Speed of camera rotation around the target
    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    // Limits for the vertical rotation angle
    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    // Current rotation angles
    private float x = 0.0f;
    private float y = 0.0f;

    // Input Actions Asset
    public InputActionAsset inputActions;

    // Input actions for zooming and mouse interactions
    private InputAction zoomAction;
    private InputAction leftClickAction;
    private InputAction rightClickAction;
    private InputAction mouseAction;

    private void Awake()
    {
        // Get actions from the input actions asset
        var gameplayActionMap = inputActions.FindActionMap("Player");
    
        zoomAction = gameplayActionMap.FindAction("Zoom");
        leftClickAction = gameplayActionMap.FindAction("LeftClick");
        rightClickAction = gameplayActionMap.FindAction("RightClick");
        mouseAction = gameplayActionMap.FindAction("Mouse");

        gameManager = FindObjectOfType<GameManager>();
    }

    private void OnEnable()
    {
        // Enable input actions
        zoomAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
        mouseAction.Enable();
    }

    private void OnDisable()
    {
        // Disable input actions
        zoomAction.Disable();
        leftClickAction.Disable();
        rightClickAction.Disable();
        mouseAction.Disable();
    }

    void Start()
    {
        // Initialize rotation angles based on the current transform
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        if (gameManager.gameStarted)
        {
            // Update distance and clamp it between min and max values
            distance -= zoomAction.ReadValue<float>() * scrollSensitivity;
            distance = Mathf.Clamp(distance, minDistance, maxDistance);

            // Rotate the camera around the target when the right mouse button is held down
            if (target && rightClickAction.ReadValue<float>() > 0)
            {
                Vector2 mouseDelta = mouseAction.ReadValue<Vector2>();
                var dpiScale = 1f;
                if (Screen.dpi < 1) dpiScale = 1;
                if (Screen.dpi < 200) dpiScale = 1;
                else dpiScale = Screen.dpi / 200f;

                var pos = Mouse.current.position.ReadValue();
                if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;

                x += mouseDelta.x * xSpeed * 0.02f;
                y -= mouseDelta.y * ySpeed * 0.02f;

                y = ClampAngle(y, yMinLimit, yMaxLimit);
                var rotation = Quaternion.Euler(y, x, 0);
                var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
                transform.rotation = rotation;
                transform.position = position;
            }

            // Update the camera position and rotation based on the target's position
            if (target)
            {
                var rot = Quaternion.Euler(y, x, 0);
                var po = rot * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
                transform.rotation = rot;
                transform.position = po;
            }
        }
    }

    // Clamp the angle between the specified minimum and maximum values
    static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360)
            angle += 360;
        if (angle > 360)
            angle -= 360;
        return Mathf.Clamp(angle, min, max);
    }
    
    public void UpdateCameraPosition()
    {
        Vector2 mouseDelta = mouseAction.ReadValue<Vector2>();
        var dpiScale = 1f;
        if (Screen.dpi < 1) dpiScale = 1;
        if (Screen.dpi < 200) dpiScale = 1;
        else dpiScale = Screen.dpi / 200f;

        var pos = Mouse.current.position.ReadValue();
        if (pos.x < 380 * dpiScale && Screen.height - pos.y < 250 * dpiScale) return;

        x += mouseDelta.x * xSpeed * 0.02f;
        y -= mouseDelta.y * ySpeed * 0.02f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);
        var rotation = Quaternion.Euler(y, x, 0);
        var position = rotation * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
        transform.rotation = rotation;
        transform.position = position;
    }
}
