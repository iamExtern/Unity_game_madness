using UnityEngine;
using EventType = MonsterEventer.EventType;

public static class MonsterEventsConfig
{
    public static float targetMonsterSpawnCamAngle = 105f;
    public static float monsterDistanceToDetectPlayer = 4.5f;
    public static float spawnChanceAfterEvent = 0.21f;
    public static float maxAggressionAfterEventChance = 0.18f;
    public static float firstMonsterSpeedModerate = 1.27f;
    public static float monsterSpeedModerate = 1.35f;
    public static float firstMonsterSpeedAggressive = 1.44f;
    public static float monsterSpeedAggressive = 1.582f;

    public static (float min, float max) safeTime = new(18f, 23f);

    private static (float min, float max) maxAggressiveLifeTime = new(10f, 16f);
    private static (float min, float max) spawnDistanceAggressive = new(5.5f, 8f);
    private static (float min, float max) spawnDistanceModerate = new(5f, 7.5f);
    private static (float min, float max) minMonsterSpawnEventDelay = new(60f * 0.6f, 60f * 0.68f);
    private static (float min, float max) maxMonsterSpawnEventDelay = new(60f * 1.6f, 60f * 1.75f);
    private static (float min, float max) firstMonsterSpawnMaxDelay = new(60f * 1f, 60f * 1.12f);
    private static (float min, float max) eventDelay = new(20f, 26f);
    private static (float min, float max) firstEventDelay = new(13f, 17f);
    private static (float min, float max) fromDoorNewPointDistance = new(4f, 6.5f);
    private static (float min, float max) lightEventShortDuration = new(2.4f, 3.63f);
    private static (float min, float max) lightEventShortSpeed = new(0.43f, 0.58f);
    private static (float min, float max) lightEventLongDuration = new(7f, 9f);
    private static (float min, float max) lightEventLongSpeed = new(0.875f, 1f);
    private static (float min, float max) lightEventLongPow = new(1f, 1.33f);
    private static (float min, float max) redShoePrintDuration = new(7f, 10f);
    private static (float min, float max) redEyeDuration = new(12f, 16f);
    private static (float min, float max) spawnAggressiveMonsterDuration = new(10f, 11.5f);
    private static (float min, float max) spawnModerateMonsterDuration = new(13f, 15.3f);
    private static (float min, float max) spawnHarmlessMonsterDuration = new(12f, 16f);
    private static float spawnMonsterAfterLongLight = 0.4f;
    private static float spawnAggressiveMonsterAfterLight = 0.44f;
    private static float longLightEventChance = 0.28f;
    private static float monsterSpawnChoiceChance = 0.14f;

    private static float chanceSum;
    public static bool monsterHasAlreadySpawn = false;
    public static bool eventHasAlreadyInvoke = false;
    private static EventType? lastEvent = null;

    private static (EventType _event, float chance)[] eventsChance = new(EventType, float)[]
    {
        (EventType.SpawnAggressiveTarget, 0.1f),
        (EventType.SpawnModerateAggressive, 0.15f),
        (EventType.DoorHandleWiggling, 0.46f),
        (EventType.RedEyeDrawing, 0.47f),
        (EventType.OpenDoorHingeWiggling, 0.5f),
        (EventType.RedShoePrint, 0.51f),
        (EventType.TrashCanHand, 0.56f),
        (EventType.HarmlessSpawn, 0.61f),
        (EventType.SceneFlicker, 0.65f),
        (EventType.GlobalLightFlicker, 0.73f),
        (EventType.LampFlicker, 0.84f),
        (EventType.Sound, 0.9f)
    };

    public static void Init()
    {
        for (int i = 0; i < eventsChance.Length; i++)
        {
            chanceSum += eventsChance[i].chance;
        }
    }

