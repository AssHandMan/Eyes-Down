using System.Collections;
using UnityEngine;
using TMPro;

public class LaptopController : InteractiveObject
{
    [SerializeField] private Transform laptopCover, viewPos;
    [SerializeField] private TMP_Text text;
    private float money;
    private bool isOpen, isUsing;
    private CameraController cameraController;
    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) & isUsing)
        {
            cameraController.ChangeView(null);
            isUsing = false;
        }
    }

    public override void Interact(GameObject plr)
    {
        base.Interact(plr);
        cameraController = plr.transform.parent.GetComponent<CameraController>();
        cameraController.ChangeView(viewPos);
        isUsing = true;
    }

    public void OpenClose()
    {
        laptopCover.localRotation = Quaternion.Euler(isOpen ? 90 : 0, 0, 0);
        isOpen = !isOpen;
    }

    public void OpenNews()
    {
        text.text = "Вы такой лох! Вот это новость, да?)";
    }

    public void OpenStockMarket()
    {
        text.text = "У вас ровно" + money + "русских долларов";
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }
}
