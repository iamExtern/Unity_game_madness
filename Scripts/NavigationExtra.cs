using UnityEngine;
using UnityEngine.AI;

public static class NavigationExtra
{
    public static Vector3 GetNearCameraPosition(Vector3 camPos, float camAngle, float angleRandomMax, float distance)
    {
        float angle = Random.Range(camAngle - angleRandomMax, camAngle + angleRandomMax);
        Vector2 dir = MathExtra.AngleToDir(angle) * distance;
        Vector3 targetPoint = camPos + new Vector3(dir.x, 0f, dir.y);

        NavMeshHit hit;
        NavMesh.SamplePosition(targetPoint, out hit, 10f, NavMesh.AllAreas);

        if (hit.hit && !MathExtra.Vector3AxisIsInfinity(hit.position))
            return hit.position;
        else
            return GetReservePosition(camPos, distance, distance + 30f);
    }

    public static Vector3 GetSmartNearCameraPosition(Vector3 camPos, float camAngle, float angleRandomMax, float distance, float minDistance, float step, int steps)
    {
        Vector3 lastPos = Vector3.zero;
        for (int i = 1; i <= steps; i++)
        {
            float angle = Random.Range(camAngle - angleRandomMax, camAngle + angleRandomMax);
            Vector2 dir = MathExtra.AngleToDir(angle) * (distance + (step * (i - 1)));
            Vector3 targetPoint = camPos + new Vector3(dir.x, 0f, dir.y);

            NavMeshHit hit;
            NavMesh.SamplePosition(targetPoint, out hit, 2f, NavMesh.AllAreas);
            lastPos = hit.position;

            if (Vector3.Distance(hit.position, camPos) >= minDistance)
            {
                if (hit.hit && !MathExtra.Vector3AxisIsInfinity(lastPos))
                    return hit.position;
                else
                    return GetReservePosition(camPos, minDistance, minDistance + 30f);
            }
        }

        if (!MathExtra.Vector3AxisIsInfinity(lastPos))
            return lastPos;
        else
            return GetReservePosition(camPos, minDistance, minDistance + 30f);
    }

    public static Vector3 GetNearPlayerPostion(Vector3 camPos, float distance)
    {
        float angle = Random.Range(0f, 360f);
        Vector2 dir = MathExtra.AngleToDir(angle) * distance;
        Vector3 targetPoint = camPos + new Vector3(dir.x, 0f, dir.y);

        NavMeshHit hit;
        NavMesh.SamplePosition(targetPoint, out hit, 10f, NavMesh.AllAreas);

        if (hit.hit && !MathExtra.Vector3AxisIsInfinity(hit.position))
            return hit.position;
        else
            return GetReservePosition(camPos, distance, distance + 30f);
    }

    public static Vector3 GetNearPointByDirPosition(Vector3 point, Vector3 dir, float distance)
    {
        dir *= distance;
        Vector3 targetPoint = point + new Vector3(dir.x, 0f, dir.y);

        NavMeshHit hit;
        NavMesh.SamplePosition(targetPoint, out hit, 10f, NavMesh.AllAreas);

        if (hit.hit && !MathExtra.Vector3AxisIsInfinity(hit.position))
            return hit.position;
        else
            return GetReservePosition(point, distance, distance + 30f);
    }

    private static Vector3 GetReservePosition(Vector3 camPos, float minDistance, float maxDistance)
    {
        for (int i = 0; i < MonsterEventer.instance.reserveMonsterPoints.Length; i++)
        {
            float distance = Vector3.Distance(MonsterEventer.instance.reserveMonsterPoints[i].position, camPos);
            if (distance <= maxDistance && distance >= minDistance)
            {
                return MonsterEventer.instance.reserveMonsterPoints[i].position;
            }
        }

        return MonsterEventer.instance.reserveMonsterPoints[Random.Range(0, MonsterEventer.instance.reserveMonsterPoints.Length)].position;
    }
}
