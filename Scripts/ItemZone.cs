using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ItemZone : MonoBehaviour
{
    public static List<ItemZone> itemZones = new List<ItemZone>();
    public ZoneType zoneType;
    public Transform[] points;

    private int itemsCount;

    private void Awake()
    {
        if (MathExtra.BoolRandom(itemsSpawnChance[zoneType]))
            itemZones.Add(this);
    }

    public bool PutItem(Transform item, ItemType itemType)
    {
        itemsCount++;
        int pointID = Random.Range(0, points.Length);
        Transform point = points[pointID];
        item.position = point.position;
        List<Transform> pointList = points.ToList();
        pointList.RemoveAt(pointID);
        points = pointList.ToArray();

        if (itemType == ItemType.Key)
        {
            item.position += new Vector3(0f, 0.00037f, 0f);
            item.transform.eulerAngles = new Vector3(-90f, 0f, Random.Range(0f, 360f));
        }
        else
        {
            item.position += new Vector3(0f, 0.00164f, 0f);
            item.transform.eulerAngles = new Vector3(0f, Random.Range(0f, 360f), 0f);
        }

        if (zoneType == ZoneType.Nightstand)
            item.SetParent(point.parent);

        return itemsCount == maxItemsPerZone[zoneType];
    }

    public enum ZoneType
    {
        Sofa,
        Bed,
        Tub,
        Boards,
        Bench,
        Table,
        Stool,
        Chair,
        Box,
        Barrel,
        Nightstand,
        ShelvesCloset,
        Closet
    }

    private Dictionary<ZoneType, int> maxItemsPerZone = new Dictionary<ZoneType, int>()
    {
        [ZoneType.Sofa] = 1,
        [ZoneType.Bed] = 2,
        [ZoneType.Tub] = 1,
        [ZoneType.Boards] = 2,
        [ZoneType.Bench] = 2,
        [ZoneType.Table] = 1,
        [ZoneType.Stool] = 1,
        [ZoneType.Chair] = 1,
        [ZoneType.Box] = 1,
        [ZoneType.Barrel] = 1,
        [ZoneType.Nightstand] = 1,
        [ZoneType.ShelvesCloset] = 2,
        [ZoneType.Closet] = 3
    };

    private Dictionary<ZoneType, float> itemsSpawnChance = new Dictionary<ZoneType, float>()
    {
        [ZoneType.Sofa] = 0.36f,
        [ZoneType.Bed] = 0.4f,
        [ZoneType.Tub] = 0.5f,
        [ZoneType.Boards] = 0.5f,
        [ZoneType.Bench] = 0.36f,
        [ZoneType.Table] = 1f,
        [ZoneType.Stool] = 0.5f,
        [ZoneType.Chair] = 0.48f,
        [ZoneType.Box] = 0.67f,
        [ZoneType.Barrel] = 0.8f,
        [ZoneType.Nightstand] = 1f,
        [ZoneType.ShelvesCloset] = 0.43f,
        [ZoneType.Closet] = 1f
    };

    public enum ItemType
    {
        Key,
        Battery
    }
}
