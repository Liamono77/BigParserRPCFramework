using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetSyncTankTemp : NetSyncStandard
{
    public List<MeshRenderer> myRenderers = new List<MeshRenderer>();


    protected override void NetAwake(ref List<object> list)
    {
        base.NetAwake(ref list);
        Color theColor = new Color();
        Deque(ref theColor.r, list);
        Deque(ref theColor.g, list);
        Deque(ref theColor.b, list);

        SetRendererColors(theColor);
    }

    void SetRendererColors(Color theColor)
    {
        foreach (MeshRenderer mesh in myRenderers)
        {
            mesh.material.color = theColor;
        }
    }
}
