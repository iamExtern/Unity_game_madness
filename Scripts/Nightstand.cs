using UnityEngine;

public class Nightstand : InteractableObject
{
    protected bool open = false;
    protected Lerper lerper;

    protected virtual void Start()
    {
        lerper = new Lerper(1f / 0.2f, Openner);
    }

    private void Update()
    {
        lerper.Lerping();
    }

    public override void OnHoverEnter()
    {
        if (!hover)
        {
            hover = true;

            GuiManager.instance.SetAimType(GuiManager.AimType.Hand);

            if (!open)
            {
                if (Options.pc)
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(0), false);
                else
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(22), false);
            }
            else
            {
                if (Options.pc)
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(3), false);
                else
                    GuiManager.instance.SetAimHand(MultiLanguage.GetText(28), false);
            }
        }
    }

    public override void OnHoverExit()
    {
        if (hover)
        {
            hover = false;

            GuiManager.instance.SetAimType(GuiManager.AimType.Point);
        }
    }

    public override void Interactive()
    {
        open = !open;
        lerper.StartLerp(open);

        if (!open)
        {
            if (Options.pc)
                GuiManager.instance.SetAimHand(MultiLanguage.GetText(0), false);
            else
                GuiManager.instance.SetAimHand(MultiLanguage.GetText(22), false);
        }
        else
        {
            if (Options.pc)
                GuiManager.instance.SetAimHand(MultiLanguage.GetText(3), false);
            else
                GuiManager.instance.SetAimHand(MultiLanguage.GetText(28), false);
        }
    }

    protected virtual void Openner(float cof)
    {
        transform.localPosition = new Vector3(0f, Mathf.Lerp(0f, -0.002273f, cof), 0f);
    }
}
