using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class CameraTransitionManager : MonoBehaviour
{
    [Header("Transition Settings")]
    public Image fadeImage; // 拖拽Panel到这里（Panel有Image组件）
    public float transitionDuration = 1f; // 过渡时间
    public Color fadeColor = Color.white; // 过渡颜色
    
    private static CameraTransitionManager instance;
    
    void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    
    void Start()
    {
        if (fadeImage != null)
        {
            fadeImage.color = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        }
    }
    
    public static void TransitionCameras(Camera fromCamera, Camera toCamera, float duration = 1f)
    {
        if (instance != null)
        {
            instance.StartCoroutine(instance.DoTransition(fromCamera, toCamera, duration));
        }
    }
    
    IEnumerator DoTransition(Camera fromCamera, Camera toCamera, float duration)
    {
        Debug.Log("开始摄像机过渡");
        
        // 淡入白场
        yield return StartCoroutine(FadeIn(duration * 0.5f));
        
        // 切换摄像机
        if (fromCamera != null) fromCamera.enabled = false;
        if (toCamera != null) toCamera.enabled = true;
        
        Debug.Log("摄像机已切换");
        
        // 淡出白场
        yield return StartCoroutine(FadeOut(duration * 0.5f));
        
        Debug.Log("摄像机过渡完成");
    }
    
    IEnumerator FadeIn(float duration)
    {
        if (fadeImage == null) yield break;
        
        float elapsed = 0f;
        Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        Color endColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        
        fadeImage.color = endColor;
    }
    
    IEnumerator FadeOut(float duration)
    {
        if (fadeImage == null) yield break;
        
        float elapsed = 0f;
        Color startColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 1f);
        Color endColor = new Color(fadeColor.r, fadeColor.g, fadeColor.b, 0f);
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            fadeImage.color = Color.Lerp(startColor, endColor, t);
            yield return null;
        }
        
        fadeImage.color = endColor;
    }
}