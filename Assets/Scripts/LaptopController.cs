using System.Collections;
using UnityEngine;
using TMPro;

public class LaptopController : InteractiveObject
{
    [SerializeField] [TextArea(3, 4)] private string hintText;
    [SerializeField] private Transform laptopCover, viewPos;
    [SerializeField] private TMP_Text text, GameStageText;
    private float money;
    private bool isOpen, isUsing;
    private CameraController cameraController;
    private bool _isInFocus;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && isUsing && _isInFocus)
        {
            cameraController.ChangeView(null);
            cameraController.UnlockCamera();
            cameraController.LockCursor();
            isUsing = false;
        }
    }

    public override void Interact(GameObject plr)
    {
        if (!_isInFocus) return;
        base.Interact(plr);
        cameraController = plr.transform.parent.GetComponent<CameraController>();
        cameraController.ChangeView(viewPos);
        cameraController.LockCamera();
        cameraController.UnlockCursor();
        isUsing = true;
    }

    public void SetInFocus(bool value)
    {
        _isInFocus = value;
    }
    
    public void OpenClose()
    {
        laptopCover.localRotation = Quaternion.Euler(isOpen ? 90 : 0, 0, 0);
        isOpen = !isOpen;
    }

    public void OpenNews()
    {
        text.text = "�� ����� ���! ��� ��� �������, ��?)";
    }

    public void OpenStockMarket()
    {
        text.text = "� ��� �����" + money + "������� ��������";
    }

    public void AddMoney(int money)
    {
        this.money += money;
    }

    public override string PrintHelp()
    {
        return hintText;
    }

    public void SetGameStage(string type)
    {
        GameStageText.text = type;
    }
}
