using UnityEngine;

public class InteractiveObject : MonoBehaviour
{
    private string msg;

    public virtual void Interact(GameObject plr)
    {

    }

    public virtual void ExtraInteraction()
    {

    }

    public virtual void Disconnect()
    {

    }
    public virtual void EnableOutline()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            block.SetFloat("_OutlineWidth", 0.025f);
            r.SetPropertyBlock(block);
        }
    }

    public virtual void DisableOutline()
    {
        var renderers = GetComponentsInChildren<Renderer>();
        foreach (var r in renderers)
        {
            MaterialPropertyBlock block = new MaterialPropertyBlock();
            r.GetPropertyBlock(block);
            block.SetFloat("_OutlineWidth", 0f);
            r.SetPropertyBlock(block);
        }
    }

    public virtual string PrintHelp ()
    {
        return msg;
    }
}
