using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static Quaternion GetRotation2D(Transform target, Vector3 towards, float speed = 999999)
    {
        Vector3 vectorToTarget = towards - target.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);
        return Quaternion.RotateTowards(target.rotation, q, Time.deltaTime * speed);
    }
}
