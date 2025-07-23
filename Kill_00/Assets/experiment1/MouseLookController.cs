using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;

public class MouseLookController : MonoBehaviour
{
    [Header("Mouse Settings")]
    public float mouseSensitivity = 2f;
    public float verticalClampMin = -90f;
    public float verticalClampMax = 90f;
    
    private float xRotation = 0f;
    private Camera playerCamera;
    private Mouse mouse;
    private Keyboard keyboard;
    
    void Start()
    {
        // 不锁定鼠标，让它可以自由移动
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        
        playerCamera = GetComponentInChildren<Camera>();
        mouse = Mouse.current;
        keyboard = Keyboard.current;
    }
    
    void Update()
    {
        if (mouse == null) return;
        
        HandleMouseLook();
        
        // ESC键可以选择性地锁定/解锁鼠标（可选功能）
        if (keyboard != null && keyboard.escapeKey.wasPressedThisFrame)
        {
            if (Cursor.lockState == CursorLockMode.Locked)
                Cursor.lockState = CursorLockMode.None;
            else
                Cursor.lockState = CursorLockMode.Locked;
        }
    }
    
    void HandleMouseLook()
    {
        // 只有在右键按下时才控制摄像机
        if (!mouse.rightButton.isPressed) return;
        
        // 如果鼠标在UI上，也不控制摄像机
        if (EventSystem.current != null && EventSystem.current.IsPointerOverGameObject())
            return;
        
        Vector2 mouseDelta = mouse.delta.ReadValue() * mouseSensitivity * Time.deltaTime;
        
        // 水平旋转
        transform.Rotate(Vector3.up * mouseDelta.x);
        
        // 垂直旋转
        xRotation -= mouseDelta.y;
        xRotation = Mathf.Clamp(xRotation, verticalClampMin, verticalClampMax);
        
        if (playerCamera != null)
        {
            playerCamera.transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        }
    }
}
