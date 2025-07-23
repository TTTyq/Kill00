using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;

public class PlayerMoveController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform[] pathPoints; // 路径点数组
    public float moveSpeed = 5f;
    public float rotationSpeed = 10f; // 转向速度
    
    [Header("Camera Settings")]
    public Camera playerCamera;
    public Camera targetCamera;
    
    [Header("UI")]
    public Button runButton;
    
    private bool isMoving = false;
    private MouseLookController mouseLookController;
    private Keyboard keyboard;
    
    void Start()
    {
        mouseLookController = GetComponent<MouseLookController>();
        keyboard = Keyboard.current;
        
        if (runButton != null)
        {
            runButton.onClick.AddListener(StartMove);
        }
        
        if (playerCamera != null) playerCamera.enabled = true;
        if (targetCamera != null) targetCamera.enabled = false;
    }
    
    void Update()
    {
        if (keyboard != null && keyboard.rKey.wasPressedThisFrame && !isMoving)
        {
            StartMove();
        }
    }
    
    public void StartMove()
    {
        if (!isMoving && pathPoints != null && pathPoints.Length > 0)
        {
            Debug.Log("开始沿路径移动");
            StartCoroutine(MoveAlongPath());
        }
        else
        {
            Debug.LogError("路径点未设置或为空！");
        }
    }
    
    IEnumerator MoveAlongPath()
    {
        isMoving = true;
        
        if (runButton != null)
        {
            runButton.gameObject.SetActive(false); // 隐藏按钮
        }
        
        // 创建完整路径（包含当前位置作为起点）
        List<Vector3> fullPath = new List<Vector3>();
        fullPath.Add(transform.position); // 起点
        
        foreach (Transform point in pathPoints)
        {
            if (point != null)
                fullPath.Add(point.position);
        }
        
        // 沿着每个路径段移动
        for (int i = 0; i < fullPath.Count - 1; i++)
        {
            Vector3 startPos = fullPath[i];
            Vector3 endPos = fullPath[i + 1];
            
            yield return StartCoroutine(MoveToPoint(startPos, endPos));
        }
        
        Debug.Log("路径移动完成，切换摄像机");
        SwitchCamera();
        
        // 按钮保持隐藏状态，不重新显示
        
        isMoving = false;
    }
    
    IEnumerator MoveToPoint(Vector3 startPos, Vector3 endPos)
    {
        float journeyLength = Vector3.Distance(startPos, endPos);
        if (journeyLength < 0.1f) yield break;
        
        float journeyTime = journeyLength / moveSpeed;
        float journey = 0f;
        
        // 计算移动方向
        Vector3 direction = (endPos - startPos).normalized;
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        
        while (journey < journeyTime)
        {
            journey += Time.deltaTime;
            float fractionOfJourney = journey / journeyTime; // 直接使用线性进度，不用曲线
            
            // 移动位置 - 匀速移动
            transform.position = Vector3.Lerp(startPos, endPos, fractionOfJourney);
            
            // 平滑转向移动方向
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            
            yield return null;
        }
        
        // 确保到达终点和正确朝向
        transform.position = endPos;
        transform.rotation = targetRotation;
    }
    
    void SwitchCamera()
    {
        if (playerCamera != null && targetCamera != null)
        {
            Debug.Log("开始摄像机过渡");
            // 使用过渡管理器切换摄像机
            CameraTransitionManager.TransitionCameras(playerCamera, targetCamera, 1.5f);
        }
    }
    
    public void ResetToStart()
    {
        if (!isMoving)
        {
            if (playerCamera != null && targetCamera != null)
            {
                playerCamera.enabled = true;
                targetCamera.enabled = false;
            }
            
            if (runButton != null)
            {
                runButton.interactable = true;
            }
        }
    }
}


