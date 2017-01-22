using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour {

    protected enum Direction
    {
        RotateLeft,
        RotateRight,
        GoForward
    }

    WaveController waveController;
    
	void Start () {
        waveController = FindObjectOfType<WaveController>();
	}
	
	void Update () {
        Wave[] waves = waveController.GetWaves();
        WaveInfo[] waveInfo = new WaveInfo[waves.Length];

        for (int i = 0; i < waveInfo.Length; i++)
        {
            waveInfo[i] = waves[i].GetInfo();
        }
	}

    protected virtual Direction Decide(WaveInfo[] waves)
    {
        return Direction.GoForward;
    }

    public struct WaveInfo
    {
        public Vector3 center;
        public float radius;
        public float speed;

        public WaveInfo (Vector3 center, float radius, float speed)
        {
            this.center = center;
            this.radius = radius;
            this.speed = speed;
        }
    }
}
