using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Monster : MonoBehaviour
{
    public NavMeshAgent agent;
    public Animator anim;
    public Animator shaderAnim;
    public Animator lightAnim;
    public MouthRope mouthRope;
    public Transform headPoint;
    public Transform handCamCont;
    public Transform rayHitPoint;
    public Transform detectorRayPoint;
    public LayerMask doorRayMask;
    public Eye[] eye = new Eye[3];
    public float mouthDistance = 0f;
    public float mouthShakingForce = 0f;
    public float eyeShakingForce = 0f;

    private Coroutine currentRMAS = null;

    private float[] soundPlayDelay = new float[] { 6f, 10f };
    private SoundManager.Sound[] maxAggressionSounds = new SoundManager.Sound[]
    {
        SoundManager.Sound.MonsterAggression2,
        SoundManager.Sound.MonsterAggression3,
        SoundManager.Sound.MonsterAggression4,
        SoundManager.Sound.MonsterAggression5,
    };

    public Mode mode { get; private set; } = Mode.None;

    private bool playerTarget = false;
    private bool walkMove = false;
    private bool mouthDistanceShaker = false;
    private bool targetPlayerLook = false;
    private bool eyePlayerLook = false;
    private bool playerAttack = false;
    private bool walkToPlayer = false;
    public bool playerKilling = false;

    private const float defaultY = 0.505f;
    private const float camLerpDuration = 0.5f;
    private const float doorDistance = 1.35f;

    private Vector3 startCamPos;
    private Vector3 startCamRot;
    private Vector3 targetCamRot;
    private float startMouthDistance;
    private float startMouthShakingForce;
    private LerperEnd camToHandLerper = null;

    private Vector3 playerPosOnDoorRay;
    private Vector3 targetPoint;
    private bool pursueToTheEnd;
    private bool wander;
    private Coroutine wanderCoroutine = null;

    private void Update()
    {
        MouthMoving();
        EyeTarget();
        Target();
        FromDoorTarget();

        if (camToHandLerper != null)
            camToHandLerper.Lerping();
    }

    private void Target()
    {
        if (!playerTarget || playerKilling)
            return;

        if (walkToPlayer)
        {
            RaycastHit hit;
            if (Physics.Raycast(rayHitPoint.position, transform.forward, out hit, doorDistance, doorRayMask))
            {
                if (!hit.collider.transform.parent.GetComponent<Door>().IsOpen())
                {
                    walkToPlayer = false;
                    playerPosOnDoorRay = Player.instance.transform.position;
                    float distance = MonsterEventsConfig.GetFromDoorDistance();
                    Vector3 targetPos = transform.position + (-transform.forward * distance);
                    NavMeshHit navHit;
                    NavMesh.SamplePosition(targetPos, out navHit, 3f, NavMesh.AllAreas);
                    agent.destination = navHit.position;
                }
            }

            if (!wander)
            {
                if (pursueToTheEnd)
                    agent.destination = Player.instance.transform.position;
                else
                {
                    RaycastHit detectorHit;
                    agent.destination = targetPoint;

                    if (Vector3.Distance(transform.position, Player.instance.transform.position) < MonsterEventsConfig.monsterDistanceToDetectPlayer)
                    {
                        pursueToTheEnd = true;
                    }
                    else if (Physics.Raycast(detectorRayPoint.position, transform.forward, out detectorHit, 10f))
                    {
                        if (Vector3.Distance(detectorHit.point, Player.instance.transform.position) < 2f)
                        {
                            pursueToTheEnd = true;
                        }
                        else if (Vector3.Distance(transform.position, targetPoint) <= 1.1f)
                        {
                            wander = true;
                            anim.speed = 0.5f;
                            agent.speed /= 2f;
                            wanderCoroutine = StartCoroutine(MonsterWander(0.15f));
                        }
                    }
                    else
                    {
                        if (Vector3.Distance(transform.position, targetPoint) <= 1.1f)
                        {
                            wander = true;
                            anim.speed = 0.5f;
                            agent.speed /= 2f;
                            wanderCoroutine = StartCoroutine(MonsterWander(0.15f));
                        }
                    }
                }
            }
            else
            {
                RaycastHit detectorHit;
                if (Vector3.Distance(transform.position, Player.instance.transform.position) < MonsterEventsConfig.monsterDistanceToDetectPlayer)
                {
                    pursueToTheEnd = true;

                    wander = false;
                    if (wanderCoroutine != null)
                        StopCoroutine(wanderCoroutine);

                    anim.speed = 1f;
                    agent.speed *= 2f;
                }
                else if (Physics.Raycast(detectorRayPoint.position, transform.forward, out detectorHit, 10f))
                {
                    if (Vector3.Distance(detectorHit.point, Player.instance.transform.position) < 2f)
                    {
                        pursueToTheEnd = true;

                        wander = false;
                        if (wanderCoroutine != null)
                            StopCoroutine(wanderCoroutine);

                        anim.speed = 1f;
                        agent.speed *= 2f;
                    }
                }
            }
        }

        Vector2 moveDir = MathExtra.ToPlaneVector2(agent.velocity).normalized;
        if (moveDir != Vector2.zero)
        {
            transform.eulerAngles = new Vector3(0f, 180f - MathExtra.DirToAngle(moveDir), 0f);
        }
        else if (Vector2.Distance(MathExtra.ToPlaneVector2(transform.position), MathExtra.ToPlaneVector2(Player.instance.transform.position)) <= 0.9)
        {
            Vector2 toPlayerDir = (MathExtra.ToPlaneVector2(Player.instance.transform.position) - MathExtra.ToPlaneVector2(transform.position)).normalized;
            transform.eulerAngles = new Vector3(0f, 180f - MathExtra.DirToAngle(toPlayerDir), 0f);

            if (playerAttack)
            {
                KillPlayer();
            }
        }

        if (walkMove)
            anim.SetBool("Walk", moveDir != Vector2.zero);
        else
            anim.SetBool("Run", moveDir != Vector2.zero);
    }

    private void FromDoorTarget()
    {
        if (!playerTarget)
            return;

        if (!walkToPlayer)
        {
            if (pursueToTheEnd)
            {
                if (Vector3.Distance(transform.position, agent.destination) < 1.05f || Vector3.Distance(playerPosOnDoorRay, Player.instance.transform.position) > 2.5f)
                {
                    walkToPlayer = true;
                    agent.destination = Player.instance.transform.position;
                }
                else
                {
                    DoorCheck();
                }
            }
            else
            {
                DoorCheck();
            }
        }

        void DoorCheck()
        {
            RaycastHit hit;
            if (Physics.Raycast(rayHitPoint.position, transform.forward, out hit, doorDistance, doorRayMask))
            {
                if (!hit.collider.transform.parent.GetComponent<Door>().IsOpen())
                {
                    float distance = MonsterEventsConfig.GetFromDoorDistance();
                    Vector3 targetPos = transform.position + (-transform.forward * distance);
                    NavMeshHit navHit;
                    NavMesh.SamplePosition(targetPos, out navHit, 3f, NavMesh.AllAreas);
                    agent.destination = navHit.position;
                }
            }
        }
    }

    public void DeactivateMonster()
    {
        if (wanderCoroutine != null)
            StopCoroutine(wanderCoroutine);
    }

    private IEnumerator MonsterWander(float delay)
    {
        yield return new WaitForSeconds(delay);

        Vector3 newPos = NavigationExtra.GetNearPlayerPostion(transform.position, Random.Range(2f, 4f));
        agent.destination = newPos;
        wanderCoroutine = StartCoroutine(MonsterWander(Random.Range(1f, 3.5f)));
    }

    private void KillPlayer()
    {
        if (StateProvider.playerInExitZone)
            return;

        playerTarget = false;
        playerKilling = true;

        anim.SetBool("Run", false);
        anim.SetBool("Walk", false);

        anim.SetTrigger("KillPlayer");

        Player.instance.controller.SetControllerMode(Controller.ControllerMode.Freeze);
        agent.enabled = false;
        camToHandLerper = new LerperEnd(1f / camLerpDuration, CamToHandLerping, OnCamToHandLerpEnd);
        startCamPos = Player.instance.controller.cam.position;
        startCamRot = MathExtra.ExclusionRotationTo(handCamCont.eulerAngles, Player.instance.controller.cam.eulerAngles);
        targetCamRot = handCamCont.eulerAngles;
        mouthDistanceShaker = false;
        startMouthDistance = mouthDistance;
        startMouthShakingForce = mouthShakingForce;

        StopMaxAggressionSP();

        OnPlayerEyeLook();
        camToHandLerper.StartLerp(true);
    }

    private void CamToHandLerping(float cof)
    {
        Player.instance.controller.cam.position = Vector3.Lerp(startCamPos, handCamCont.position, cof);
        Player.instance.controller.cam.eulerAngles = Vector3.Lerp(startCamRot, targetCamRot, cof);
        mouthDistance = Mathf.Lerp(startMouthDistance, 0.11f, cof);
        mouthShakingForce = Mathf.Lerp(startMouthShakingForce, 10f, cof);
    }

    private void OnCamToHandLerpEnd()
    {
        Player.instance.controller.cam.SetParent(handCamCont);
        PostShader.instance.eye.SetFloat("_t", 0.6f);
    }

    private void MouthMoving()
    {
        if (targetPlayerLook)
            mouthRope.HeadLookAtPlayer(mouthShakingForce);

        if (!mouthDistanceShaker)
            mouthRope.SetDistance(mouthDistance, mouthShakingForce);
        else
            mouthRope.SetDistance(Random.Range(0.37f, 0.43f), mouthShakingForce);
    }

    private void EyeTarget()
    {
        if (!eyePlayerLook)
            return;

        foreach (Eye _eye in eye)
            _eye.LookAtPlayer(eyeShakingForce);
    }

    private void SetEyeTargetActive(bool active)
    {
        if (active == eyePlayerLook)
            return;

        eyePlayerLook = active;
        if (active)
        {
            EyeTarget();
        }
        else
        {
            foreach (Eye _eye in eye)
                _eye.ResetRotation();
        }
    }

    private void StartMaxAggressionSP()
    {
        currentRMAS = StartCoroutine(RandomMaxAggressionSound(0.4f));
    }

    public void StopMaxAggressionSP()
    {
        if (currentRMAS != null)
            StopCoroutine(currentRMAS);
    }

    private IEnumerator RandomMaxAggressionSound(float delay)
    {
        yield return new WaitForSeconds(delay);

        SoundManager.instance.PlaySound3D(maxAggressionSounds[Random.Range(0, maxAggressionSounds.Length)], headPoint.position, headPoint);
        float _delay = Random.Range(soundPlayDelay[0], soundPlayDelay[1]);
        currentRMAS = StartCoroutine(RandomMaxAggressionSound(_delay));
    }

    public void PlayGrabAggressiveSound()
    {
        SoundManager.instance.PlaySound3D(SoundManager.Sound.MonsterAggression2, headPoint.position, headPoint);
    }

    public void OnPlayerEyeLook()
    {
        if (mode == Mode.Peaceful)
        {
            MonsterEventer.instance.MonsterModeEMP();
            MonsterEventer.instance.ResetCoroutineTime(MonsterEventer.EventType.HarmlessSpawn, MonsterEventsConfig.GetAggressiveLifeTimeFromPeaceful());
        }

        if (mode != Mode.MaximumAggressive)
        {
            MonsterEventer.lastMonsterSpawnTime = MonsterEventer.gameTime;
            MonsterEventer.instance.MonsterModeEMP();
            SetMode(Mode.MaximumAggressive, Vector3.zero, true);
        }
    }

    public void SetPosition(Vector3 pos)
    {
        if (agent.enabled)
        {
            agent.enabled = false;
            transform.position = pos;
            agent.enabled = true;
        }
        else
            transform.position = pos;
    }

    public void SetMode(Mode _mode, Vector3 _targetPoint, bool _pursueToTheEnd = false)
    {
        anim.speed = 1f;
        wander = false;

        if (_mode == Mode.None)
        {
            playerAttack = false;
            agent.enabled = false;
            playerTarget = false;
            shaderAnim.SetBool("RedGlitch", false);
            lightAnim.SetBool("GlobalLightFlicker", false);
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
            mouthDistanceShaker = false;
            playerTarget = false;
            targetPlayerLook = false;
            walkToPlayer = false;
            mouthShakingForce = 0f;
            mouthDistance = 0f;
            CameraEffector.instance.shaking = false;
            mouthRope.ResetHeadRotation();
            mouthRope.SetDistance(mouthDistance, mouthShakingForce);
            SetEyeTargetActive(false);
            StopMaxAggressionSP();
        }
        else if (_mode == Mode.AggressiveTarget)
        {
            MonsterEventer.lastMonsterSpawnTime = MonsterEventer.gameTime;
            playerAttack = true;
            agent.enabled = true;
            if (MonsterEventsConfig.monsterHasAlreadySpawn)
                agent.speed = MonsterEventsConfig.monsterSpeedAggressive;
            else
                agent.speed = MonsterEventsConfig.firstMonsterSpeedModerate;
            shaderAnim.SetBool("RedGlitch", false);
            lightAnim.SetBool("GlobalLightFlicker", false);

            if (anim.GetBool("Run"))
                anim.SetBool("Walk", true);

            anim.SetBool("Run", false);
            playerTarget = true;
            walkMove = true;
            walkToPlayer = true;

            mouthShakingForce = 5f;
            mouthDistance = 0.35f;
            mouthDistanceShaker = false;
            CameraEffector.instance.shaking = false;
            targetPlayerLook = true;
            SetEyeTargetActive(true);
            StopMaxAggressionSP();
            MouthMoving();
            MonsterEventsConfig.monsterHasAlreadySpawn = true;
            targetPoint = _targetPoint;
            pursueToTheEnd = _pursueToTheEnd;
        }
        else if (_mode == Mode.MaximumAggressive)
        {
            MonsterEventer.lastMonsterSpawnTime = MonsterEventer.gameTime;
            playerAttack = true;
            agent.enabled = true;
            if (MonsterEventsConfig.monsterHasAlreadySpawn)
                agent.speed = MonsterEventsConfig.monsterSpeedAggressive;
            else
                agent.speed = MonsterEventsConfig.firstMonsterSpeedAggressive;
            shaderAnim.SetBool("RedGlitch", true);
            lightAnim.SetBool("GlobalLightFlicker", true);

            if (anim.GetBool("Walk"))
                anim.SetBool("Run", true);

            anim.SetBool("Walk", false);

            walkMove = false;
            CameraEffector.instance.shaking = true;
            targetPlayerLook = true;

            if (!playerKilling)
            {
                playerTarget = true;
                walkToPlayer = true;
                mouthShakingForce = 15f;
                mouthDistance = 0.45f;
                mouthDistanceShaker = true;
                SetEyeTargetActive(true);
                StartMaxAggressionSP();
                MouthMoving();
            }

            MonsterEventsConfig.monsterHasAlreadySpawn = true;
            targetPoint = _targetPoint;
            pursueToTheEnd = _pursueToTheEnd;
        }
        else if (_mode == Mode.Peaceful)
        {
            playerAttack = false;
            agent.enabled = false;
            playerTarget = false;
            walkToPlayer = false;
            shaderAnim.SetBool("RedGlitch", false);
            lightAnim.SetBool("GlobalLightFlicker", false);
            anim.SetBool("Run", false);
            anim.SetBool("Walk", false);
            mouthDistanceShaker = false;
            playerTarget = false;
            targetPlayerLook = true;
            mouthShakingForce = 0f;
            mouthDistance = 0.02f;
            CameraEffector.instance.shaking = false;
            mouthRope.SetDistance(mouthDistance, mouthShakingForce);
            SetEyeTargetActive(true);
            StopMaxAggressionSP();
        }

        mode = _mode;
        MonsterEventer.instance.MonsterModeEMP();
    }

    public enum Mode
    {
        None,
        AggressiveTarget,
        MaximumAggressive,
        Peaceful
    }
}
