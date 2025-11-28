using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StockManager : MonoBehaviour
{
    public List<Stock> stocks = new List<Stock>();

    private void Start()
    {
        // Создаем 5 акций
        string[] stockNames = { "IT", "Minerals", "Gaz", "Banking", "Chimestry" };

        foreach (string name in stockNames)
        {
            Stock stock = new Stock(name);
            stocks.Add(stock);
        }

        // Запускаем обновление цен каждую секунду
        StartCoroutine(UpdateStockPrices());
    }

    private IEnumerator UpdateStockPrices()
    {
        while (true)
        {
            yield return new WaitForSeconds(1f);

            foreach (Stock stock in stocks)
            {
                stock.UpdatePrice();
            }

            // Обновляем UI если он открыт
            //if (StockUI.Instance != null && StockUI.Instance.isActiveAndEnabled)
            //{
            //    StockUI.Instance.UpdateStockPrices();
            //}
        }
    }

    public Stock GetStockByName(string name)
    {
        return stocks.Find(stock => stock.name == name);
    }
}