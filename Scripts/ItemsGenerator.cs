using UnityEngine;

public class ItemsGenerator : MonoBehaviour
{
    public int minKeyLockDoors, maxKeyLockDoors;
    public int minBattery, maxBattery;
    public int keyAdditional;
    public Transform keyPrefab;
    public Transform emp;
    public Transform[] batteryPrefab;
    public Transform[] empPoints;

    void Start()
    {
        ItemsSpawn();
    }

    private void ItemsSpawn()
    {
        int lockKeyDoorsCount = Random.Range(minKeyLockDoors, maxKeyLockDoors);
        for (int i = 0; i < lockKeyDoorsCount; i++)
        {
            int doorID = Random.Range(0, Door.doors.Count);
            Door door = Door.doors[doorID];
            door.lockMode = Door.LockMode.LockKey;
            Door.doors.RemoveAt(doorID);
        }

        int keyItemsCount = lockKeyDoorsCount + keyAdditional;
        for (int i = 0; i < keyItemsCount; i++)
        {
            int itemZoneID = Random.Range(0, ItemZone.itemZones.Count);
            ItemZone itemZone = ItemZone.itemZones[itemZoneID];

            if (itemZone.PutItem(Instantiate(keyPrefab), ItemZone.ItemType.Key))
            {
                ItemZone.itemZones.RemoveAt(itemZoneID);
            }
        }

        int batterysCount = Random.Range(minBattery, maxBattery);
        for (int i = 0; i < batterysCount; i++)
        {
            int itemZoneID = Random.Range(0, ItemZone.itemZones.Count);
            ItemZone itemZone = ItemZone.itemZones[itemZoneID];

            if (itemZone.PutItem(Instantiate(MathExtra.BoolRandom() ? batteryPrefab[0] : batteryPrefab[1]), ItemZone.ItemType.Battery))
            {
                ItemZone.itemZones.RemoveAt(itemZoneID);
            }
        }

        Transform empPoint = empPoints[Random.Range(0, empPoints.Length)];
        emp.position = empPoint.position + new Vector3(0f, 0.0073f, 0f);
        emp.eulerAngles = new Vector3(-90f, 0f, Random.Range(0f, 360f));
    }
}
