using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        //Generate();
	}

    public void Move(float deltaTime)
    {
        currentRadius += speed * deltaTime;

        int numberOfPoints = lineRenderer.numPositions;
        Vector3[] positions = new Vector3[numberOfPoints];
        lineRenderer.GetPositions(positions);
        for (int i = 0; i < numberOfPoints; i++)
        {
            positions[i] += new Vector3(Mathf.Cos(2 * Mathf.PI / (numberOfPoints - 1) * i),
                Mathf.Sin(2 * Mathf.PI / (numberOfPoints - 1) * i), 0) * speed * deltaTime;
        }
        lineRenderer.SetPositions(positions);
    }

    void Generate()
    {
        int numberOfPoints = 100;// Mathf.CeilToInt(Mathf.Max(currentRadius * 10, 50));
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
        if (lineRenderer == null) Debug.LogWarning("null!!");
        Vector3[] thisPoints = new Vector3[lineRenderer.numPositions];
        Vector3[] otherPoints = new Vector3[otherWave.lineRenderer.numPositions];
        lineRenderer.GetPositions(thisPoints);
        otherWave.lineRenderer.GetPositions(otherPoints);
        List<Vector3> thisNextPoints = thisPoints.ToList();
        List<Vector3> otherNextPoints = otherPoints.ToList();

        for (int i = 0; i < thisPoints.Length; i++)
        {
            for (int j = 0; j < otherPoints.Length; j++)
            {
                float sqrDistance = (thisPoints[i] - otherPoints[j]).sqrMagnitude;
                if (sqrDistance > epsilon * epsilon)
                {
                    thisNextPoints.Remove(thisPoints[i]);
                    otherNextPoints.Remove(otherPoints[j]);
                }
            }
        }
    }
}