public class ExitKey : Key
{
    public static ExitKey instance = null;

    private void Awake()
    {
        instance = this;    
    }

    public override void Interactive()
    {
        hover = false;
        outline.enabled = false;

        SoundManager.instance.PlaySound(SoundManager.Sound.KeyTake);

        GuiManager.instance.SetAimType(GuiManager.AimType.Point);
        transform.position = Player.instance.exitKeyPoint.position;
        transform.SetParent(Player.instance.controller.empContAnim.transform);
        transform.localEulerAngles = Player.instance.exitKeyPoint.localEulerAngles;
        transform.localScale = Player.instance.exitKeyPoint.localScale;
        Player.instance.exitKeyIsHave = true;
    }
}
