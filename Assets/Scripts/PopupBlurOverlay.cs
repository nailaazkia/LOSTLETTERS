using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(RectTransform))]
public class PopupBlurOverlay : MonoBehaviour
{
    [Header("Blur & Overlay Settings")]
    [Range(1f, 8f)]
    public float blurSize = 3.5f;
    public Color overlayWhiteColor = new Color(1f, 1f, 1f, 0.40f);

    private RawImage blurBackground;
    private RenderTexture blurRT;
    private Material blurMaterial;
    private CanvasGroup canvasGroup;

    void Awake()
    {
        SetupOverlay();
    }

    void SetupOverlay()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        Transform existingBg = transform.Find("WhiteBlurBackground");
        if (existingBg != null)
        {
            blurBackground = existingBg.GetComponent<RawImage>();
        }
        else
        {
            GameObject bgObj = new GameObject("WhiteBlurBackground");
            bgObj.transform.SetParent(transform, false);
            blurBackground = bgObj.AddComponent<RawImage>();

            RectTransform rect = blurBackground.rectTransform;
            rect.anchorMin = Vector2.zero;
            rect.anchorMax = Vector2.one;
            rect.offsetMin = Vector2.zero;
            rect.offsetMax = Vector2.zero;
        }

        blurBackground.transform.SetAsFirstSibling();
        blurBackground.raycastTarget = true; // Blokir klik ke elemen gameplay di belakang popup

        Shader shader = Shader.Find("UI/WhiteFrostedBlur");
        if (shader != null)
        {
            if (blurMaterial == null) blurMaterial = new Material(shader);
            blurBackground.material = blurMaterial;
        }
        else
        {
            blurBackground.color = overlayWhiteColor;
        }
    }

    void OnEnable()
    {
        if (blurBackground == null) SetupOverlay();
        CaptureAndBlurScreen();
    }

    void OnDisable()
    {
        if (blurRT != null)
        {
            blurRT.Release();
            Destroy(blurRT);
            blurRT = null;
        }
    }

    void OnDestroy()
    {
        if (blurMaterial != null) Destroy(blurMaterial);
        if (blurRT != null)
        {
            blurRT.Release();
            Destroy(blurRT);
        }
    }

    public void CaptureAndBlurScreen()
    {
        Camera cam = Camera.main;
        if (cam == null) return;

        bool oldBgActive = blurBackground != null && blurBackground.gameObject.activeSelf;
        if (blurBackground != null) blurBackground.gameObject.SetActive(false);

        float oldAlpha = canvasGroup != null ? canvasGroup.alpha : 1f;
        if (canvasGroup != null) canvasGroup.alpha = 0f;

        int width = Mathf.Max(1, Screen.width / 4);
        int height = Mathf.Max(1, Screen.height / 4);

        if (blurRT != null && (blurRT.width != width || blurRT.height != height))
        {
            blurRT.Release();
            Destroy(blurRT);
            blurRT = null;
        }

        if (blurRT == null)
        {
            blurRT = new RenderTexture(width, height, 16, RenderTextureFormat.ARGB32);
            blurRT.filterMode = FilterMode.Bilinear;
        }

        RenderTexture oldTarget = cam.targetTexture;
        cam.targetTexture = blurRT;
        cam.Render();
        cam.targetTexture = oldTarget;

        if (canvasGroup != null) canvasGroup.alpha = oldAlpha;
        if (blurBackground != null)
        {
            blurBackground.gameObject.SetActive(oldBgActive);
            blurBackground.texture = blurRT;
            blurBackground.color = Color.white;

            if (blurMaterial != null)
            {
                blurMaterial.SetFloat("_BlurSize", blurSize);
                blurMaterial.SetColor("_Color", overlayWhiteColor);
            }
            else
            {
                blurBackground.color = overlayWhiteColor;
            }
        }
    }
}
