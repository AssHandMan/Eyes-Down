using UnityEngine;

public class TakeableObject : InteractiveObject
{
    [SerializeField] [TextArea(3, 4)] private string hintText;
    [SerializeField] private float holdDistance, holdForce, throwForce;
    private Rigidbody rb;
    private bool isUsed;
    private GameObject plr;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    public override string PrintHelp()
    {
        return hintText;
    }
    public override void Interact(GameObject plr)
    {
        base.Interact(plr);
        this.plr = plr;
        rb.useGravity = false;
        rb.linearDamping = 10f;
        isUsed = true;
    }

    public override void ExtraInteraction()
    {
        base.ExtraInteraction();
        rb.useGravity = true;
        rb.linearDamping = 0f;
        isUsed = false;
        plr.GetComponent<PlayerTakeDropSystem>().removeObject();
        rb.AddForce(plr.transform.forward * throwForce, ForceMode.Impulse);
        plr = null;

    }

    public override void Disconnect()
    {
        base.Disconnect();
        rb.useGravity = true;
        rb.linearDamping = 0f;
        isUsed = false;
        plr = null;
    }

    private void FixedUpdate()
    {
        if(isUsed)
        {
            Vector3 targetPos = plr.transform.position + plr.transform.forward * holdDistance;
            Vector3 direction = targetPos - rb.position;

            rb.AddForce(direction * holdForce * Time.fixedDeltaTime, ForceMode.VelocityChange);
        }
    }
}
