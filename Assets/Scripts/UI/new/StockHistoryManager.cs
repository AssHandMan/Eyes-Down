using Mirror;
using System.Collections.Generic;
using UnityEngine;

public class StockHistoryManager : NetworkBehaviour
{
    public static StockHistoryManager Instance;

    [System.Serializable]
    public struct SerializableHistory
    {
        public string stockName;
        public float[] priceArray;
        public int count;

        public List<float> ToList()
        {
            var list = new List<float>();
            for (int i = 0; i < count; i++)
            {
                list.Add(priceArray[i]);
            }
            return list;
        }
    }

    [SyncVar(hook = nameof(OnHistoriesChanged))]
    public SerializableHistory[] stockHistories = new SerializableHistory[5];

    private Dictionary<string, int> stockIndexMap = new Dictionary<string, int>();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        string[] names = { "IT", "Gaz", "Minerals", "Chymestry", "Bank" };

        for (int i = 0; i < names.Length; i++)
        {
            stockHistories[i] = new SerializableHistory
            {
                stockName = names[i],
                priceArray = new float[180],
                count = 0
            };
            stockIndexMap[names[i]] = i;
        }
    }

    [Server]
    public void AddPrice(string stockName, float price)
    {
        if (stockIndexMap.TryGetValue(stockName, out int index))
        {
            var history = stockHistories[index];

            if (history.count >= 180)
            {
                // —двигаем массив
                for (int i = 1; i < 180; i++)
                {
                    history.priceArray[i - 1] = history.priceArray[i];
                }
                history.priceArray[179] = price;
            }
            else
            {
                history.priceArray[history.count] = price;
                history.count++;
            }

            stockHistories[index] = history;
        }
    }

    public List<float> GetHistory(string stockName)
    {
        if (stockIndexMap.TryGetValue(stockName, out int index))
        {
            return stockHistories[index].ToList();
        }
        return null;
    }

    private void OnHistoriesChanged(SerializableHistory[] oldHistories, SerializableHistory[] newHistories)
    {
        // ќбновл€ем UI когда истории мен€ютс€
    //    if (StockUI.Instance != null)
    //    {
    //        StockUI.Instance.RefreshUI();
    //    }
    }
}