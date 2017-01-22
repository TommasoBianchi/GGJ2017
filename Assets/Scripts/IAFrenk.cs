using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IAFrenk : IA {

	protected override Direction Decide(WaveInfo[] waves, Vector3 playerPosition){
		int i, minI = 0;
		float distance, minDist = 2000;
		Vector3 direction;
		float[] directions = new float[waves.Length];
		for (i = 0; i < waves.Length; i++) {
			distance = Vector3.Distance(gameObject.transform.position, waves[i].center);
			directions[i] = distance;
			}
		for (i=0; i < directions.Length ; i++) {
			if(directions[i] < minDist){
				minDist = directions[i];
				minI = i;
			}
		if (Vector3.Distance(gameObject.transform.position, waves[minI].center) <= (waves[minI].radius+4)) {
				direction = gameObject.transform.position - waves[minI].center;
				if (Vector3.Angle(gameObject.transform.forward, direction) > 0)
					return(Direction.RotateLeft);
				else
					return(Direction.RotateRight); 
		} 
	}
	return(Direction.GoForward);
}
