using UnityEngine;

public class ObjectShowHider : MonoBehaviour
{
    public GameObject[] hideObj, showObj;
    public void ShowHide()
    {
        for (int i = 0; i < showObj.Length; i++) { showObj[i].SetActive(true); }
        for (int i = 0; i < hideObj.Length; i++) { hideObj[i].SetActive(false); }
    }
}
