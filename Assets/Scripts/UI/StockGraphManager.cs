using UnityEngine;
using System.Collections.Generic;
using UnityEngine.UI;
using System.Collections;

public class StockGraphManager : MonoBehaviour
{
    [Header("Window References")]
    public GameObject graphWindow;
    public RectTransform graphContainer;
    public Text titleText;
    public Button closeButton;

    [Header("UI Controls")]
    public Dropdown stockSelector;
    public Text currentPriceText;
    public Text minPriceText;
    public Text maxPriceText;
    public Text updateStatusText; // Текст статуса обновления

    [Header("Graph Settings")]
    public int maxDataPoints = 100;
    public Color graphColor = Color.cyan;
    public float lineWidth = 1.5f;

    [Header("Auto Update Settings")]
    public float updateInterval = 1f; // Интервал обновления в секундах
    public bool autoUpdate = true;    // Включить автообновление
    [Header("Options of Stock")]
    public float volatility = 2f;     // Волатильность (макс изменение цены)
    public float _globalMod = 0.1f;

    public LineRenderer lineRenderer;
    public GameObject graphObject;
    private List<float> priceData = new List<float>();
    private string currentStock = "AAPL";
    private Coroutine updateCoroutine; // Ссылка на корутину

    void Start()
    {
        InitializeGraph();
        SetupUIEvents();
        LoadSampleData();

        // Запускаем автоматическое обновление
        if (autoUpdate)
        {
            StartAutoUpdate();
        }

        if (graphWindow != null)
            graphWindow.SetActive(true);
    }

    void InitializeGraph()
    {
        // Настройка LineRenderer
        lineRenderer.positionCount = 0;
        lineRenderer.startWidth = lineWidth;
        lineRenderer.endWidth = lineWidth;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.startColor = graphColor;
        lineRenderer.endColor = graphColor;
        lineRenderer.useWorldSpace = false;

        // Улучшаем сглаживание линии
        lineRenderer.numCapVertices = 5;
        lineRenderer.numCornerVertices = 5;
    }

    // НОВЫЙ МЕТОД: Запуск автоматического обновления
    public void StartAutoUpdate()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
        updateCoroutine = StartCoroutine(AutoUpdateCoroutine());

