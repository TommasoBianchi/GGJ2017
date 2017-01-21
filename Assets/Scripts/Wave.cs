using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave : MonoBehaviour {

    public float startingRadius = 0.1f;
    public float maxRadius = 10;
    public float speed = 1;

    float currentRadius;
    WaveController waveController;
    LinkedListNode<Wave> waveControllerNode;
    bool[] activeVertices = new bool[numberOfPoints + 1];
    Vector3[] positions = new Vector3[numberOfPoints + 1];
    List<LineRenderer> lineRenderers = new List<LineRenderer>();

    const int numberOfPoints = 1000;
    const int minNumberOfPointsForLine = 50;

	void Start () {
        currentRadius = startingRadius;
        lineRenderers.Add(GetComponent<LineRenderer>());

        for (int i = 0; i < activeVertices.Length; i++)
        {
            activeVertices[i] = true;
        }

        Generate();
	}
	
	void Update () {
        Move(Time.deltaTime);

        if (currentRadius > maxRadius)
            Die();

        if (activeVertices[0] == false)
            activeVertices[activeVertices.Length - 1] = activeVertices[0];
        else
            activeVertices[0] = activeVertices[activeVertices.Length - 1];

        Generate();
	}

    void Move(float deltaTime)
    {
        currentRadius += speed * deltaTime;
    }

    void Generate()
    {
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].numPositions = 0;
        }

        int lineRendererIndex = 0;

        List<Vector3> positionsToShow = new List<Vector3>(numberOfPoints);
        for (int i = 0; i <= numberOfPoints; i++)
        {
            positions[i] = (new Vector3(Mathf.Cos(2 * Mathf.PI / numberOfPoints * i),
                Mathf.Sin(2 * Mathf.PI / numberOfPoints * i), 0) * currentRadius + transform.position);
            if (activeVertices[i])
            {                
                positionsToShow.Add(positions[i]);
            }
            else if(positionsToShow.Count > minNumberOfPointsForLine)
            {
                lineRendererIndex = UpdateLineRenderer(lineRendererIndex, positionsToShow);
            }
            else if (positionsToShow.Count > 0)
            {
                for (int j = 0; j < minNumberOfPointsForLine; j++)
                {
                    if(i - j - 1 >= 0)
                    {
                        if (activeVertices[i - j - 1])
                            activeVertices[i - j - 1] = false;
                        else
                            break;
                    }
                }
                positionsToShow.Clear();
            }
        }
        if (positionsToShow.Count > 0)
        {
            lineRendererIndex = UpdateLineRenderer(lineRendererIndex, positionsToShow);
        }

        float alpha = (maxRadius - currentRadius) / (maxRadius - startingRadius);
        alpha = 1 - (1 - alpha) * (1 - alpha);
        Color color = new Color(1, 1, 1, alpha);
        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].startColor = color;
            lineRenderers[i].endColor = color;
        }        
    }

    int UpdateLineRenderer(int lineRendererIndex, List<Vector3> positionsToShow)
    {
        if (lineRendererIndex >= lineRenderers.Count)
        {
            GameObject child = new GameObject();
            child.transform.SetParent(transform);
            LineRenderer newLineRenderer = child.AddComponent<LineRenderer>();
            LineRenderer oldLineRenderer = GetComponent<LineRenderer>();
            newLineRenderer.sharedMaterial = oldLineRenderer.sharedMaterial;
            newLineRenderer.startWidth = oldLineRenderer.startWidth;
            newLineRenderer.endWidth = oldLineRenderer.endWidth;
            lineRenderers.Add(newLineRenderer);
        }
        lineRenderers[lineRendererIndex].numPositions = positionsToShow.Count;
        lineRenderers[lineRendererIndex].SetPositions(positionsToShow.ToArray());
        positionsToShow.Clear();
        lineRendererIndex++;
        return lineRendererIndex;
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

    public bool[] CheckCollision(Wave otherWave)
    {
        float epsilon = 0.3f;

        Vector3[] thisPoints = positions;
        Vector3[] otherPoints = otherWave.positions;
        bool[] nextActiveVertices = new bool[numberOfPoints + 1];
        bool[] otherNextActiveVertices = new bool[numberOfPoints + 1];

        for (int i = 0; i <= numberOfPoints; i++)
        {
            nextActiveVertices[i] = activeVertices[i];
            otherNextActiveVertices[i] = otherWave.activeVertices[i];
        }

        float sqrDistanceBetweenCenters = SqrDistanceBetweenVectors2D(transform.position, otherWave.transform.position);
        float maxDistance = currentRadius + otherWave.currentRadius + epsilon;
        if (sqrDistanceBetweenCenters > maxDistance * maxDistance)
            return nextActiveVertices;

        for (int i = 0; i < thisPoints.Length; i++)
        {
            if (activeVertices[i] == false) continue;
            float sqrDistance = SqrDistanceBetweenVectors2D(thisPoints[i], otherWave.transform.position);
            if (Mathf.Abs(sqrDistance - otherWave.currentRadius * otherWave.currentRadius) < epsilon * epsilon * 2)
            {
                for (int j = 0; j < otherPoints.Length; j++)
                {
                    if (otherWave.activeVertices[j] == false) continue;
                    sqrDistance = SqrDistanceBetweenVectors2D(thisPoints[i], otherPoints[j]);
                    if (sqrDistance < epsilon * epsilon)
                    {
                        nextActiveVertices[i] = false;
                        otherNextActiveVertices[j] = false;
                    }
                }
            }
        }

        return nextActiveVertices;
        //activeVertices = nextActiveVertices;
        //otherWave.activeVertices = otherNextActiveVertices;

        //Vector3[] thisPoints = positions;
        //Vector3[] otherPoints = otherWave.positions;
        //List<Vector3> thisNextPoints = positions.ToList();
        //List<Vector3> otherNextPoints = otherWave.positions.ToList();

        //for (int i = 0; i < thisPoints.Length; i++)
        //{
        //    if (activeVertices[i] == false || thisPoints[i] == Vector3.zero) continue;
        //    for (int j = 0; j < otherPoints.Length; j++)
        //    {
        //        if (otherWave.activeVertices[j] == false || otherPoints[j] == Vector3.zero) continue;
        //        float sqrDistance = (thisPoints[i] - otherPoints[j]).sqrMagnitude;
        //        if (sqrDistance < epsilon * epsilon)
        //        {
        //            activeVertices[i] = false;
        //            otherWave.activeVertices[j] = false;
        //        }
        //    }
        //}
    }

    float SqrDistanceBetweenVectors2D(Vector3 a, Vector3 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }

    public void SetActiveVertices(bool[] activeVertices)
    {
        this.activeVertices = activeVertices;
    }
}