    public static float GetFirstMonsterSpawnMaxDelay() => R2(firstMonsterSpawnMaxDelay);
    public static float GetMinMonsterSpawnEventDelay() => R2(minMonsterSpawnEventDelay);
    public static float GetMaxMonsterSpawnEventDelay() => R2(maxMonsterSpawnEventDelay);
    public static float GetAggressiveLifeTimeFromPeaceful() => R2(maxAggressiveLifeTime);
    public static float GetAggressiveDistanceSpawn() => R2(spawnDistanceAggressive);
    public static float GetModerateDistanceSpawn() => R2(spawnDistanceModerate);
    public static float GetSafeTime() => R2(safeTime);
    public static float GetFromDoorDistance() => R2(fromDoorNewPointDistance);
    public static float GetRedShoePrintDuration() => R2(redShoePrintDuration);
    public static float GetRedEyeDuration() => R2(redEyeDuration);
    public static float GetAggressiveMonsterDuration() => R2(spawnAggressiveMonsterDuration);
    public static float GetModerateMonsterDuration() => R2(spawnModerateMonsterDuration);
    public static float GetHarmlessMonsterDuration() => R2(spawnHarmlessMonsterDuration);
    public static bool SpawnMonsterIsMaxAggressive() => MathExtra.BoolRandom(spawnAggressiveMonsterAfterLight);

    public static bool SpawnMonsterAfterLightEvent(out bool maxAggressive, bool sceneLight)
    {
        float multiply = sceneLight ? 1.23f : 1f;
        bool spawn = MathExtra.BoolRandom(spawnMonsterAfterLongLight * multiply);
        if (spawn)
            maxAggressive = MathExtra.BoolRandom(spawnAggressiveMonsterAfterLight * multiply);
        else
            maxAggressive = false;

        return spawn;
    }

    public static float GetEventDelay(bool succesfullCall)
    {
        float delay = eventHasAlreadyInvoke ? R2(eventDelay) : R2(firstEventDelay);
        if (!succesfullCall)
            delay /= 2f;

        return delay;
    }

    public static void GetLightEventData(out bool isLong, out float speed, out float duration, out float intensity)
    {
        isLong = MathExtra.BoolRandom(longLightEventChance);
        if (isLong)
        {
            speed = R2(lightEventLongSpeed);
            duration = R2(lightEventLongDuration);
            intensity = R2(lightEventLongPow);
        }
        else
        {
            speed = R2(lightEventShortSpeed);
            duration = R2(lightEventShortDuration);
            intensity = 1f;
        }
    }

    public static EventType GetRadndomEvent()
    {
        EventType priorityEvent = EventType.Sound;
        int priorityEventID = 0;
        float randNum = Random.Range(0f, chanceSum);

        float priorityCount = 0f;
        for (int i = 0; i < eventsChance.Length; i++)
        {
            priorityCount += eventsChance[i].chance;

            if (randNum < priorityCount)
            {
                priorityEventID = i;
                priorityEvent = eventsChance[i]._event;

                break;
            }
        }

        if (lastEvent != null)
        {
            if (priorityEvent == lastEvent)
            {
                if (priorityEventID == eventsChance.Length - 1)
                {
                    priorityEvent = eventsChance[eventsChance.Length - 2]._event;
                }
                else
                {
                    priorityEvent = eventsChance[priorityEventID + 1]._event;
                }
            }
        }

        if (priorityEvent != EventType.SpawnAggressiveTarget && priorityEvent != EventType.SpawnModerateAggressive)
        {
            if (MonsterEventer.GetSpawnDelta() > R2(maxMonsterSpawnEventDelay))
            {
                float randNum2 = Random.Range(0f, 1f);

                if (randNum2 < monsterSpawnChoiceChance)
                    priorityEvent = EventType.SpawnAggressiveTarget;
                else
                    priorityEvent = EventType.SpawnModerateAggressive;
            }
            else if (!monsterHasAlreadySpawn)
            {
                if (MonsterEventer.gameTime > R2(firstMonsterSpawnMaxDelay))
                {
                    float randNum2 = Random.Range(0f, 1f);
                    priorityEvent = EventType.SpawnModerateAggressive;
                }
            }
        }
        else if (monsterHasAlreadySpawn)
        {
            if (MonsterEventer.GetSpawnDelta() < R2(minMonsterSpawnEventDelay))
            {
                int newID = Random.Range(2, eventsChance.Length);
                priorityEvent = eventsChance[newID]._event;
            }
        }

        eventHasAlreadyInvoke = true;

        lastEvent = priorityEvent;
        return priorityEvent;
    }

    private static float R2((float min, float max) num) => Random.Range(num.min, num.max);
}
