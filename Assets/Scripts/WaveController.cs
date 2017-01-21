using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public GameObject wavePrefab;

    LinkedList<Wave> activeWaves = new LinkedList<Wave>();
    
	void Start () {
		
	}
	
	void Update ()
    {
        //CheckInput(); // this is only for testing
        CheckCollisions();
    }

    void CheckCollisions()
    {
        LinkedListNode<Wave> waveA = activeWaves.First;
        while (waveA != null)
        {
            LinkedListNode<Wave> waveB = waveA.Next;
            while (waveB != null)
            {
                float sqrDistanceBetweenCenters = (waveA.Value.transform.position - waveB.Value.transform.position).sqrMagnitude;
                float maxDistance = waveA.Value.maxRadius + waveB.Value.maxRadius;
                if (sqrDistanceBetweenCenters < maxDistance * maxDistance)
                {
                    bool[] waveAVertices = waveA.Value.CheckCollision(waveB.Value);
                    bool[] waveBVertices = waveB.Value.CheckCollision(waveA.Value);
                    waveA.Value.SetActiveVertices(waveAVertices);
                    waveB.Value.SetActiveVertices(waveBVertices);
                }
                waveB = waveB.Next;
            }
            waveA = waveA.Next;
        }
    }

    float lastTime = 0;
    void CheckInput()
    {
        if (Time.time - lastTime > 1)
        {
            lastTime = Time.time;
            Vector3 pos = new Vector3(Random.Range(-10f, 10f), Random.Range(-5f, 5f), 0);
            Wave wave = (Instantiate(wavePrefab, pos, Quaternion.identity) as GameObject).GetComponent<Wave>();
            wave.speed = Random.Range(0.5f, 1.5f);
            wave.maxRadius = wave.speed * 10;
            wave.name = "Wave" + activeWaves.Count;
            wave.SetWaveController(this);
        }
    }

    public LinkedListNode<Wave> AddWave(Wave wave)
    {
        return activeWaves.AddFirst(wave);
    }

    public void RemoveWave(LinkedListNode<Wave> wave)
    {
        activeWaves.Remove(wave);
    }

    public void SpawnWave(Vector3 position, float speed, float maxRadius, float startingRadius = 0.1f)
    {
        Wave wave = (Instantiate(wavePrefab, position, Quaternion.identity) as GameObject).GetComponent<Wave>();
        wave.speed = speed;
        wave.maxRadius = maxRadius;
        wave.startingRadius = startingRadius;
        wave.name = "Wave" + activeWaves.Count;
        wave.SetWaveController(this);
    }
}
