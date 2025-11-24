using UnityEngine;
using TMPro;

public class HintsMsgController : MonoBehaviour
{
    [SerializeField] private GameObject HintMsgObj;
    [SerializeField] private TMP_Text text;

    public void Print(string msg)
    {
        if(!HintMsgObj.activeSelf) { HintMsgObj.SetActive(true); }
        text.text = msg;
    }
    public void Hide()
    {
        HintMsgObj.SetActive(false);
    }
}
