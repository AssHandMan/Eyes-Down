using Mirror;
using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.Animations;
using System.Collections;

public class CameraController : NetworkBehaviour
{
    [SerializeField] private GameObject playerCameraPrefab;
    [SerializeField] private Transform head;
    private GameObject playerCameraInstance;
    private CinemachineCamera virtualCamera;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        playerCameraInstance = Instantiate(playerCameraPrefab);
        virtualCamera = playerCameraInstance.GetComponent<CinemachineCamera>();
        virtualCamera.Target.TrackingTarget = head;

        var headConstraint = head.GetComponent<RotationConstraint>();
        StartCoroutine(AssignConstraintNextFrame(headConstraint, playerCameraInstance.transform));
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
        if (isLocalPlayer && playerCameraInstance != null)
        {
            Destroy(playerCameraInstance);
        }
    }

    public void ChangeView(Transform viewPos)
    {
        if (virtualCamera.Target.TrackingTarget == head)
        {
            virtualCamera.Target.TrackingTarget = viewPos;
            playerCameraInstance.GetComponent<CinemachineInputAxisController>().enabled = false;
            playerCameraInstance.GetComponent<CinemachinePanTilt>().enabled = false;
            playerCameraInstance.transform.rotation = Quaternion.Euler(viewPos.transform.eulerAngles.x, viewPos.transform.eulerAngles.y, viewPos.transform.eulerAngles.z);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            virtualCamera.Target.TrackingTarget = head;
            playerCameraInstance.GetComponent<CinemachineInputAxisController>().enabled = true;
            playerCameraInstance.GetComponent<CinemachinePanTilt>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }

    public void LockCameraController()
    {
        if (!Cursor.visible)
        {
            playerCameraInstance.GetComponent<CinemachineInputAxisController>().enabled = false;
            playerCameraInstance.GetComponent<CinemachinePanTilt>().enabled = false;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
        else
        {
            playerCameraInstance.GetComponent<CinemachineInputAxisController>().enabled = true;
            playerCameraInstance.GetComponent<CinemachinePanTilt>().enabled = true;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
