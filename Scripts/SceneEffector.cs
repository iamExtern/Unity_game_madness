using UnityEngine;

public abstract class SceneEffector : MonoBehaviour
{
    public bool changing = false;

    protected virtual void Start()
    {
        ResetValues();
    }

    private void Update()
    {
        if (!changing)
            return;

        SetValues();
    }

    protected virtual void SetValues()
    {

    }

    protected virtual void ResetValues()
    {

    }
}