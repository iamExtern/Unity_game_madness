public class Battery : Key
{
    public override void Interactive()
    {
        taked = true;

        GuiManager.instance.SetAimType(GuiManager.AimType.Point);

        Player.instance.battery++;
        SoundManager.instance.PlaySound(SoundManager.Sound.EmpTake);
        Destroy(gameObject);
    }
}
