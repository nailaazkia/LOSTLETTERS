using UnityEngine;

public class RandomSpawn : MonoBehaviour
{
    [Header("Bar yang diacak")]
    public RectTransform[] bars;

    [Header("Area Random")]
    public float minX = -500f;
    public float maxX = 500f;

    public float minY = -150f;
    public float maxY = 150f;

    void Start()
    {
        RandomizeBars();
    }

    void RandomizeBars()
    {
        foreach (RectTransform bar in bars)
        {
            float randomX = Random.Range(minX, maxX);
            float randomY = Random.Range(minY, maxY);

            bar.anchoredPosition = new Vector2(randomX, randomY);

            Debug.Log(bar.name + " diacak");
        }
    }
}