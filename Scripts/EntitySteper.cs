using UnityEngine;

public class EntitySteper : MonoBehaviour
{
    public EntityType entityType;

    public void Step()
    {
        if (entityType == EntityType.Monster)
            SoundManager.instance.RandomMonsterStep(transform.position);
        else
            SoundManager.instance.RandomPlayerStep();
    }

    public enum EntityType
    {
        Monster,
        Player
    }
}
