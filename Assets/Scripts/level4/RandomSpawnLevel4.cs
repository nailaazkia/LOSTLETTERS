using UnityEngine;

public class RandomSpawnLevel4 : MonoBehaviour
{
    public RectTransform spawnArea;
    public RectTransform[] pencils;

    void Start()
    {
        foreach (RectTransform pencil in pencils)
        {
            pencil.SetParent(spawnArea);

            float x = Random.Range(-350f, 350f);
            float y = Random.Range(-120f, 120f);

            pencil.anchoredPosition =
                new Vector2(x, y);
        }
    }
}