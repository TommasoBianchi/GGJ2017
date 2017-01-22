using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAFlavio : IA {

    public enum FavouriteDirection { left_handed, right_handed }
    FavouriteDirection favouriteDir;

    void Start ()
    {
        int r = Random.Range(0, 1);
        if (r == 0)
            favouriteDir = FavouriteDirection.left_handed;
        else
            favouriteDir = FavouriteDirection.right_handed;
    }

    protected override Direction Decide(WaveInfo[] waves, Vector3 playerPosition)
    {
        foreach (WaveInfo wave in waves)
        {
            if (CalculateQuadDistance(wave, this.transform.position) < 100)
            {
                float DangerRadius = CalculateRadius(wave, this.transform);
                if (DangerRadius > -Mathf.PI/4 && DangerRadius < 0)
                    return Direction.RotateRight;
                if (DangerRadius > 0 && DangerRadius < Mathf.PI/4)
                    return Direction.RotateRight;
            }
        }
        int r = Random.Range(0, 1);
        if (r == 1)
        {
            if (favouriteDir == FavouriteDirection.left_handed)
                return Direction.RotateLeft;
            if (favouriteDir == FavouriteDirection.right_handed)
                return Direction.RotateRight;
        }
        return Direction.GoForward;
    }

    private float CalculateQuadDistance(WaveInfo wave, Vector3 myPosition)
    {
        float distance = Mathf.Pow(wave.center.x - myPosition.x, 2) + Mathf.Pow(wave.center.y - myPosition.y, 2);
        return distance;
    }

    private float CalculateRadius(WaveInfo wave, Transform myPosition)
    {
        Vector3 IADirection = myPosition.transform.TransformDirection(Vector3.up);
        Vector3 WaveDirection = (wave.center - myPosition.transform.position).normalized;
        float IARadius = Mathf.Atan2(IADirection.y, IADirection.x);
        float WaveRadius = Mathf.Atan2(WaveDirection.y, WaveDirection.x);
        return IARadius - WaveRadius;
    }
}
