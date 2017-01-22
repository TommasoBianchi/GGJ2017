using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAFrenk : IA {

    protected override Direction Decide(WaveInfo[] waves, Vector3 playerPosition)
    {
        if (waves.Length == 0)
            return Direction.GoForward;

        int i, minI = 0;
        float distance, minDist = 2000;
        Vector3 direction;

        for (i = 0; i < waves.Length; i++)
        {
            distance = Vector3.Distance(gameObject.transform.position, waves[i].center);

            if (distance < minDist)
            {
                minDist = distance;
                minI = i;
            }
        }

        
        if (minDist <= (waves[minI].radius + 4))
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
