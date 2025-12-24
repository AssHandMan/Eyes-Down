using Mirror;
using System;
using UnityEngine;

[Serializable]
public class Stock
{
    public string name;

    [SyncVar(hook = nameof(OnPriceChanged))]
    public float currentPrice;

    // Событие для обновления UI кнопок при изменении цены
    public event Action<float> PriceChanged;

    public Stock() { }

    public Stock(string stockName, float initialPrice)
    {
        name = stockName;
        currentPrice = initialPrice;
    }

    private void OnPriceChanged(float oldPrice, float newPrice)
    {
        PriceChanged?.Invoke(newPrice);
    }
}