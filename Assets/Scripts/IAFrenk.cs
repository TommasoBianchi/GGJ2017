using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAFrenk : IA {

	protected override Direction Decide(WaveInfo[] waves){
		int i;
		float distance;
		Vector3 direction;
		for (i = 0; i < waves.Length; i++) {
			distance = Vector3.Distance(gameObject.transform.position, waves[i].center);
			if (distance <= (waves[i].radius+2)) {
				direction = gameObject.transform.position - waves[i].center;
				if (Vector3.Angle(gameObject.transform.forward, direction) > 0)
					return(Direction.RotateLeft);
				else
					return(Direction.RotateRight);
			} 
		}
		return(Direction.GoForward);
	}
}
