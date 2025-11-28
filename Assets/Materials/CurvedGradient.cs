using UnityEngine;
using UnityEngine.UI;

public class CurvedGradient : MonoBehaviour
{
    public Color colorA = Color.blue;
    public Color colorB = Color.green;
    public Color colorC = Color.red;

    [Range(0, 1)] public float blendFactor = 0.5f;
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private Image image;

    void Start()
    {
        image = GetComponent<Image>();
        UpdateGradient();
    }

    void UpdateGradient()
    {
        Texture2D texture = new Texture2D(256, 128);

        for (int x = 0; x < texture.width; x++)
        {
            float t = (float)x / texture.width;

            // Применяем кривую перехода
            float curvedT = transitionCurve.Evaluate(t);

            Color color;
            if (t < blendFactor)
            {
                float localT = t / blendFactor;
                localT = transitionCurve.Evaluate(localT);
                color = Color.Lerp(colorA, colorB, localT);
            }
            else
            {
                float localT = (t - blendFactor) / (1f - blendFactor);
                localT = transitionCurve.Evaluate(localT);
                color = Color.Lerp(colorB, colorC, localT);
            }

            for (int y = 0; y < texture.height; y++)
            {
                texture.SetPixel(x, y, color);
            }
        }

        texture.Apply();
        image.sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);
    }
}