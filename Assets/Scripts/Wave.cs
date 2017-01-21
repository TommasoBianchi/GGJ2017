using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wave : MonoBehaviour {

    public float startingRadius = 0.1f;
    public float maxRadius = 10;
    public float speed = 1;

    float currentRadius;
    LineRenderer lineRenderer;
    WaveController waveController;
    LinkedListNode<Wave> waveControllerNode;

	void Start () {
        currentRadius = startingRadius;
        lineRenderer = GetComponent<LineRenderer>();

        Generate();
	}
	
	void Update () {
        Move(Time.deltaTime);

        if (currentRadius > maxRadius)
            Die();

        Generate();
	}

    public void Move(float deltaTime)
    {
        currentRadius += speed * deltaTime;
    }

    void Generate()
    {
        int numberOfPoints = Mathf.CeilToInt(Mathf.Max(currentRadius * 10, 50));
        Vector3[] positions = new Vector3[numberOfPoints + 1];
        for (int i = 0; i <= numberOfPoints; i++)
        {
            positions[i] = new Vector3(Mathf.Cos(2 * Mathf.PI / numberOfPoints * i),
                Mathf.Sin(2 * Mathf.PI / numberOfPoints * i), 0) * currentRadius + transform.position;
        }
        lineRenderer.numPositions = numberOfPoints + 1;
        lineRenderer.SetPositions(positions);

        float alpha = (maxRadius - currentRadius) / (maxRadius - startingRadius);
        alpha = 1 - (1 - alpha) * (1 - alpha);
        Color color = new Color(1, 1, 1, alpha);
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;
    }

    public void SetWaveController(WaveController waveController)
    {
        this.waveController = waveController;
        this.waveControllerNode = waveController.AddWave(this);
    }

    void Die()
    {
        waveController.RemoveWave(waveControllerNode);
        Destroy(gameObject);
    }
    public void CheckCollision(Wave otherWave)
    {
        float epsilon = 0.01f;

        List<Vector3> thisNextPoints = new List<Vector3>(lineRenderer.numPositions);
        List<Vector3> otherNextPoints = new List<Vector3>(otherWave.lineRenderer.numPositions);
        Vector3[] thisPoints = new Vector3[lineRenderer.numPositions];
        Vector3[] otherPoints = new Vector3[otherWave.lineRenderer.numPositions];
        lineRenderer.GetPositions(thisPoints);
        otherWave.lineRenderer.GetPositions(otherPoints);

        for (int i = 0; i < thisPoints.Length; i++)
        {
            for (int j = 0; j < otherPoints.Length; j++)
            {
                float sqrDistance = (thisPoints[i] - otherPoints[j]).sqrMagnitude;
                if (sqrDistance > epsilon * epsilon)
                {
                    thisNextPoints.Add(thisPoints[i]);
                }
            }
        }
    }
}