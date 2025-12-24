using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class PlayerStockHandler : NetworkBehaviour
{
    [SyncVar] public float money = 100000f;

    public Dictionary<string, int> portfolio = new Dictionary<string, int>();

    public System.Action OnPortfolioChanged;

    public override void OnStartServer()
    {
        foreach (var stock in StockManager.Instance.stocks)
            portfolio[stock.name] = 0;
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();

        if (isServer)
            return; // хосту не нужно отправлять себе

        CmdRequestInitialStocks();
    }

    [Command]
    private void CmdRequestInitialStocks()
    {
        TargetSendInitialStocks(connectionToClient);
    }

    [TargetRpc]
    private void TargetSendInitialStocks(NetworkConnection target)
    {
        Debug.Log("[PlayerStockHandler] Запускаю создание кнопок на клиенте");
        Debug.Log(StockManager.Instance.tst + "Realy ");

        StockUI ui = FindObjectOfType<StockUI>();
        if (ui != null)
        {
            ui.BuildStockButtons(); // теперь с правильным ожиданием
        }
    }
    [Command]
    public void SellStock(string stockName, int amount)
    {
        if (!isServer) return;

        Stock stock = StockManager.Instance.stocks.Find(s => s.name == stockName);
        if (stock == null || amount <= 0) return;

        if (!portfolio.ContainsKey(stockName) || portfolio[stockName] < amount)
        {
            // Недостаточно акций — можно отправить сообщение клиенту
            TargetNotEnoughStocks(connectionToClient);
            return;
        }

        // Продаём
        float revenue = stock.currentPrice * amount;
        money += revenue;

        portfolio[stockName] -= amount;

        if (portfolio[stockName] == 0)
            portfolio.Remove(stockName); // опционально, для чистоты

        RpcUpdateClient();
    }

    [TargetRpc]
    private void TargetNotEnoughStocks(NetworkConnection target)
    {
        Debug.Log("Недостаточно акций для продажи!");
        // Можно добавить UI-уведомление
    }
    [Command]
    public void BuyStock(string stockName, int amount)
    {
        if (!isServer) return;

        Stock stock = StockManager.Instance.stocks.Find(s => s.name == stockName);
        if (stock == null || amount <= 0) return;

        float cost = stock.currentPrice * amount;
        if (money < cost) return;

        money -= cost;
        if (!portfolio.ContainsKey(stockName))
            portfolio[stockName] = 0;

        portfolio[stockName] += amount;

        RpcUpdateClient();
    }

    [ClientRpc]
    private void RpcUpdateClient()
    {
        OnPortfolioChanged?.Invoke();
    }

    public float GetPortfolioValue()
    {
        float value = 0f;
        foreach (var kvp in portfolio)
        {
            Stock stock = StockManager.Instance?.stocks.Find(s => s.name == kvp.Key);
            if (stock != null)
                value += stock.currentPrice * kvp.Value;
        }
        return value;
    }
}