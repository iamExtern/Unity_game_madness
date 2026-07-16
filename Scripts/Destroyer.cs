using UnityEngine;

public class Destroyer : MonoBehaviour
{
    public float lifeTime = 1f;
    void Start()
    {
        Destroy(gameObject, lifeTime);
    }
}
