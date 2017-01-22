using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroySpriteAfterSeconds : MonoBehaviour {

    public float seconds;
    public float timeTresholdForAlpha;

    float targetTime;
    SpriteRenderer spriteRenderer;

	void Start () {
        targetTime = Time.time + seconds;
        spriteRenderer = GetComponent<SpriteRenderer>();
	}
	
	void Update () {
        if (targetTime <= Time.time)
            Destroy(gameObject);
        else if(targetTime - timeTresholdForAlpha <= Time.time)
        {
            float t = (targetTime - Time.time) / (seconds - timeTresholdForAlpha);
            Color color = spriteRenderer.color;
            color.a = t;
            spriteRenderer.color = color;
        }
	}
}
