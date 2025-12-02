using UnityEngine;

public class RevolverController : InteractiveObject
{

    [SerializeField] [TextArea(3, 4)] private string hintText;
    [SerializeField] private Transform Cylindr;
    [SerializeField] private BulletSocketSetter[] sockets;
    [SerializeField] private CameraController cam;
    [SerializeField] private LaptopController laptop;
    private bool inProcess;
    private Vector3 startPos, startRot;
    private GameObject plrHead;
    public override void Interact(GameObject plr)
    {
        base.Interact(plr);
        plrHead = plr;
        if (!inProcess)
        {
            transform.parent = plr.transform;
            transform.localEulerAngles = new Vector3(0,270,90);
            Vector3 pos = transform.localPosition;
            pos.z = 1;
            transform.localPosition = pos;
            Cylindr.localEulerAngles = new Vector3 (0,90,0);
            cam.LockCameraController();
            inProcess = true;
        }
    }

    private void Start()
    {
        startPos = transform.position;
        startRot = transform.rotation.eulerAngles;
    }
    public override string PrintHelp()
    {
        return hintText;
    }

    public void Shoot()
    {
        int count = 0;
        for (int i = 0; i < sockets.Length; i++)
        {
            if (sockets[i].GetActive()) { count++; }
        }
        if(count > 0)
        {
            if (sockets[Random.Range(0, sockets.Length)].GetActive())
            {
                Debug.Log("Умер");
                plrHead.AddComponent<Rigidbody>();
            }
            else
            {
                Debug.Log("Пронесло!");
                laptop.AddMoney(1000 * count);
            }
            for (int i = 0; i < sockets.Length; i++)
            {
                sockets[i].Shoot();
            }
        }
    }
    private void Return()
    {
        transform.parent = null;
        transform.position = startPos;
        transform.eulerAngles = startRot;
        Cylindr.localEulerAngles = new Vector3(0, 0, 0);
        cam.LockCameraController();
        inProcess = false;
    }

    public void Update()
    {
        if(inProcess & Input.GetKeyDown(KeyCode.Escape))
        {
            Return();
        }
        if(Input.GetKeyDown(KeyCode.E) & inProcess)
        {
            Shoot();
            Return();
        }
    }

}
