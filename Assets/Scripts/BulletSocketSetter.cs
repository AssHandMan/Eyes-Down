using UnityEngine;

public class BulletSocketSetter : MonoBehaviour
{
    [SerializeField] private GameObject bullet;
    private bool isActive;
    public void OnMouseDown()
    {
        isActive = !isActive;
        bullet.SetActive(isActive);
    }

    public bool GetActive() { return isActive; }
    public void Shoot() { isActive = false; bullet.SetActive(isActive); }
}
