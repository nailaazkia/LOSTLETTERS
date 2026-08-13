using UnityEngine;

public class AspectRatioController : MonoBehaviour
{
    [Header("Target Aspect Ratio")]
    public float targetAspectWidth = 16f;
    public float targetAspectHeight = 9f;

    private int lastScreenWidth = 0;
    private int lastScreenHeight = 0;
    private Camera cam;

    void Awake()
    {
        cam = GetComponent<Camera>();
        if (cam == null) cam = Camera.main;

        UpdateCameraAspectRatio();
    }

    void Update()
    {
        // Jika resolusi atau ukuran window layar laptop berubah saat game berjalan, update kamera otomatis
        if (Screen.width != lastScreenWidth || Screen.height != lastScreenHeight)
        {
            UpdateCameraAspectRatio();
        }
    }

    public void UpdateCameraAspectRatio()
    {
        if (cam == null) return;

        lastScreenWidth = Screen.width;
        lastScreenHeight = Screen.height;

        float targetAspect = targetAspectWidth / targetAspectHeight;
        float windowAspect = (float)Screen.width / (float)Screen.height;
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f)
        {
            // Layar lebih tinggi/sempit dari 16:9 (Letterbox - bar hitam di atas & bawah)
            Rect rect = cam.rect;
            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;
            cam.rect = rect;
        }
        else
        {
            // Layar lebih lebar dari 16:9 (Pillarbox - bar hitam di kiri & kanan)
            float scaleWidth = 1.0f / scaleHeight;
            Rect rect = cam.rect;
            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;
            cam.rect = rect;
        }
    }
}
