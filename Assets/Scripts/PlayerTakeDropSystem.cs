using UnityEngine;
using Mirror;

public class PlayerTakeDropSystem : NetworkBehaviour
{
    [SerializeField] private float holdDistance = 2f;
    [SerializeField] private float holdForce = 200f;
    [SerializeField] private LayerMask pickupLayer;

    private Rigidbody heldRb;

    void Update()
    {
        if(!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 5, pickupLayer))
            {
                Rigidbody rb = hit.collider.attachedRigidbody;
                if (rb != null)
                {
                    heldRb = rb;
                    heldRb.useGravity = false;
                    heldRb.linearDamping = 10f;
                }
            }
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && heldRb != null)
        {
            heldRb.useGravity = true;
            heldRb.linearDamping = 0f;
            heldRb = null;
        }
    }

    void FixedUpdate()
    {
        if (heldRb != null)
        {
            Vector3 targetPos = transform.position + transform.forward * holdDistance;
            Vector3 direction = targetPos - heldRb.position;

            heldRb.AddForce(direction * holdForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
