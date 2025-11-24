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
        Ray ray = new Ray(transform.position, transform.forward);

        if (Physics.Raycast(ray, out RaycastHit hit, 5, pickupLayer))
        {
            InteractiveObject io = hit.collider.GetComponent<InteractiveObject>();

            if (io != null && io != objLook) 
            { 
                objLook = io;
                if (objIntercat == null) 
                { 
                    hintsMsgController.Print(objLook.PrintHelp());
                    objLook.EnableOutline();
                }
            }
        }
        else
        {
            if(objLook != null)
            {
                objLook.DisableOutline();
                objLook = null;
                hintsMsgController.Hide();
            }
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
