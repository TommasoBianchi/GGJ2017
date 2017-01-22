using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Wave : MonoBehaviour {

    public float startingRadius = 0.1f;
    public float maxRadius = 10;
    public float speed = 1;

    [HideInInspector]
    public System.Func<bool> canStartSimulateWave;

    float currentRadius;
    WaveController waveController;
    LinkedListNode<Wave> waveControllerNode;
    bool[] activeVertices = new bool[numberOfPoints + 1];
    Vector3[] positions = new Vector3[numberOfPoints + 1];
    List<LineRenderer> lineRenderers = new List<LineRenderer>();
    Vector3 center;
    bool canHitPlayer = true;
    bool simulateWave = false;

    const int numberOfPoints = 500;
    const int minNumberOfPointsForLine = 30;

    void StartCircle () {
        currentRadius = startingRadius;
        lineRenderers.Add(GetComponent<LineRenderer>());
        center = transform.position;

        for (int i = 0; i < activeVertices.Length; i++)
        {
            activeVertices[i] = true;
        }

        simulateWave = true;

        Generate();
	}
	
	void Update () {
        if (simulateWave)
        {
            Move(Time.deltaTime);

            if (currentRadius > maxRadius)
                Die();

            if (activeVertices[0] == false)
                activeVertices[activeVertices.Length - 1] = activeVertices[0];
            else
                activeVertices[0] = activeVertices[activeVertices.Length - 1];

            Generate();
        }
        else
        {
            if (canStartSimulateWave())
                StartCircle();
        }
	}

    void Move(float deltaTime)
    {
        currentRadius += speed * deltaTime;
    }

    void Generate()
    {
        int lineRendererIndex = 0;

        List<Vector3> positionsToShow = new List<Vector3>(numberOfPoints);
        for (int i = 0; i <= numberOfPoints; i++)
        {
            positions[i] = (new Vector3(Mathf.Cos(2 * Mathf.PI / numberOfPoints * i),
                Mathf.Sin(2 * Mathf.PI / numberOfPoints * i), 0) * currentRadius + center);
            if (activeVertices[i])
            {                
                positionsToShow.Add(positions[i]);
            }
            else if(positionsToShow.Count > minNumberOfPointsForLine)
            {
                lineRendererIndex = UpdateLineRendererInfo(lineRendererIndex, positionsToShow);
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
            lineRendererIndex = UpdateLineRendererInfo(lineRendererIndex, positionsToShow);
        }

        for (int i = 0; i < lineRenderers.Count; i++)
        {
            float alpha = (maxRadius - currentRadius) / (maxRadius - startingRadius);
            alpha = 1 - (1 - alpha) * (1 - alpha);
            canHitPlayer = canHitPlayer && alpha > 0.2;
            Color color = new Color(1, 1, 1, alpha);

            lineRenderers[i].startColor = color;
            lineRenderers[i].endColor = color;
        }
    }

    int UpdateLineRendererInfo(int lineRendererIndex, List<Vector3> positionsToShow)
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

    public void CheckCollisionWithPlayer(PlayerController player)
    {
        if (!canHitPlayer)
            return;

        float epsilon = 0.3f;
        float maxDistance = currentRadius + epsilon;
        Vector3 playerPosition = player.transform.position;

        Vector3[] thisPoints = positions;

        float sqrDistance = SqrDistanceBetweenVectors2D(center, playerPosition);
        if (sqrDistance < maxDistance * maxDistance)
        {
            for (int j = 0; j < positions.Length; j++)
            {
                if (activeVertices[j] == false) continue;
                sqrDistance = SqrDistanceBetweenVectors2D(positions[j], playerPosition);
                if (sqrDistance < epsilon * epsilon)
                {
                    canHitPlayer = player.WaveTouch();
                    return;
                }
            }
        }
    }

    public void CheckCollisionsWithWaterlilies(Vector3[] waterliliesPositions, float[] waterliliesRadius)
    {
        float epsilon = 0.3f;

        for (int i = 0; i < waterliliesPositions.Length; i++)
        {
            float maxDistance = currentRadius + waterliliesRadius[i] + epsilon;
            float sqrDistance = SqrDistanceBetweenVectors2D(center, waterliliesPositions[i]);
            if(sqrDistance < maxDistance * maxDistance)
            {
                for (int j = 0; j < positions.Length; j++)
                {
                    if (activeVertices[j] == false) continue;
                    sqrDistance = SqrDistanceBetweenVectors2D(positions[j], waterliliesPositions[i]);
                    if(sqrDistance - waterliliesRadius[i] * waterliliesRadius[i] < epsilon * epsilon)
                    {
                        activeVertices[j] = false;
                    }
                }
            }
        }
    }

    public bool[] CheckCollisionWithWave(Wave otherWave)
    {
        float epsilon = 0.3f;
        
        bool[] nextActiveVertices = new bool[numberOfPoints + 1];
        bool[] otherNextActiveVertices = new bool[numberOfPoints + 1];

        for (int i = 0; i <= numberOfPoints; i++)
        {
            nextActiveVertices[i] = activeVertices[i];
            otherNextActiveVertices[i] = otherWave.activeVertices[i];
        }

        float sqrDistanceBetweenCenters = SqrDistanceBetweenVectors2D(center, otherWave.center);
        float maxDistance = currentRadius + otherWave.currentRadius + epsilon;
        if (sqrDistanceBetweenCenters > maxDistance * maxDistance)
            return nextActiveVertices;

        for (int i = 0; i < positions.Length; i++)
        {
            if (activeVertices[i] == false) continue;
            float sqrDistance = SqrDistanceBetweenVectors2D(positions[i], otherWave.center);
            if (Mathf.Abs(sqrDistance - otherWave.currentRadius * otherWave.currentRadius) < epsilon * epsilon * 2)
            {
                for (int j = 0; j < otherWave.positions.Length; j++)
                {
                    if (otherWave.activeVertices[j] == false) continue;
                    sqrDistance = SqrDistanceBetweenVectors2D(positions[i], otherWave.positions[j]);
                    if (sqrDistance < epsilon * epsilon)
                    {
                        nextActiveVertices[i] = false;
                        otherNextActiveVertices[j] = false;
                    }
                }
            }
        }

        return nextActiveVertices;
    }

    public void CheckCollisionWithIA(IA[] IAObjects)
    {
        float epsilon = 0.3f;
        float maxDistance = currentRadius + epsilon;

        for (int i = 0; i < IAObjects.Length; i++)
        {
            Vector3 playerPosition = IAObjects[i].transform.position;

            Vector3[] thisPoints = positions;

            float sqrDistance = SqrDistanceBetweenVectors2D(center, playerPosition);
            if (sqrDistance < maxDistance * maxDistance)
            {
                for (int j = 0; j < positions.Length; j++)
                {
                    if (activeVertices[j] == false) continue;
                    sqrDistance = SqrDistanceBetweenVectors2D(positions[j], playerPosition);
                    if (sqrDistance < epsilon * epsilon)
                    {
                        IAObjects[i].Die();
                        return;
                    }
                }
            }
        }
    }

    float SqrDistanceBetweenVectors2D(Vector3 a, Vector3 b)
    {
        return (a.x - b.x) * (a.x - b.x) + (a.y - b.y) * (a.y - b.y);
    }

    public void SetActiveVertices(bool[] activeVertices)
    {
        this.activeVertices = activeVertices;
    }

    public IA.WaveInfo GetInfo()
    {
        return new IA.WaveInfo(center, currentRadius, speed);
    }
}