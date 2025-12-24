using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class StockUI : MonoBehaviour
{
    [Header("=== Left Panel ===")]
    public GameObject stockButtonPrefab;
    public Transform stockListContent;

    [Header("=== Right Panel ===")]
    public GameObject rightPanel;
    public Text stockNameText;
    public Text currentPriceText;
    public Text changeText;
    public GraphRenderer graph;

    [Header("=== Buy Buttons ===")]
    public Button buy1Button;
    public Button buy10Button;
    public Button buy100Button;

    [Header("=== Portfolio Panel (Fixed 5 stocks) ===")]
    public Text moneyText;
    public Text portfolioValueText;
    public Text totalValueText;

    public Text itCountText;
    public Text gazCountText;
    public Text mineralsCountText;
    public Text chymestryCountText;
    public Text bankCountText;

    private PlayerStockHandler playerHandler;
    public Stock currentStock = null;

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        StartCoroutine(WaitForEverythingAndCreate());
    }

    private IEnumerator WaitForEverythingAndCreate()
    {
        // 1. Ждём локального игрока
        while (NetworkClient.localPlayer == null)
            yield return null;

        playerHandler = NetworkClient.localPlayer.GetComponent<PlayerStockHandler>();

        if (playerHandler == null)
        {
            Debug.LogError("[StockUI] PlayerStockHandler не найден!");
            yield break;
        }

        playerHandler.OnPortfolioChanged += UpdatePortfolioUI;
        UpdatePortfolioUI();

        // 2. Ждём StockManager и хотя бы 1 акцию (самое главное!)
        while (StockManager.Instance == null || StockManager.Instance.stocks.Count == 0)
        {
            Debug.Log("[StockUI] Ждём StockManager и акции... (0.3 сек)");
            yield return new WaitForSeconds(0.3f);
        }

        BuildStockButtons();
    }
    public void Sell100()
    {
        TrySell(100);
    }
    private void TrySell(int amount)
    {
        if (currentStock == null || playerHandler == null) return;

        playerHandler.SellStock(currentStock.name, amount);
    }
    public void BuildStockButtons()
    {
        // Очищаем
        foreach (Transform child in stockListContent)
            Destroy(child.gameObject);

        Debug.Log($"[StockUI] Создаю {StockManager.Instance.stocks.Count} кнопок");

        foreach (var stock in StockManager.Instance.stocks)
        {
            GameObject btn = Instantiate(stockButtonPrefab, stockListContent);
            btn.name = $"Btn_{stock.name}";

            Text nameT = btn.transform.Find("NameText")?.GetComponent<Text>();
            Text priceT = btn.transform.Find("PriceText")?.GetComponent<Text>();
            Text changeT = btn.transform.Find("ChangeText")?.GetComponent<Text>();

            var handler = btn.GetComponent<StockButtonHandler>() ?? btn.AddComponent<StockButtonHandler>();

            handler.stock = stock;
            handler.nameText = nameT;
            handler.priceText = priceT;
            handler.changeText = changeT;
            handler.onClick = () => ShowStockDetails(stock);

            handler.RefreshDisplay();
        }
    }

    public void ShowStockDetails(Stock stock)
    {
        currentStock = stock;
        rightPanel?.SetActive(true);

        if (stockNameText != null)
            stockNameText.text = stock.name.ToUpper();

        if (currentPriceText != null)
            currentPriceText.text = $"${stock.currentPrice:F2}";

        UpdateRightPanel();
    }

    public void Buy1() { TryBuy(1); }
    public void Buy10() { TryBuy(10); }
    public void Buy100() { TryBuy(100); }

    private void TryBuy(int amount)
    {
        if (currentStock == null || playerHandler == null) return;
        playerHandler.BuyStock(currentStock.name, amount);
    }

    private void Update()
    {
        if (StockManager.Instance == null) return;

        if (currentStock != null)
            UpdateRightPanel();

        UpdatePortfolioUI();
    }

    private void UpdateRightPanel()
    {
        if (currentStock == null) return;

        if (currentPriceText != null)
            currentPriceText.text = $"${currentStock.currentPrice:F2}";

        if (changeText != null)
        {
            var history = StockManager.Instance.GetHistory(currentStock.name);
            if (history != null && history.Count >= 2)
            {
                float prev = history[history.Count - 2];
                float delta = currentStock.currentPrice - prev;
                float percent = prev != 0f ? delta / prev * 100f : 0f;

                string sign = delta >= 0 ? "+" : "";
                changeText.text = $"{sign}{delta:F2} ({sign}{percent:F1}%)";
                changeText.color = delta >= 0 ? Color.green : Color.red;
            }
            else
            {
                changeText.text = "—";
                changeText.color = Color.gray;
            }
        }

        if (graph != null)
        {
            var history = StockManager.Instance.GetHistory(currentStock.name);
            if (history != null && history.Count > 0)
            {
                var list = new List<float>(history);
                if (list.Count > 180) list.RemoveRange(0, list.Count - 180);
                graph.RenderGraph(list);
            }
        }
    }

    private void UpdatePortfolioUI()
    {
        if (playerHandler == null) return;

        if (moneyText != null)
            moneyText.text = $"Cash: ${playerHandler.money:F2}";

        float portfolioValue = playerHandler.GetPortfolioValue();
        if (portfolioValueText != null)
            portfolioValueText.text = $"Portfolio: ${portfolioValue:F2}";

        if (totalValueText != null)
            totalValueText.text = $"Total: ${playerHandler.money + portfolioValue:F2}";

        UpdateStockCountText(itCountText, playerHandler, "IT");
        UpdateStockCountText(gazCountText, playerHandler, "Gaz");
        UpdateStockCountText(mineralsCountText, playerHandler, "Minerals");
        UpdateStockCountText(chymestryCountText, playerHandler, "Chymestry");
        UpdateStockCountText(bankCountText, playerHandler, "Bank");
    }

    private void UpdateStockCountText(Text textField, PlayerStockHandler player, string stockName)
    {
        if (textField == null) return;

        int count = player.portfolio.ContainsKey(stockName) ? player.portfolio[stockName] : 0;

        if (count > 0)
        {
            var stock = StockManager.Instance.stocks.Find(s => s.name == stockName);
            float value = stock != null ? stock.currentPrice * count : 0f;
            textField.text = $"<b>{stockName}</b> × {count} — <color=#00DDFF>${value:F2}</color>";
            textField.color = Color.white;
        }
        else
        {
            textField.text = $"<b>{stockName}</b> × 0";
            textField.color = new Color(0.6f, 0.6f, 0.6f, 0.7f);
        }
    }
}