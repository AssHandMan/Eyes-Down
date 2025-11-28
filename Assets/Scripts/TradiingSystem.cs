using UnityEngine;

public class TradiingSystem : MonoBehaviour
{
    [SerializeField] private GameObject panel;
    private bool _isTrade= false;
    private void Start()
    {
        panel = GameObject.FindGameObjectWithTag("Trade");
        panel.SetActive(false);
    }
    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Q))
        {
            _isTrade = !_isTrade;
            panel.SetActive(_isTrade);
        }
    }
}
