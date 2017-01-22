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
    float dieTime = -1;
    
	void Start () {
        waveController = FindObjectOfType<WaveController>();
        player = FindObjectOfType<PlayerController>();
        animator = GetComponent<Animator>();
	}
	
	void Update () {
        if (dieTime > 0 && Time.deltaTime > dieTime)
        {
            transform.position = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0) + player.transform.position;
            animator.SetTrigger("Live");
            dieTime = -1;
        }
        else if (dieTime == -1)
        {
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
    }

    public void Die()
    {
        animator.SetTrigger("Die");
        dieTime = Time.time;
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
