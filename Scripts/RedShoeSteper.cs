using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class RedShoeSteper : MonoBehaviour
{
    public GameObject redShoePrefab;
    public NavMeshAgent agent;

    private float stepDelay = 0.75f;
    private float stepWidth = 0.09f;
    private const float redShoeY = 0.541f;
    private bool lastStepRight = false;
    private Vector3 lastPos;
    private Vector3 moveDir;

    private void Update()
    {
        DirMeter();
    }

    private void DirMeter()
    {
        moveDir = (lastPos - transform.position).normalized;
        lastPos = transform.position;
    }

    public void Init(Transform[] fourMapCorners, GameObject redShoePrefab)
    {
        this.redShoePrefab = redShoePrefab;
        GenerateStartPosition();
        GenerateTargetPoint(fourMapCorners);
        StartCoroutine(Step());
    }

    private void GenerateStartPosition()
    {
        Vector3 targetPoint = NavigationExtra.GetNearCameraPosition(Player.instance.controller.playerMain.position,
                                                                    Player.instance.controller.playerMain.localEulerAngles.y,
                                                                    40f, Random.Range(3f, 5f));
        agent.enabled = false;
        transform.position = targetPoint;
        agent.enabled = true;
        lastPos = targetPoint;
    }

    private void GenerateTargetPoint(Transform[] fourMapCorners)
    {
        Vector3 farPoint = Vector3.zero;
        float distance = 0f;
        int? farPointID = null;
        
        for (int i = 0; i < fourMapCorners.Length; i++)
        {
            float newDistance = MathExtra.ToPlaneDistanceVector3(Player.instance.transform.position, fourMapCorners[i].position);
            if (farPointID == null)
            {
                farPointID = i;
                farPoint = fourMapCorners[i].position;
                distance = newDistance;
            }
            else if (newDistance < distance)
            {
                farPointID = i;
                farPoint = fourMapCorners[i].position;
                distance = newDistance;
            }
        }

        Vector3 centerPoint = MathExtra.Y0(Vector3.Lerp(fourMapCorners[0].position, fourMapCorners[2].position, 0.5f));
        Vector2 dirCorrect = MathExtra.AngleToDir(Random.Range(-110f, 110f));
        Vector3 dirCorrect3 = new Vector3(dirCorrect.x, 0f, dirCorrect.y);
        farPoint = MathExtra.Y0(farPoint);
        Vector3 dir = ((centerPoint - farPoint).normalized + dirCorrect3).normalized;

        Vector3 pos = centerPoint + (dir * Random.Range(18f, 25f));

        NavMeshHit hit;
        if (NavMesh.SamplePosition(pos, out hit, 10f, NavMesh.AllAreas))
        {
            agent.destination = hit.position;
        }
        else
        {
            agent.destination = pos;
        }
    }

    private IEnumerator Step()
    {
        yield return new WaitForSeconds(stepDelay);

        Transform redShoeClone = Instantiate(redShoePrefab).transform;
        float angleY = MathExtra.DirToAngle(MathExtra.ToPlaneVector2(moveDir));
        redShoeClone.eulerAngles = new Vector3(0f, 180f - angleY, 0f);

        Vector2 posCorrect;

        if (lastStepRight)
        {
            posCorrect = MathExtra.RotateVectorRight(moveDir) * stepWidth;
        }
        else
        {
            posCorrect = MathExtra.RotateVectorLeft(moveDir) * stepWidth;
            redShoeClone.localScale = new Vector3(-redShoeClone.localScale.x, redShoeClone.localScale.y, redShoeClone.localScale.z);
        }

        redShoeClone.position = new Vector3(transform.position.x, redShoeY, transform.position.z) + new Vector3(posCorrect.x, 0f, posCorrect.y);
        SoundManager.instance.RandomMonsterStep(redShoeClone.position);
        lastStepRight = !lastStepRight;

        StartCoroutine(Step());
    }
}
