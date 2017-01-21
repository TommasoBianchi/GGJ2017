using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float turningSpeed;
	public float turningRadius;
	public bool hasShield;

	private PowerUp currentPowerUp;
	private bool KeyPressed = false;
	private float FirstClick = 0;
	private int count = 0;


	// Use this for initialization
	void Start () {
		hasShield = false;	
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		if(KeyPressed && Time.time - FirstClick > 0.2f)
			KeyPressed = false;

		gameObject.transform.position += transform.forward * speed * Time.deltaTime;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
		if (!KeyPressed && Input.GetKey("space")) {
			gameObject.transform.RotateAround(transform.position, Vector3.back, (turningRadius * Time.deltaTime));
		}
		if (Input.GetKeyDown ("space")) {
			//gameObject.transform.RotateAround(transform.position, Vector3.back, turningRadius);
			if (!KeyPressed) {
				KeyPressed = true;
				FirstClick = Time.time;
			}
			else {
				if (currentPowerUp != null) {
					currentPowerUp.Activate(this);
				}
			}
		}		
	}


	void OnTriggerEnter(Collider other)
    {
		if (other.tag == "powerup" ) {
			currentPowerUp = other.GetComponent<PowerUp>();
            float RandomX = Random.Range(-20000, 20000);
            float RandomY = Random.Range(-20000, 20000);
            other.gameObject.transform.position = new Vector3(RandomX, RandomY, 0);
		}
    }
}