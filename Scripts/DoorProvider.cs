public class DoorProvider : InteractableObject
{
    public bool isFront;
    public Door door;

    public override void OnHoverEnter()
    {
        if (!hover)
        {
            hover = true;
            door.OnHoverEnter(isFront);
        }
    }

    public override void OnHoverExit()
    {
        if (hover)
        {
            hover = false;
            door.OnHoverExit(isFront);
        }
    }

    public override void Interactive()
    {
        door.Interactive();
    }
}
