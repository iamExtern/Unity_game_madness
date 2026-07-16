using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MonsterEventer : MonoBehaviour
{
    public static MonsterEventer instance = null;

    public Monster monster;
    public Animator roomWallAnim;
    public Transform[] mapCornerPoints;
    public Transform[] reserveMonsterPoints;
    public GameObject redShoePrefab;
    public RedShoeSteper redShoeSteperPrefab;
    public LayerMask trashCunLayerMask;
    public LayerMask doorLayerMask;

    private SoundManager.Sound[] Ambients = new SoundManager.Sound[]
    {
        SoundManager.Sound.Ambient1,
        SoundManager.Sound.Ambient3
    };

    private int[][] patterns = new int[][]
    {
        new int[] { 1, 2 },
        new int[] { 2, 3 },
        new int[] { 2, 3, 4 },
        new int[] { 2, 1 },
        new int[] { 1, 2, 3 },
        new int[] { 1, 3 },
        new int[] { 2, 4 },
    };

    private EmpClip currentEmpClip = null;
    private Coroutine empClipStep = null;
    private RedShoeSteper currentRedShoeSteper = null;

    public static Action doorHandlesWigglingStop;
    public static Action doorHingeWigglingIteration;
    public static Action doorHingeWigglingStop;
    public static Action timerIteration;

    private bool doorHingeWiggling = false;
    private bool currentLightEventIsLong = false;

    private bool[] stateStack = new bool[12];

    private Timer[] timer;
    private bool isSafeTime = true;
    public static float lastMonsterSpawnTime;
    public static float gameTime;

    private Coroutine eventCycle = null;
    private Coroutine empDisable = null;
    private bool currentEmpTranslate = false;
    private float maxDistance;
    private float maxLevel;
    private float minDistance;
    private Transform empTarget = null;
    private Vector3 lastPlayerPosForMonster;

    private void Awake()
    {
        MonsterEventsConfig.Init();
        Timer lightTimer = new Timer(DisableLightEvent);

        timer = new Timer[]
        {
            null,
            lightTimer,
            lightTimer,
            lightTimer,
            new Timer(DoorHingeWigglingStop),
            new Timer(DoorHandlesWigglingStop),
            new Timer(RedShoePrintStop),
            new Timer(RedEyeDrawingStop),
            null,
            new Timer(AggressiveTargetStop),
            new Timer(ModerateAggressiveStop),
            new Timer(HarmlessSpawnStop),
        };

        instance = this;
    }

    private void Start()
    {
        monster.mouthRope.Init();
        Invoke(nameof(OnSafeTimeWasted), MonsterEventsConfig.GetSafeTime());
        DisableEvent();
        StartCoroutine(TimeLogger());
    }

    private void Update()
    {
        DoorHingeLerping();
        TimerManager();
        GameTimeCounter();
        EmpTranslate();
    }

    private void EmpActive(float duration, int level)
    {
        if (currentEmpTranslate)
            return;

        ResetEmpDisable(duration);
        EMP.instance.SetLevel(level);
    }

    private void EmpActive(float duration, EmpClip empClip)
    {
        if (currentEmpTranslate)
            return;

        ResetEmpDisable(duration);
        PlayEmpClip(empClip);
    }

    private void EmpActiveTranslate(Transform target, float maxDistance, float minDistance, float maxLevel) //maxLevel (min) = 2
    {
        if (empDisable != null)
        {
            StopEmpClip();
            StopCoroutine(empDisable);
        }

        empTarget = target;
        this.maxDistance = maxDistance;
        this.maxLevel = maxLevel;
        this.minDistance = minDistance;
        currentEmpTranslate = true;
    }

    private void EmpTranslate()
    {
        if (!currentEmpTranslate)
            return;

        if (empTarget != null && empTarget.gameObject.activeInHierarchy)
        {
            float distance = Vector3.Distance(Player.instance.transform.position, empTarget.position);
            float t = MathExtra.ClampFromLerp(maxDistance, minDistance, distance);
            int level = (int)((maxLevel - 1) * t) + 1;

            if (level != EMP.instance.currentLevel)
            {
                EMP.instance.SetLevel(level);
            }
        }
        else
        {
            currentEmpTranslate = false;
            EMP.instance.SetLevel(1);
        }
    }


    private void ResetEmpDisable(float duration)
    {
        if (empDisable != null)
        {
            StopEmpClip();
            StopCoroutine(empDisable);
        }
        empDisable = StartCoroutine(EmpDisable(duration));
    }

    private IEnumerator EmpDisable(float duration)
    {
        yield return new WaitForSeconds(duration);

        if (currentEmpClip == null)
            EMP.instance.SetLevel(1);
        else
            StopEmpClip();
    }

    private void OnSafeTimeWasted()
    {
        isSafeTime = false;
        eventCycle = StartCoroutine(EventInvoker());
    }

    private IEnumerator TimeLogger()
    {
        yield return new WaitForSeconds(1f);
        StartCoroutine(TimeLogger());
    }

    private IEnumerator EventInvoker(bool succesfullCall = true)
    {
        float delay = MonsterEventsConfig.GetEventDelay(succesfullCall);
        yield return new WaitForSeconds(delay);

        EventType _event = MonsterEventsConfig.GetRadndomEvent();

        if (!GetEventState(_event))
        {
            if (EventTypeIsLight(_event) && LightEventIsActive())
                Failed();
            else if (EventTypeIsDoor(_event) && DoorEventIsActive())
                Failed();
            else if (EventTypeIsMonster(_event) && MonsterEventIsActive())
                Failed();
            else if (_event == EventType.RedShoePrint && MonsterEventIsActive())
                Failed();
            else if (EventTypeIsMonster(_event) && GetEventState(EventType.RedShoePrint))
                Failed();
            else if ((monster.mode == Monster.Mode.AggressiveTarget || monster.mode == Monster.Mode.MaximumAggressive) && EventTypeIsLight(_event))
                Failed();
            else
            {
                MonsterEventsConfig.eventHasAlreadyInvoke = true;
                //Debug.Log("Event Start: " + _event);
                StartEvent(_event);
                eventCycle = StartCoroutine(EventInvoker());
            }
        }
        else
        {
            Failed();
        }

        void Failed()
        {
            StartCoroutine(EventInvoker(false));
            eventCycle = StartCoroutine(EventInvoker());
        }
    }

    private void StartEvent(EventType _event)
    {
        switch (_event)
        {
            case EventType.Sound:
                {
                    bool playClip = MathExtra.BoolRandom(0.34f);
                    float duration = MathExtra.R2(2.7f, 5.3f);

                    if (playClip)
                        EmpActive(duration, new EmpClip(MathExtra.BoolRandom() ? patterns[0] : patterns[3], MathExtra.R2(0.3f, 0.4f), 0.18f));
                    else
                        EmpActive(duration, 2);

                    SoundEvent();
                    break;
                }
            case EventType.LampFlicker:
                {
                    MonsterEventsConfig.GetLightEventData(out bool islong, out float speed, out float duration, out float intensity);
                    currentLightEventIsLong = islong;
                    LightEmpPlayer(islong, false, duration);
                    LightFlickingEvent(EventType.LampFlicker, duration, speed, intensity);

                    lastPlayerPosForMonster = Player.instance.transform.position;

                    break;
                }
            case EventType.GlobalLightFlicker:
                {
                    MonsterEventsConfig.GetLightEventData(out bool islong, out float speed, out float duration, out float intensity);
                    currentLightEventIsLong = islong;
                    LightEmpPlayer(islong, true, duration);
                    LightFlickingEvent(EventType.GlobalLightFlicker, duration, speed, intensity);

                    lastPlayerPosForMonster = Player.instance.transform.position;

                    break;
                }
            case EventType.SceneFlicker:
                {
                    MonsterEventsConfig.GetLightEventData(out bool islong, out float speed, out float duration, out float intensity);
                    currentLightEventIsLong = islong;
                    LightEmpPlayer(islong, false, duration);
                    LightFlickingEvent(EventType.SceneFlicker, duration, speed, intensity);

                    lastPlayerPosForMonster = Player.instance.transform.position;

                    break;
                }
            case EventType.OpenDoorHingeWiggling:
                {
                    DoorHingeWigglignEvent();

                    lastPlayerPosForMonster = Player.instance.transform.position;

                    EventEmpPlayer(0.35f, 7.5f + MathExtra.R2(0.5f, 1.2f), 1, 2, MathExtra.R2(0.25f, 0.4f), MathExtra.R2(0.5f, 0.73f), MathExtra.BoolRandom() ? patterns[1] : patterns[4], 2);
					
                    break;
                }
            case EventType.DoorHandleWiggling:
                {
                    DoorHandlesWigglingEvent(7f);

                    lastPlayerPosForMonster = Player.instance.transform.position;

                    EventEmpPlayer(0.35f, 7f + MathExtra.R2(1.3f, 2f), 1, 3, MathExtra.R2(0.25f, 0.43f), MathExtra.R2(0.15f, 0.3f), MathExtra.BoolRandom() ? patterns[1] : patterns[5], 3);
					
                    break;
                }
            case EventType.RedShoePrint:
                {
                    RedShoePrintEvent(MonsterEventsConfig.GetRedShoePrintDuration());

                    lastPlayerPosForMonster = Player.instance.transform.position;

                    EmpActiveTranslate(currentRedShoeSteper.transform, MathExtra.R2(10f, 11.5f), 2f, MathExtra.IR2(3, 5));

                    break;
                }
            case EventType.RedEyeDrawing:
                {
                    float duration = MonsterEventsConfig.GetRedEyeDuration();
                    RedEyeDrawingEvent(duration);
                    EventEmpPlayer(0.15f, duration + MathExtra.R2(-1.3f, -2f), 2, 4, MathExtra.R2(0.3f, 0.4f), MathExtra.R2(0.5f, 0.7f), MathExtra.BoolRandom() ? patterns[2] : patterns[6], MathExtra.IR2(3, 5));

                    break;
                }
            case EventType.TrashCanHand:
                {
                    lastPlayerPosForMonster = Player.instance.transform.position;

                    if (!TryTrashCunHandEvent())
                    {
                        StopCoroutine(eventCycle);
                        EmpActiveTranslate(monster.transform, MathExtra.R2(9.5f, 11.5f), 1.4f, MathExtra.IR2(2, 4));
                        eventCycle = StartCoroutine(EventInvoker(false));
                    }
                    break;
                }
            case EventType.SpawnAggressiveTarget:
                {
                    lastPlayerPosForMonster = Player.instance.transform.position;

                    SpawnAggressiveTargetEvent(MonsterEventsConfig.GetAggressiveMonsterDuration());

                    break;
                }
            case EventType.SpawnModerateAggressive:
                {
                    lastPlayerPosForMonster = Player.instance.transform.position;

                    SpawnModerateAggressiveEvent(MonsterEventsConfig.GetModerateMonsterDuration());

                    break;
                }
            case EventType.HarmlessSpawn:
                {
                    lastPlayerPosForMonster = Player.instance.transform.position;

                    HarmlessSpawnEvent(MonsterEventsConfig.GetHarmlessMonsterDuration());

                    break;
                }
            default:
                break;
        }

        void LightEmpPlayer(bool _isLong, bool _globalLight, float _duration)
        {
            float duration2 = _duration + MathExtra.R2(0f, 1f);

            bool randLevel = MathExtra.BoolRandom(0.6f);
            EmpClip empClip;
			
            if (randLevel)
            {
                empClip = new EmpClip(2, 3, MathExtra.R2(0.2f, 0.3f) * (_globalLight ? 0.8f : 1f), 0.32f);
            }
            else
            {
                empClip = new EmpClip(patterns[1], MathExtra.R2(0.2f, 0.3f) * (_isLong ? 1.2f : 1f), 0.23f);
            }

            EmpActive(duration2, empClip);
        }

        void EventEmpPlayer(float clipChance, float _duration, int minLevel, int maxLevel, float stepDelay, float random, int[] pattern, int level)
        {
            bool playClip = MathExtra.BoolRandom(clipChance);

            if (playClip)
            {
                bool randLevel = MathExtra.BoolRandom(0.6f);
                EmpClip empClip;
                if (randLevel)
                {
                    empClip = new EmpClip(minLevel, maxLevel, stepDelay, random);
                }
                else
                {
                    empClip = new EmpClip(pattern, stepDelay, random);
                }

                EmpActive(_duration, empClip);
            }
            else
            {
                EmpActive(_duration, level);
            }
        }
    }

    public void MonsterModeEMP()
    {
        if (monster.mode == Monster.Mode.MaximumAggressive)
            EmpActiveTranslate(monster.transform, 10f, 2, 6);
        else if (monster.mode == Monster.Mode.AggressiveTarget)
            EmpActiveTranslate(monster.transform, 10f, 2, 5);
        else if (monster.mode == Monster.Mode.Peaceful)
            EmpActiveTranslate(monster.transform, 13f, 1.85f, 5);
    }

    public static float GetSpawnDelta() => gameTime - lastMonsterSpawnTime;

    private void DisableEvent()
    {
        monster.SetMode(Monster.Mode.None, Vector3.zero);
        monster.DeactivateMonster();
        monster.gameObject.SetActive(false);
    }

    private void GameTimeCounter()
    {
        gameTime += Time.deltaTime;
    }

    private void TimerManager()
    {
        timerIteration?.Invoke();
    }

    private void DoorHingeLerping()
    {
        if (!doorHingeWiggling)
            return;

        doorHingeWigglingIteration?.Invoke();
    }

    private void SoundEvent()
    {
        SoundManager.instance.PlaySound(Ambients[UnityEngine.Random.Range(0, Ambients.Length)]);
    }

    private void SpawnAfterEvent()
    {
        if (MonsterEventsConfig.monsterHasAlreadySpawn)
        {
            if (MathExtra.BoolRandom(MonsterEventsConfig.spawnChanceAfterEvent))
            {
                if (GetSpawnDelta() > MonsterEventsConfig.GetMinMonsterSpawnEventDelay())
                {
                    if (MathExtra.BoolRandom(MonsterEventsConfig.maxAggressionAfterEventChance))
                        SpawnAggressiveTargetEvent(MonsterEventsConfig.GetAggressiveMonsterDuration());
                    else
                        SpawnModerateAggressiveEvent(MonsterEventsConfig.GetModerateMonsterDuration());
                }
            }
            else if (GetSpawnDelta() > MonsterEventsConfig.GetMaxMonsterSpawnEventDelay())
            {
                if (MathExtra.BoolRandom(MonsterEventsConfig.maxAggressionAfterEventChance))
                    SpawnAggressiveTargetEvent(MonsterEventsConfig.GetAggressiveMonsterDuration());
                else
                    SpawnModerateAggressiveEvent(MonsterEventsConfig.GetModerateMonsterDuration());
            }
        }
        else if (gameTime > MonsterEventsConfig.GetFirstMonsterSpawnMaxDelay())
        {
            if (MathExtra.BoolRandom(MonsterEventsConfig.maxAggressionAfterEventChance))
                SpawnAggressiveTargetEvent(MonsterEventsConfig.GetAggressiveMonsterDuration());
            else
                SpawnModerateAggressiveEvent(MonsterEventsConfig.GetModerateMonsterDuration());
        }
    }

    private bool TryTrashCunHandEvent()
    {
        Collider[] coll = Physics.OverlapSphere(Player.instance.transform.position, 3.2f, trashCunLayerMask);

        Transform nearestTrashCun = null;
        float distance = 0f;
        for (int i = 0; i < coll.Length; i++)
        {
            float newDistance = Vector3.Distance(Player.instance.transform.position, coll[i].transform.position);

            if (nearestTrashCun == null)
            {
                nearestTrashCun = coll[i].transform;
                distance = newDistance;
            }
            else if (newDistance < distance)
            {
                nearestTrashCun = coll[i].transform;
                distance = newDistance;
            }
        }

        if (nearestTrashCun != null)
        {
            monster.gameObject.SetActive(true);
            monster.SetMode(Monster.Mode.None, Vector3.zero);
            monster.transform.position = new Vector3(nearestTrashCun.position.x, -1.124f, nearestTrashCun.position.z);
            monster.transform.eulerAngles = new Vector3(0f, 180f, 0f);

            SoundManager.instance.PlaySound3D(SoundManager.Sound.Ambient2, nearestTrashCun.position, nearestTrashCun);
            monster.anim.SetTrigger("TrashCunHand");
            EnableEventState(EventType.TrashCanHand);

            return true;
        }
        else
            return false;
    }

    public void TrashCunHandEnd()
    {
        monster.DeactivateMonster();
        monster.gameObject.SetActive(false);
        DisableEventState(EventType.TrashCanHand);
    }

    private void LightFlickingEvent(EventType _evenType, float time, float speed, float sceneLightPow)
    {
        monster.lightAnim.speed = speed;

        if (_evenType == EventType.LampFlicker)
        {
            monster.lightAnim.SetBool("GlobalLightFlicker", false);
            monster.lightAnim.SetBool("SceneFlicker", false);
            monster.lightAnim.SetBool("LampFlicker", true);
        }
        else if (_evenType == EventType.GlobalLightFlicker)
        {
            SceneLightProvider.instance.directionalLightPow = sceneLightPow;

            monster.lightAnim.SetBool("LampFlicker", false);
            monster.lightAnim.SetBool("SceneFlicker", false);
            monster.lightAnim.SetBool("GlobalLightFlicker", true);
        }
        else if (_evenType == EventType.SceneFlicker)
        {
            SceneLightProvider.instance.directionalLightPow = sceneLightPow;

            monster.lightAnim.SetBool("LampFlicker", false);
            monster.lightAnim.SetBool("GlobalLightFlicker", false);
            monster.lightAnim.SetBool("SceneFlicker", true);
        }

        StartTimer(_evenType, time);
    }

    private void DisableLightEvent()
    {
        SceneLightProvider.instance.directionalLightPow = 1f;

        monster.lightAnim.SetBool("LampFlicker", false);
        monster.lightAnim.SetBool("GlobalLightFlicker", false);
        monster.lightAnim.SetBool("SceneFlicker", false);

        monster.lightAnim.speed = 1f;

        if (currentLightEventIsLong)
        {
            bool sceneLight = GetEventState(EventType.GlobalLightFlicker);
            if (!MonsterEventIsActive())
            {
                if (MonsterEventsConfig.SpawnMonsterAfterLightEvent(out bool maxAggressive, sceneLight))
                {
                    if (maxAggressive)
                        SpawnAggressiveTargetEvent(MonsterEventsConfig.GetAggressiveMonsterDuration());
                    else
                        SpawnAggressiveTargetEvent(MonsterEventsConfig.GetModerateMonsterDuration());
                }
            }
        }

        DisableEventState(EventType.LampFlicker);
        DisableEventState(EventType.GlobalLightFlicker);
        DisableEventState(EventType.SceneFlicker);
    }

    private void PlayEmpClip(EmpClip clip)
    {
        if (empClipStep != null)
            StopCoroutine(empClipStep);

        currentEmpClip = clip;
        currentEmpClip.ResetStep();

        empClipStep = StartCoroutine(EmpClipStep(0.15f));
    }

    private void StopEmpClip()
    {
        if (empClipStep != null)
        {
            EMP.instance.SetLevel(1);
            StopCoroutine(empClipStep);
        }
    }

    private IEnumerator EmpClipStep(float delay)
    {
        yield return new WaitForSeconds(delay);
		
        EMP.instance.SetLevel(currentEmpClip.GetStep());
        empClipStep = StartCoroutine(EmpClipStep(currentEmpClip.GetDelay()));
    }

    private void DoorHandlesWigglingEvent(float duration)
    {
        Collider[] doorColliders = Physics.OverlapSphere(Player.instance.transform.position, 7f, doorLayerMask);

        if (doorColliders.Length == 0)
            return;

        for (int i = 0; i < doorColliders.Length; i++)
        {
            doorColliders[i].transform.parent.GetComponent<Door>().StartHandleWiggling();
        }

        StartTimer(EventType.DoorHandleWiggling, duration);
    }

    private void DoorHandlesWigglingStop()
    {
        doorHandlesWigglingStop?.Invoke();
        DisableEventState(EventType.DoorHandleWiggling);

        SpawnAfterEvent();
    }

    private void DoorHingeWigglignEvent()
    {
        Collider[] doorColliders = Physics.OverlapSphere(Player.instance.transform.position, 4f, doorLayerMask);

        if (doorColliders.Length == 0)
            return;

        doorHingeWiggling = true;

        int totalDoors = UnityEngine.Random.Range(1, 4);
        for (int i = 0; i < doorColliders.Length; i++)
        {
            if (i < totalDoors)
                doorColliders[i].transform.parent.GetComponent<Door>().StartHingeWiggling();
            else
                break;
        }

        StartTimer(EventType.OpenDoorHingeWiggling, 7.5f);
    }

    private void DoorHingeWigglingStop()
    {
        doorHingeWiggling = false;
        doorHingeWigglingStop?.Invoke();
        DisableEventState(EventType.OpenDoorHingeWiggling);

        SpawnAfterEvent();
    }

    private void RedEyeDrawingEvent(float duration) //min duration - 5f;
    {
        SoundManager.instance.PlaySound(SoundManager.Sound.EyeDrawingManifest);
        roomWallAnim.SetTrigger("ShowEye");

        StartTimer(EventType.RedEyeDrawing, duration);
    }

    private void RedEyeDrawingStop()
    {
        roomWallAnim.SetTrigger("HideEye");
        DisableEventState(EventType.RedEyeDrawing);
    }

    private void RedShoePrintEvent(float duration)
    {
        currentRedShoeSteper = Instantiate(redShoeSteperPrefab);
        currentRedShoeSteper.Init(mapCornerPoints, redShoePrefab);

        StartTimer(EventType.RedShoePrint, duration);
    }

    private void RedShoePrintStop()
    {
        Destroy(currentRedShoeSteper.gameObject);
        DisableEventState(EventType.RedShoePrint);

        SpawnAfterEvent();
    }

    private void SpawnAggressiveTargetEvent(float duration)
    {
        monster.gameObject.SetActive(true);
        monster.SetMode(Monster.Mode.MaximumAggressive, lastPlayerPosForMonster);

        monster.SetPosition(SpawnMonsterAfterEvent());

        StartTimer(EventType.SpawnAggressiveTarget, duration);
    }

    private void AggressiveTargetStop()
    {
        if (!monster.playerKilling)
        {
            monster.SetMode(Monster.Mode.None, Vector3.zero);
            monster.DeactivateMonster();
            monster.gameObject.SetActive(false);

            DisableEventState(EventType.SpawnAggressiveTarget);
        }
    }

    private void SpawnModerateAggressiveEvent(float duration)
    {
        monster.gameObject.SetActive(true);
        monster.SetMode(Monster.Mode.AggressiveTarget, lastPlayerPosForMonster);

        monster.SetPosition(SpawnMonsterAfterEvent());

        StartTimer(EventType.SpawnModerateAggressive, duration);
    }

    private void ModerateAggressiveStop()
    {
        if (!monster.playerKilling)
        {
            monster.SetMode(Monster.Mode.None, Vector3.zero);
            monster.DeactivateMonster();
            monster.gameObject.SetActive(false);

            DisableEventState(EventType.SpawnModerateAggressive);
        }
    }

    private void HarmlessSpawnEvent(float duration)
    {
        monster.gameObject.SetActive(true);
        monster.SetMode(Monster.Mode.Peaceful, Vector3.zero);
        Vector3 targetPos = NavigationExtra.GetNearPlayerPostion(Player.instance.controller.playerMain.position, UnityEngine.Random.Range(4.5f, 7f));
        monster.SetPosition(targetPos);

        StartTimer(EventType.HarmlessSpawn, duration);
    }

    private void HarmlessSpawnStop()
    {
        monster.SetMode(Monster.Mode.None, Vector3.zero);
        monster.DeactivateMonster();
        monster.gameObject.SetActive(false);
        DisableEventState(EventType.HarmlessSpawn);
    }

    private Vector3 SpawnMonsterAfterEvent()
    {
        Vector3 dir = (lastPlayerPosForMonster - Player.instance.transform.position).normalized;
        Vector2 dir2 = MathExtra.ToPlaneVector2(dir);
        Vector3 result = NavigationExtra.GetSmartNearCameraPosition(lastPlayerPosForMonster, MathExtra.DirToAngle(dir2), 40f, UnityEngine.Random.Range(7f, 9f), 6.5f, 3f, 4);

        return result;
    }

    private void DisableEventState(EventType et)
    {
        stateStack[(int)et] = false;
    }

    private void EnableEventState(EventType et)
    {
        stateStack[(int)et] = true;
    }

    private bool EventTypeIsLight(EventType et) => et == EventType.LampFlicker || et == EventType.GlobalLightFlicker || et == EventType.SceneFlicker;
    private bool LightEventIsActive() => GetEventState(EventType.LampFlicker) || GetEventState(EventType.GlobalLightFlicker) || GetEventState(EventType.SceneFlicker);
    private bool EventTypeIsDoor(EventType et) => et == EventType.DoorHandleWiggling || et == EventType.OpenDoorHingeWiggling;
    private bool DoorEventIsActive() => GetEventState(EventType.DoorHandleWiggling) || GetEventState(EventType.OpenDoorHingeWiggling);
    private bool EventTypeIsMonster(EventType et) => et == EventType.SpawnModerateAggressive || et == EventType.SpawnAggressiveTarget || et == EventType.HarmlessSpawn || et == EventType.TrashCanHand;
    private bool MonsterEventIsActive() => GetEventState(EventType.SpawnModerateAggressive) || GetEventState(EventType.SpawnAggressiveTarget) || GetEventState(EventType.HarmlessSpawn) || GetEventState(EventType.TrashCanHand);
    public bool GetEventState(EventType et) => stateStack[(int)et];
    private int GetEventID(EventType et) => (int)et;
    public void ResetCoroutineTime(EventType et, float duration) => timer[(int)et].Resetart(duration);

    public void StartTimer(EventType et, float duration)
    {
        stateStack[(int)et] = true;
        timer[(int)et].Start(duration);
    }

    private List<EventType> GetActiveEvents()
    {
        List<EventType> list = new List<EventType>();

        for (int i = 0; i < stateStack.Length; i++)
        {
            if (stateStack[i])
                list.Add((EventType)i);
        }

        return list;
    }

    public enum EventType
    {
        Sound,
        LampFlicker,
        GlobalLightFlicker,
        SceneFlicker,
        OpenDoorHingeWiggling,
        DoorHandleWiggling,
        RedShoePrint,
        RedEyeDrawing,
        TrashCanHand,
        SpawnAggressiveTarget,
        SpawnModerateAggressive,
        HarmlessSpawn
    }
}
