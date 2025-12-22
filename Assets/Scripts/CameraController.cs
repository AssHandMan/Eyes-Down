using System;
using Mirror;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Animations;
using System.Collections;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject playerCameraPrefab;
    [SerializeField] private Transform head;
    private GameObject _playerCameraInstance;
    private CinemachineInputAxisController _axisController;
    private CinemachinePanTilt _panTilt;
    private CinemachineCamera virtualCamera;
    private int _safeCounter;
    private int _safeCameraCounter;
    private bool _isCursorLocked = true;
    private Vector2 _baseSensivity;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        _playerCameraInstance = Instantiate(playerCameraPrefab);
        virtualCamera = _playerCameraInstance.GetComponent<CinemachineCamera>();
        _axisController = _playerCameraInstance.GetComponent<CinemachineInputAxisController>();
        _panTilt = _playerCameraInstance.GetComponent<CinemachinePanTilt>();

        float senseX = _axisController.Controllers[0].Input.Gain;
        float senseY = _axisController.Controllers[1].Input.Gain;
        _baseSensivity = new Vector2(senseX, senseY);
        
        virtualCamera.Target.TrackingTarget = head;

        var headConstraint = head.GetComponent<RotationConstraint>();
        StartCoroutine(AssignConstraintNextFrame(headConstraint, _playerCameraInstance.transform));
    }

    private IEnumerator AssignConstraintNextFrame(RotationConstraint constraint, Transform target)
    {
        yield return null;

        ConstraintSource source = new ConstraintSource
        {
            sourceTransform = target,
            weight = 1f
        };

        constraint.AddSource(source);
        constraint.constraintActive = true;
        constraint.transform.hasChanged = true;
    }


    public override void OnStopClient()
    {
        base.OnStopClient();
        if (isLocalPlayer && _playerCameraInstance != null)
        {
            Destroy(_playerCameraInstance);
        }
    }

    private void Update()
    {
        if (_isCursorLocked && isLocalPlayer)
        {
            if (Cursor.visible)
            {
                Debug.Log($"[Update] Forcing cursor hidden. _safeCounter: {_safeCounter}");
            }
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void ChangeView(Transform viewPos)
    {
        if (virtualCamera.Target.TrackingTarget == head)
        {
            virtualCamera.Target.TrackingTarget = viewPos;
            _playerCameraInstance.transform.rotation = Quaternion.Euler(viewPos.transform.eulerAngles.x, viewPos.transform.eulerAngles.y, viewPos.transform.eulerAngles.z);
        }
        else
        {
            virtualCamera.Target.TrackingTarget = head;
        }
    }
    
    public void LockCursor()
    {
        _safeCounter = Mathf.Max(0, _safeCounter - 1);
        Debug.Log($"[LockCursor] _safeCounter: {_safeCounter}, _isCursorLocked: {_isCursorLocked}");

        if (_safeCounter == 0)
        {
            _isCursorLocked = true;
            Debug.Log("[LockCursor] Cursor locked!");
        }
    }

    public void UnlockCursor()
    {
        _safeCounter++;
        _isCursorLocked = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log($"[UnlockCursor] _safeCounter: {_safeCounter}, Cursor shown");
    }
    
    /// <summary>
    /// Блокирует управление камерой (скрывает курсор)
    /// </summary>
    public void LockCamera()
    {
        _safeCameraCounter++;
        if (!_playerCameraInstance) return;
        
        _axisController.enabled = false;
        _panTilt.enabled = false;
    }

    /// <summary>
    /// Разблокирует управление камерой (показывает курсор)
    /// </summary>
    public void UnlockCamera()
    {
        _safeCameraCounter = Mathf.Max(_safeCameraCounter - 1, 0);
        if (!_playerCameraInstance) return;

        if (_safeCameraCounter == 0)
        {
            _axisController.enabled = true;
            _panTilt.enabled = true;
        }
    }
    
    public void SetRotationSensitivity(float sensitivity)
    {
        if (!_playerCameraInstance) return;

        _axisController.Controllers[0].Input.Gain = _baseSensivity.x * sensitivity / 100f;
        _axisController.Controllers[1].Input.Gain = _baseSensivity.y * sensitivity / 100f;
    }
}
