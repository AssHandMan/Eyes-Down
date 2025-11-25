using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerTakeDropSystem : NetworkBehaviour
{
    [SerializeField] private KeyCode run;
    [SerializeField] private float speed;
    [SerializeField] private LayerMask pickupLayer;

    private InteractiveObject objLook, objIntercat;
    private HintsMsgController hintsMsgController;
    private void Start()
    {
        hintsMsgController = FindAnyObjectByType<HintsMsgController>();
    }
    void Update()
    {
        if(!isLocalPlayer) return;
        LookObject();
        InteractObject();
        Move();
    }

    private void LookObject()
    {
        if (objIntercat != null)
        {
            if (objLook != null)
            {
                objLook.DisableOutline();
                hintsMsgController.Hide();
                objLook = null;
            }
            return;
        }

        Ray ray = new Ray(transform.position, transform.forward);
        InteractiveObject newObj = null;

        if (Physics.Raycast(ray, out RaycastHit hit, 5, pickupLayer))
            newObj = hit.collider.GetComponentInParent<InteractiveObject>();

        if (newObj == objLook)
            return;

        if (objLook != null)
        {
            objLook.DisableOutline();
            hintsMsgController.Hide();
        }

        objLook = newObj;

        if (objLook != null)
        {
            objLook.EnableOutline();
            hintsMsgController.Print(objLook.PrintHelp());
        }
    }

    private void InteractObject()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (objIntercat == null)
            {
                if(objLook != null)
                {
                    objIntercat = objLook;
                    objIntercat.Interact(gameObject);
                }
            }
            else
            {
                objIntercat.Disconnect();
                objIntercat = null;
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse1) && objIntercat != null)
        {
            objIntercat.ExtraInteraction();
        }
    }

    private void Move()
    {
        if (Input.GetKey(run))
        {
            float moveX = Input.GetAxis("Horizontal");
            float moveY = Input.GetAxis("Vertical");
            Vector3 move = new Vector3(moveX, 0, moveY);
            transform.Translate(move * speed * Time.deltaTime);
        }
    }
    public void removeObject()
    {
        if (objIntercat != null) { objIntercat = null; }
    }
}
