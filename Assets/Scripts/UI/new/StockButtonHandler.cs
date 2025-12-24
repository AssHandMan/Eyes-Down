using UnityEngine;
using UnityEngine.UI;

public class StockButtonHandler : MonoBehaviour
{
    public Stock stock;
    public Text nameText;
    public Text priceText;
    public Text changeText;

    public System.Action onClick;

    private Button button;

    private void Awake()
    {
        button = GetComponent<Button>();
    }

    private void OnEnable()
    {
        if (button != null)
            button.onClick.AddListener(() => onClick?.Invoke());

        if (stock != null)
            stock.PriceChanged += OnStockPriceChanged;

        RefreshDisplay();
    }

    private void OnDisable()
    {
        if (button != null)
            button.onClick.RemoveListener(() => onClick?.Invoke());

        if (stock != null)
            stock.PriceChanged -= OnStockPriceChanged;
    }

    private void OnStockPriceChanged(float newPrice)
    {
        RefreshDisplay();
    }

    public void RefreshDisplay()
    {
        if (stock == null || StockManager.Instance == null) return;
        if (nameText == null || priceText == null || changeText == null) return;

        nameText.text = stock.name;
        priceText.text = $"${stock.currentPrice:F2}";

        var history = StockManager.Instance.GetHistory(stock.name);
        if (history == null || history.Count == 0)
        {
            changeText.text = "";
            return;
        }

        float prev = history.Count > 1 ? history[history.Count - 2] : stock.currentPrice;
        float change = stock.currentPrice - prev;
        float percent = prev != 0 ? (change / prev) * 100f : 0f;

        string sign = change >= 0 ? "+" : "";
        string color = change >= 0 ? "#00FF88" : "#FF4444";

        changeText.text = $"<color={color}>{sign}{change:F2} ({sign}{percent:F1}%)</color>";
    }
}