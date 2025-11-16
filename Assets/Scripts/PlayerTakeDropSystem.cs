using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerTakeDropSystem : NetworkBehaviour
{
    [SerializeField] private KeyCode run;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask pickupLayer;

    private InteractiveObject obj;

    void Update()
    {
        if(!isLocalPlayer) return;
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Ray ray = new Ray(transform.position, transform.forward);
            if (Physics.Raycast(ray, out RaycastHit hit, 5, pickupLayer))
            {
                if (hit.collider.gameObject.GetComponent<InteractiveObject>() != null)
                {
                    obj = hit.collider.gameObject.GetComponent<InteractiveObject>();
                    obj.Interact(gameObject);
                }
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && obj != null)
        {
            obj.ExtraInteraction();
        }

        if (Input.GetKeyUp(KeyCode.Mouse0) && obj != null)
        {
            obj.Disconnect();
            obj = null;
        }

        if(Input.GetKey(run))
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(moveX, 0, moveY);
            transform.Translate(move * speed * Time.deltaTime);
        }

    }

    public void removeObject()
    {
        if (obj != null) { obj = null; }
    }
}
