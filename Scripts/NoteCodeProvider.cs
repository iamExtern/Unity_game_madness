using System;
using UnityEngine;

public class NoteCodeProvider : MonoBehaviour
{
    public static Action<GameObject> onDoorInit;
    public static Action onNoteInit;

    public GameObject codeLockPrefab;

    private void Start()
    {
        onDoorInit.Invoke(codeLockPrefab);
        onNoteInit.Invoke();
    }
}
