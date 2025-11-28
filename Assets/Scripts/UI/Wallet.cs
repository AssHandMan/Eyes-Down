using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Wallet : MonoBehaviour
{
    [SerializeField] public float Cash { get; set; } = 1000;
    [SerializeField] public List<Stock> _stocks = new List<Stock>();
    [SerializeField] private Text wallet;

    private void Start()
    {
        wallet.text = Cash.ToString();
    }
    public void Buy(Stock stock)
    {
        if(Cash - stock.CurrentPrice > 0)
        {
            Cash-=stock.CurrentPrice;
            _stocks.Add(stock);
        }
        UpdWallet();
    }
    private void UpdWallet()
    {
        wallet.text = Cash.ToString();
    }
}
