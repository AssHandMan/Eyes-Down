using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class GraphRenderer : MonoBehaviour
{
    private LineRenderer lr;
    private RectTransform rectTransform;

    [Header("Настройки графика (автоматически подстраиваются под размер)")]
    public Color lineColor = new Color(0f, 1f, 0.8f, 1f);
    public float lineWidth = 5f;
    public float verticalPadding = 20f; // отступы сверху и снизу в пикселях

    private void Awake()
    {
        lr = GetComponent<LineRenderer>();
        rectTransform = GetComponent<RectTransform>();

        // КРИТИЧНО для билда: используем шейдер, который работает везде
        lr.material = new Material(Shader.Find("UI/Default")); // или "Sprites/Default"
        lr.startColor = lineColor;
        lr.endColor = lineColor;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.useWorldSpace = false;
        lr.positionCount = 0;
        lr.sortingOrder = 10;
    }

    public void RenderGraph(List<float> prices)
    {
        if (prices == null || prices.Count < 2 || rectTransform == null)
        {
            lr.positionCount = 0;
            return;
        }

        int count = Mathf.Min(prices.Count, 180);
        List<float> data = prices.GetRange(prices.Count - count, count);

        lr.positionCount = count;

        // Автоматически берём размер GraphHolder
        float width = rectTransform.rect.width;
        float height = rectTransform.rect.height - verticalPadding * 2; // отступы

        // Находим мин и макс
        float min = data[0];
        float max = data[0];
        foreach (float p in data)
        {
            if (p < min) min = p;
            if (p > max) max = p;
        }

        float range = Mathf.Max(max - min, 0.01f);

        // Рисуем точки
        for (int i = 0; i < count; i++)
        {
            float x = (float)i / (count - 1) * width - (width / 2f); // от -width/2 до +width/2
            float normalizedY = (data[i] - min) / range;
            float y = normalizedY * height - (height / 2f) + verticalPadding; // центрируем + отступ

            lr.SetPosition(i, new Vector3(x, y, 0));
        }
    }

    // Перерисовываем при изменении размера (важно в билде!)
    private void OnRectTransformDimensionsChange()
    {
        // Если текущая акция выбрана — перерисовываем
        StockUI ui = FindObjectOfType<StockUI>();
        if (ui != null && ui.currentStock != null)
        {
            var history = StockManager.Instance.GetHistory(ui.currentStock.name);
            if (history != null)
            {
                List<float> list = new List<float>(history);
                if (list.Count > 180) list.RemoveRange(0, list.Count - 180);
                RenderGraph(list);
            }
        }
    }
}