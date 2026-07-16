#if UNITY_EDITOR
using UnityEngine;

public class EditMethods : MonoBehaviour
{
    public GameObject door;
    public Transform doorParent;
    public Transform[] points;

    public Transform[] nightstands;

    public Transform[] closets;

    [ContextMenu("SpawnDoors")]
    public void SpawnDoors()
    {
        for (int i = 0; i < points.Length; i++)
        {
            GameObject clone = Instantiate(door);
            clone.transform.SetParent(doorParent);
            clone.transform.position = points[i].position;
        }
    }

    [ContextMenu("DeleteDoors")]
    public void DeleteDoors()
    {
        for (int i = doorParent.childCount - 1; i >= 0; i--)
        {
            DestroyImmediate(doorParent.GetChild(i).gameObject);
        }
    }

    [ContextMenu("CorrectNightstands")]
    public void CorrectNightstands()
    {
        for (int i = 0; i < nightstands.Length; i++)
        {
            for (int c = 0; c < 3; c++)
            {
                Transform shelfCont = nightstands[i].GetChild(c);

                if (c == 1)
                {
                    Transform shelf = shelfCont.GetChild(2);
                    Transform itemPoint1 = shelfCont.GetChild(0);
                    Transform itemPoint2 = shelfCont.GetChild(1);

                    itemPoint1.SetParent(shelf);
                    itemPoint2.SetParent(shelf);
                }
                else
                {
                    Transform shelf = shelfCont.GetChild(0);
                    Transform itemPoint1 = shelfCont.GetChild(1);
                    Transform itemPoint2 = shelfCont.GetChild(2);

                    itemPoint1.SetParent(shelf);
                    itemPoint2.SetParent(shelf);
                }
            }
        }
    }

    [ContextMenu("CorrectCloset")]
    public void CorrectCloset()
    {
        for (int i = 0; i < closets.Length; i++)
        {
            closets[i].GetComponent<Closet>().doorL = closets[i].GetChild(0);
            closets[i].GetComponent<Closet>().doorR = closets[i].GetChild(1);
        }
    }
}
#endif