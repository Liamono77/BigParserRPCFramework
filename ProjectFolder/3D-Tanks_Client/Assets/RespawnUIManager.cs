using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RespawnUIManager : MonoBehaviour
{
    public static RespawnUIManager instance;

    private void Awake()
    {
        instance = this;
    }

    public List<UIScreen> myScreens;
    // Start is called before the first frame update
    void Start()
    {
        foreach (UIScreen screen in myScreens)
        {
            screen.ManagedStart();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    
}
