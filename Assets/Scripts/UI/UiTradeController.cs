using System;
using UnityEngine;

public class UiTradeController : MonoBehaviour
{
    public bool _StocksListOff = true;
    public GameObject _prefabStock;
    public GameObject _profileParent;
    public Wallet _wallet;
    [Header("Pages")]
    public GameObject _trade;
    public GameObject _profile;
    public GameObject _stock;
    public void TradeOnOff(GameObject page) 
    {

        ClosePage();
        page.SetActive(true);
        CreateStock();
    }

    private void ClosePage()
    {
        _trade.SetActive(false);
        _stock.SetActive(false);
        _profile.SetActive(false);
    }

    private void CreateStock()
    {
        foreach(var item in _wallet._stocks) 
        {
            GameObject stock = Instantiate(_prefabStock, _profileParent.transform);
            stock.GetComponent<Stock>().change(item);
        }
    }
    
}
