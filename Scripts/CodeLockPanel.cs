using System;
using UnityEngine;

public class CodeLockPanel : MonoBehaviour
{
    public static Action onExit;
    public static Action onOpen;

    public void Exit()
    {
        onExit?.Invoke();
    }

    public void Open()
    {
        onOpen?.Invoke();
    }
}
