using UnityEngine;

public class Lerper
{
    public float speed;
    public bool to = false;
    public bool from = false;
    public float cof = 0f;

    public ActionTo actionTo;

    public Lerper(float speed, ActionTo actionTo)
    {
        this.speed = speed;
        this.actionTo = actionTo;
    }

    public virtual void Lerping()
    {
        if (to)
        {
            if (cof < 1f)
                cof += Time.deltaTime * speed;
            if (cof >= 1f)
            {
                cof = 1f;
                to = false;
            }

            actionTo(cof);
        }
        else if (from)
        {
            if (cof >= 0f)
                cof -= Time.deltaTime * speed;
            if (cof <= 0f)
            {
                cof = 0f;
                from = false;
            }

            actionTo(cof);
        }
    }

    public void StartLerp(bool _to = true)
    {
        to = _to;
        from = !_to;
    }

    public void StopLerp()
    {
        to = false;
        from = false;
    }

    public delegate void ActionTo(float cof);
}
