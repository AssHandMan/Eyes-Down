using EyesDown.Core;
using UnityEngine;
using Mirror;
using R3;

public class PlayerController : NetworkBehaviour
{
    public readonly ReactiveProperty<bool> FocusedOnUI = new ();
    [Header("Camera")]
    public CameraController CameraController;

    [SerializeField] private LaptopController _laptop;
    [SerializeField] private PlayerTakeDropSystem _takeDropSystem;

    private void Awake()
    {
        FocusedOnUI.Subscribe((value) => _laptop.SetInFocus(!value));
        FocusedOnUI.Subscribe((value) => _takeDropSystem.SetIsFocused(!value));
    }

    public void Initialize(SaveLoaderManager saveLoaderManager)
    {
        saveLoaderManager.Sensivity.Subscribe(CameraController.SetRotationSensitivity);
    }
}