        UpdateStatusText("Auto Update: ON");
    }

    // НОВЫЙ МЕТОД: Остановка автоматического обновления
    public void StopAutoUpdate()
    {
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
            updateCoroutine = null;
        }

        UpdateStatusText("Auto Update: OFF");
    }

    // НОВЫЙ МЕТОД: Корутина для автоматического обновления
    IEnumerator AutoUpdateCoroutine()
    {
        while (autoUpdate)
        {
            yield return new WaitForSeconds(updateInterval);

            // Генерируем новую цену
            GenerateNewPricePoint();

            // Обновляем текст статуса с временем
            UpdateStatusText($"Last update: {System.DateTime.Now:HH:mm:ss}");
        }
    }

    // НОВЫЙ МЕТОД: Генерация новой точки цены
    void GenerateNewPricePoint()
    {
        if (priceData.Count == 0)
        {
            // Если данных нет, начинаем со случайной цены
            priceData.Add(Random.Range(100f, 200f));
            return;
        }

        float lastPrice = priceData[priceData.Count - 1];

        // Создаём более реалистичное изменение цены
        float change = Random.Range(-volatility, volatility);

        // Добавляем небольшой тренд (50% шанс тренда вверх)
        if (Random.Range(0, 100) < 50)
        {
            change += Random.Range(0f, volatility * 0.5f);
        }

        float newPrice = lastPrice + change;

        // Защита от отрицательных цен
        newPrice = Mathf.Max(newPrice, 1f);

        AddNewPricePoint(newPrice);
    }

    void SetupUIEvents()
    {
        // Кнопка закрытия
        if (closeButton != null)
            closeButton.onClick.AddListener(CloseWindow);

        // Выбор акции
        if (stockSelector != null)
        {
            stockSelector.onValueChanged.AddListener(OnStockSelected);
        }
    }

    void LoadSampleData()
    {
        priceData.Clear();
        float basePrice = 150f;

        // Генерируем начальные данные
        for (int i = 0; i < 20; i++) // Меньше начальных точек для лучшей анимации
        {
            basePrice += Random.Range(-volatility, volatility)*_globalMod;
            basePrice = Mathf.Max(basePrice, 1f);
            priceData.Add(basePrice);
        }

        UpdateGraphVisualization();
        UpdateStatusText("Initial data loaded");
    }

    void UpdateGraphVisualization()
    {
        if (priceData.Count == 0 || graphContainer == null) return;

        float containerWidth = graphContainer.rect.width;
        float containerHeight = graphContainer.rect.height;

        Vector3[] positions = new Vector3[priceData.Count];
        float minPrice = GetMinPrice();
        float maxPrice = GetMaxPrice();
        float priceRange = Mathf.Max(maxPrice - minPrice, 1f);

        for (int i = 0; i < priceData.Count; i++)
        {
            float x = ((float)i / (priceData.Count - 1)) * containerWidth - containerWidth / 2;
            float y = ((priceData[i] - minPrice) / priceRange) * containerHeight - containerHeight / 2;

            positions[i] = new Vector3(x, y, 0);
        }

        lineRenderer.positionCount = priceData.Count;
        lineRenderer.SetPositions(positions);

        UpdateUI();
    }

    void UpdateUI()
    {
        if (priceData.Count > 0)
        {
            float currentPrice = priceData[priceData.Count - 1];
            float previousPrice = priceData.Count > 1 ? priceData[priceData.Count - 2] : currentPrice;
            float change = currentPrice - previousPrice;
            float changePercent = (change / previousPrice) * 100f;

            if (currentPriceText != null)
            {
                string changeColor = change >= 0 ? "#00FF00" : "#FF0000";
                string changeSymbol = change >= 0 ? "↑" : "↓";
                currentPriceText.text = $"${currentPrice:F2} <color={changeColor}>{changeSymbol} {Mathf.Abs(change):F2} ({Mathf.Abs(changePercent):F2}%)</color>";
            }

            if (minPriceText != null)
                minPriceText.text = $"Min: ${GetMinPrice():F2}";

            if (maxPriceText != null)
                maxPriceText.text = $"Max: ${GetMaxPrice():F2}";

            if (titleText != null)
                titleText.text = $"{currentStock} Stock Price";
        }
    }

    // НОВЫЙ МЕТОД: Обновление текста статуса
    void UpdateStatusText(string message)
    {
        if (updateStatusText != null)
        {
            updateStatusText.text = message;
        }
    }

    public void AddNewPricePoint(float newPrice)
    {
        priceData.Add(newPrice);

        if (priceData.Count > maxDataPoints)
        {
            priceData.RemoveAt(0);
        }

        UpdateGraphVisualization();
    }

    // Обработчики UI событий
    public void OnStockSelected(int index)
    {
        if (stockSelector != null)
        {
            currentStock = stockSelector.options[index].text;
            LoadSampleData(); // reload data for new stock

            if (autoUpdate)
            {
                // reaload autoupdate stock(perSec)
                StartAutoUpdate();
            }
        }
    }

    public void OnTimeFrameClicked(string timeFrame)
    {
        Debug.Log($"Time frame changed to: {timeFrame}");
        // Здесь будет логика изменения таймфрейма
        LoadSampleData();
    }

    // НОВЫЕ МЕТОДЫ: Управление автообновлением через UI
    public void ToggleAutoUpdate()
    {
        autoUpdate = !autoUpdate;

        if (autoUpdate)
        {
            StartAutoUpdate();
        }
        else
        {
            StopAutoUpdate();
        }
    }

    public void OnUpdateIntervalChanged(float newInterval)
    {
        updateInterval = Mathf.Max(newInterval, 0.1f); // Минимум 0.1 секунды

        if (autoUpdate)
        {
            // Перезапускаем с новым интервалом
            StartAutoUpdate();
        }
    }

    public void CloseWindow()
    {
        StopAutoUpdate(); // Останавливаем обновление при закрытии
        if (graphWindow != null)
            graphWindow.SetActive(false);
    }

    public void OpenWindow()
    {
        if (graphWindow != null)
        {
            graphWindow.SetActive(true);
            if (autoUpdate)
            {
                StartAutoUpdate();
            }
        }
    }

    void OnDestroy()
    {
        // Важно: останавливаем корутину при уничтожении объекта
        if (updateCoroutine != null)
        {
            StopCoroutine(updateCoroutine);
        }
    }

    // Вспомогательные методы
    float GetMinPrice()
    {
        if (priceData.Count == 0) return 0f;
        float min = float.MaxValue;
        foreach (float price in priceData)
        {
            if (price < min) min = price;
        }
        return min;
    }

    float GetMaxPrice()
    {
        if (priceData.Count == 0) return 0f;
        float max = float.MinValue;
        foreach (float price in priceData)
        {
            if (price > max) max = price;
        }
        return max;
    }

    // Метод для тестирования - добавляет случайную точку
    [ContextMenu("Add Random Data Point")]
    public void AddRandomDataPoint()
    {
        GenerateNewPricePoint();
    }
}