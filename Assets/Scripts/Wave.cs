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
    List<EdgeCollider2D> edgeColliders = new List<EdgeCollider2D>();

    const int numberOfPoints = 100; 

	void Start () {
        currentRadius = startingRadius;
        lineRenderers.Add(GetComponent<LineRenderer>());
        edgeColliders.Add(GetComponent<EdgeCollider2D>());

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

    public void Move(float deltaTime)
    {
        currentRadius += speed * deltaTime;

        //int numberOfPoints = lineRenderer.numPositions;
        //Vector3[] positions = new Vector3[numberOfPoints];
        //lineRenderer.GetPositions(positions);
        //for (int i = 0; i < numberOfPoints; i++)
        //{
        //    positions[i] += new Vector3(Mathf.Cos(2 * Mathf.PI / (numberOfPoints - 1) * i),
        //        Mathf.Sin(2 * Mathf.PI / (numberOfPoints - 1) * i), 0) * speed * deltaTime;
        //}
        //lineRenderer.SetPositions(positions);
    }

    void Generate()
    {
        //int numberOfPoints =  Mathf.CeilToInt(Mathf.Max(currentRadius * 10, 50));

        for (int i = 0; i < lineRenderers.Count; i++)
        {
            lineRenderers[i].numPositions = 0;
        }

        int lineRendererIndex = 0;

        List<Vector3> positionsToShow = new List<Vector3>();
        for (int i = 0; i <= numberOfPoints; i++)
        {
            positions[i] = (new Vector3(Mathf.Cos(2 * Mathf.PI / numberOfPoints * i),
                Mathf.Sin(2 * Mathf.PI / numberOfPoints * i), 0) * currentRadius + transform.position);
            if (activeVertices[i])
            {                
                positionsToShow.Add(positions[i]);
            }
            else if(positionsToShow.Count > 0)
            {
                lineRendererIndex = UpdateLineRenderer(lineRendererIndex, positionsToShow);
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
            EdgeCollider2D newEdgeColliders = child.AddComponent<EdgeCollider2D>();
            newEdgeColliders.isTrigger = true;
            edgeColliders.Add(newEdgeColliders);
        }
        lineRenderers[lineRendererIndex].numPositions = positionsToShow.Count;
        lineRenderers[lineRendererIndex].SetPositions(positionsToShow.ToArray());
        edgeColliders[lineRendererIndex].points = positionsToShow.Select(v => (Vector2)(v - transform.position)).ToArray();
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

    public void CheckCollision(Wave otherWave)
    {
        float epsilon = 0.2f;

        float sqrDistanceBetweenCenters = (transform.position - otherWave.transform.position).sqrMagnitude;
        float maxDistance = currentRadius + otherWave.currentRadius + epsilon;
        if (sqrDistanceBetweenCenters > maxDistance * maxDistance)
            return;

        Vector3[] thisPoints = positions;
        Vector3[] otherPoints = otherWave.positions;
        bool[] nextActiveVertices = new bool[numberOfPoints + 1];
        bool[] otherNextActiveVertices = new bool[numberOfPoints + 1];

        for (int i = 0; i <= numberOfPoints; i++)
        {
            nextActiveVertices[i] = activeVertices[i];
            otherNextActiveVertices[i] = otherWave.activeVertices[i];
        }

        for (int i = 0; i < thisPoints.Length; i++)
        {
            if (activeVertices[i] == false || thisPoints[i] == Vector3.zero) continue;
            float sqrDistance = (thisPoints[i] - otherWave.transform.position).sqrMagnitude;
            if (Mathf.Abs(sqrDistance - otherWave.currentRadius * otherWave.currentRadius) < epsilon * epsilon)
            {
                for (int j = 0; j < otherPoints.Length; j++)
                {
                    if (otherWave.activeVertices[j] == false || otherPoints[j] == Vector3.zero) continue;
                    sqrDistance = (thisPoints[i] - otherPoints[j]).sqrMagnitude;
                    if (sqrDistance < epsilon * epsilon * 5)
                    {
                        nextActiveVertices[i] = false;
                        otherNextActiveVertices[j] = false;
                    }
                }
            }
        }

        activeVertices = nextActiveVertices;
        otherWave.activeVertices = otherNextActiveVertices;

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

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
    }
}