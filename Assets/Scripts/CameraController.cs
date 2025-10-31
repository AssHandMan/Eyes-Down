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

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();
        Cursor.visible = false;
        playerCameraInstance = Instantiate(playerCameraPrefab);
        var vcam = playerCameraInstance.GetComponent<CinemachineCamera>();
        vcam.Target.TrackingTarget = head;

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
}
