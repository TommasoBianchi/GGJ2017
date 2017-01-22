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
    Animator animator;
    
	void Start () {
        waveController = FindObjectOfType<WaveController>();
        player = FindObjectOfType<PlayerController>();
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        WaveInfo[] waveInfo = waveController.GetWaves();

        switch (Decide(waveInfo, player.transform.position))
        {
            case Direction.RotateLeft:
                gameObject.transform.RotateAround(transform.position, Vector3.back, -(turningSpeed * Time.deltaTime));
                animator.SetBool("IsTurning", true);
                break;
            case Direction.RotateRight:
                gameObject.transform.RotateAround(transform.position, Vector3.back, (turningSpeed * Time.deltaTime));
                animator.SetBool("IsTurning", true);
                break;
            default:
                if ((transform.position - player.transform.position).sqrMagnitude > 2500)
                {
                    gameObject.transform.position = player.transform.position - (gameObject.transform.position - player.transform.position);
                    gameObject.transform.rotation = Quaternion.LookRotation(Vector3.forward, player.transform.position - transform.position);
                }
                animator.SetBool("IsTurning", false);
                break;
        }

        gameObject.transform.position += transform.up * speed * Time.deltaTime;
    }

    public void Die()
    {
        animator.SetTrigger("Die");
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
