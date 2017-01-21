using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {
	public GameObject Player;

	void Update () {
		gameObject.transform.position = Player.transform.position;
		gameObject.transform.position  = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, -50);
	}
}
