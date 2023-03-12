using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//public class UINode 


public class UIScreen : UIBase
{
    //public virtual void ManagedStart()
    //{

    //}
    //put this on UI buttons tabs
    public void SwitchToMe()
    {
        Debug.Log("SWITCHSCREEN");
        gameObject.SetActive(true);
        foreach (UIScreen screen in RespawnUIManager.instance.myScreens)
        {
            if (screen != this)
            {
                screen.gameObject.SetActive(false);
            }
        }
    }
}
