using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Stock : MonoBehaviour
{
    public string name;
    public List<float> PriceData;
    public float CurrentPrice { get; private set; }

    private string filePath;

    public Stock(string stockName)
    {
        name = stockName;
        PriceData = new List<float>();
        filePath = Path.Combine(Application.persistentDataPath, $"{name}_data.txt");
        GenerateInitialPrice();
    }

    private void GenerateInitialPrice()
    {
        // Начальная цена от 100 до 500
        CurrentPrice = UnityEngine.Random.Range(100f, 500f);
        PriceData.Add(CurrentPrice);
    }

    public void UpdatePrice()
    {
        // Изменение цены на ±5%
        float changePercent = UnityEngine.Random.Range(-0.05f, 0.05f);
        CurrentPrice += CurrentPrice * changePercent;

        // Ограничение минимальной цены
        CurrentPrice = Mathf.Max(CurrentPrice, 1f);

        PriceData.Add(CurrentPrice);
        SaveToFile();
    }

    private void SaveToFile()
    {
        try
        {
            string dataLine = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{CurrentPrice:F2}";
            File.AppendAllText(filePath, dataLine + Environment.NewLine);
        }
        catch (Exception e)
        {
            Debug.LogError($"Ошибка сохранения данных акции {name}: {e.Message}");
        }
    }

    public void LoadPriceHistory()
    {
        if (File.Exists(filePath))
        {
            PriceData.Clear();
            string[] lines = File.ReadAllLines(filePath);
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 2 && float.TryParse(parts[1], out float price))
                {
                    PriceData.Add(price);
                }
            }
        }
    }
    public void change(Stock stock) {  }
}