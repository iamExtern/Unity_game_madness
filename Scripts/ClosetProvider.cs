public class ClosetProvider : InteractableObject
{
    public Closet closet;

    public override void OnHoverEnter()
    {
        if (!hover)
        {
            hover = true;
            closet.OnHoverEnter();
        }
    }

    public override void OnHoverExit()
    {
        if (hover)
        {
            hover = false;
            closet.OnHoverExit();
        }
    }

    public override void Interactive()
    {
        closet.Interactive();
    }
}
