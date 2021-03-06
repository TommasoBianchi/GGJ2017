﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAFrenk : IA {

    protected override Direction Decide(WaveInfo[] waves, Vector3 playerPosition)
    {
        if (waves.Length == 0)
            return Direction.GoForward;

        int i, minI = 0;
        float sqrDistance, minDistSqr = 2000;
        Vector3 direction;

        for (i = 0; i < waves.Length; i++)
        {
            sqrDistance = (gameObject.transform.position - waves[i].center).sqrMagnitude;

            if (sqrDistance < minDistSqr)
            {
                minDistSqr = sqrDistance;
                minI = i;
            }
        }

        if (minDistSqr <= (waves[minI].radius + 4) * (waves[minI].radius + 4))
        {
            direction = waves[minI].center - gameObject.transform.position;
            float angle = Vector3.Angle(gameObject.transform.up, direction);
            if (Mathf.Abs(angle) < 90) {
                if (angle < 0)
                    return (Direction.RotateLeft);
                else
                    return (Direction.RotateRight);
            }
        }

        return (Direction.GoForward);
    }
}
