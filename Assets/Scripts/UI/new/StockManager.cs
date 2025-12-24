using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class StockManager : NetworkBehaviour
{
    public static StockManager Instance;
    public string tst = "stockManager is Work";
    public List<Stock> stocks = new List<Stock>();

    private Dictionary<string, SyncList<float>> histories = new Dictionary<string, SyncList<float>>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        InitializeStocks();
        InvokeRepeating(nameof(ServerUpdatePrices), 1f, 1f);
    }

    void InitializeStocks()
    {
        string[] names = { "IT", "Gaz", "Minerals", "Chymestry", "Bank" };
        foreach (var n in names)
        {
            float price = Random.Range(50f, 200f);
            price = Mathf.Round(price * 100f) / 100f;

            var stock = new Stock(n, price);
            stocks.Add(stock);
            
            var history = new SyncList<float>();
            history.Add(price);
            histories[n] = history;
        }
    }

    void ServerUpdatePrices()
    {
        foreach (var stock in stocks)
        {
            float oldPrice = stock.currentPrice;
            float change = Random.Range(-0.05f, 0.05f);
            stock.currentPrice = oldPrice * (1f + change);
            stock.currentPrice = Mathf.Round(stock.currentPrice * 100f) / 100f;

            histories[stock.name].Add(stock.currentPrice);
        }
    }

    public SyncList<float> GetHistory(string name)
    {
        histories.TryGetValue(name, out var list);
        return list;
    }

    [Command]
    public void CmdBuyStock(string stockName, int amount, NetworkConnectionToClient buyerConn)
    {
        Stock stock = stocks.Find(s => s.name == stockName);
        if (stock == null) return;

        float buyPrice = stock.currentPrice;
        Debug.Log($"Player bought {amount} shares of {stockName} at {buyPrice} each.");

        TargetBuyConfirmation(buyerConn, stockName, amount, buyPrice);
    }

    [TargetRpc]
    private void TargetBuyConfirmation(NetworkConnectionToClient target, string stockName, int amount, float price)
    {
        Debug.Log($"Purchase confirmed: {amount} shares of {stockName} at {price}.");
    }
}