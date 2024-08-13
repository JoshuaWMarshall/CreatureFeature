using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraOrbit : MonoBehaviour
{
    public GameObject target;
    public float distance = 10.0f;
    public float scrollSensitivity = 2f;
    public float minDistance = 20f; // Minimum zoom distance
    public float maxDistance = 500f; // Maximum zoom distance

    public float xSpeed = 250.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20;
    public float yMaxLimit = 80;

    private float x = 0.0f;
    private float y = 0.0f;

    private float prevDistance;

    // Input Actions Asset
    public InputActionAsset inputActions;

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
    }

    private void OnEnable()
    {
        zoomAction.Enable();
        leftClickAction.Enable();
        rightClickAction.Enable();
        mouseAction.Enable();
    }

    private void OnDisable()
    {
        zoomAction.Disable();
        leftClickAction.Disable();
        rightClickAction.Disable();
        mouseAction.Disable();
    }

    void Start()
    {
        var angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    void LateUpdate()
    {
        // Update distance and clamp it between min and max values
        distance -= zoomAction.ReadValue<float>() * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);

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
        
        prevDistance = distance;
        var rot = Quaternion.Euler(y, x, 0);
        var po = rot * new Vector3(0.0f, 0.0f, -distance) + target.transform.position;
        transform.rotation = rot;
        transform.position = po;
    }

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
