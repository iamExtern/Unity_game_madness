using UnityEngine;

public class LerperEnd : Lerper
{
    public OnEnd onEnd;

    public LerperEnd(float speed, ActionTo actionTo, OnEnd onEnd) : base(speed, actionTo)
    {
        this.onEnd = onEnd;
    }

    public override void Lerping()
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

            if (!to)
            {
                onEnd();
            }
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

            if (!from)
            {
                onEnd();
            }
        }
    }

    public delegate void OnEnd();
}

