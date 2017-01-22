using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IA : MonoBehaviour {

    public float speed;
    public float turningSpeed;

    protected enum Direction
    {
        RotateLeft,
        RotateRight,
        GoForward
    }

    WaveController waveController;
    PlayerController player;
    
	void Start () {
        waveController = FindObjectOfType<WaveController>();
        player = FindObjectOfType<PlayerController>();
	}
	
	void Update () {
        Wave[] waves = waveController.GetWaves();
        WaveInfo[] waveInfo = new WaveInfo[waves.Length];

        for (int i = 0; i < waveInfo.Length; i++)
        {
            waveInfo[i] = waves[i].GetInfo();
        }

        switch (Decide(waveInfo, player.transform.position))
        {
            case Direction.RotateLeft:
                gameObject.transform.RotateAround(transform.position, Vector3.back, -(turningSpeed * Time.deltaTime));
                break;
            case Direction.RotateRight:
                gameObject.transform.RotateAround(transform.position, Vector3.back, (turningSpeed * Time.deltaTime));
                break;
            default:
                break;
        }

        gameObject.transform.position += transform.up * speed * Time.deltaTime;
    }

    protected virtual Direction Decide(WaveInfo[] waves, Vector3 playerPosition)
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
