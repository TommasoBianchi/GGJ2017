﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour {

	public float speed;
	public float turningSpeed;
	public float turningRadius;
	public bool hasShield;

	private GameObject currentPowerUp;
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
				PowerUp.Activate(this);
			}
		}		
	}
}
