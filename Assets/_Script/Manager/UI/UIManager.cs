using System.Collections;
using System.Collections.Generic;
using _Script.DesignPattern.Singleton;
using UnityEngine;

public class UIManager : Singleton<UIManager>
{
    protected override void Awake()
    {
        base.Awake();
        KeepAlive(true);
    }
    
    
}
