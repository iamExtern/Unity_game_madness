public class Key : InteractableObject
{
    public Outline outline;
    protected bool taked = false;

    protected virtual void Start()
    {
        outline.enabled = false;
    }

    public override void OnHoverEnter()
    {
        if (taked)
            return;

        if (!hover)
        {
            GuiManager.instance.SetAimType(GuiManager.AimType.Hand);
            if (Options.pc)
                GuiManager.instance.SetAimHand(MultiLanguage.GetText(2), false);
            else
                GuiManager.instance.SetAimHand(MultiLanguage.GetText(27), false);

            hover = true;
            outline.enabled = true;
        }
    }

    public override void OnHoverExit()
    {
        if (taked)
            return;

        if (hover)
        {
            GuiManager.instance.SetAimType(GuiManager.AimType.Point);

            hover = false;
            outline.enabled = false;
        }
    }

    public override void Interactive()
    {
        taked = true;

        GuiManager.instance.SetAimType(GuiManager.AimType.Point);

        Player.instance.key++;
        SoundManager.instance.PlaySound(SoundManager.Sound.KeyTake);
        Destroy(gameObject);
    }
}
