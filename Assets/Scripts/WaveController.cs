using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveController : MonoBehaviour {

    public GameObject wavePrefab;

    LinkedList<Wave> activeWaves = new LinkedList<Wave>();
    GameController gameController;
    PlayerController player;
    
	void Start () {
        gameController = FindObjectOfType<GameController>();
        player = FindObjectOfType<PlayerController>();
	}
	
	void Update ()
    {
        //CheckInput(); // this is only for testing
        CheckCollisions();
    }

    void CheckCollisions()
    {
        Vector2[] waterliliesPos = gameController.getWaterliliesPos().ToArray();

        LinkedListNode<Wave> waveA = activeWaves.First;
        while (waveA != null)
        {
            // Check collisions with other waves
            LinkedListNode<Wave> waveB = waveA.Next;
            while (waveB != null)
            {
                float sqrDistanceBetweenCenters = (waveA.Value.transform.position - waveB.Value.transform.position).sqrMagnitude;
                float maxDistance = waveA.Value.maxRadius + waveB.Value.maxRadius;
                if (sqrDistanceBetweenCenters < maxDistance * maxDistance)
                {
                    bool[] waveAVertices = waveA.Value.CheckCollisionWithWave(waveB.Value);
                    bool[] waveBVertices = waveB.Value.CheckCollisionWithWave(waveA.Value);
                    waveA.Value.SetActiveVertices(waveAVertices);
                    waveB.Value.SetActiveVertices(waveBVertices);
                }
                waveB = waveB.Next;
            }

            // Check collisions with waterlilies
            waveA.Value.CheckCollisionsWithWaterlilies(waterliliesPos, 1.4f);

            // Check collisions with the player
            waveA.Value.CheckCollisionWithPlayer(player);

            waveA = waveA.Next;
        }
    }

    float lastTime = 0;
    void CheckInput()
    {
        if (Time.time - lastTime > 1)
        {
            lastTime = Time.time;           
            Vector3 pos = new Vector3(Random.Range(-20f, 20f), Random.Range(-10f, 10f), 0);
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

    public void SpawnWave(Vector3 position, float speed, float maxRadius, float startingRadius = 0.1f, System.Func<bool> canSimulate = null)
    {
        Wave wavePrefabComponent = wavePrefab.GetComponent<Wave>();
        wavePrefabComponent.speed = speed;
        wavePrefabComponent.maxRadius = maxRadius;
        wavePrefabComponent.startingRadius = startingRadius;
        Wave wave = (Instantiate(wavePrefab, position, Quaternion.identity) as GameObject).GetComponent<Wave>();
        wave.name = "Wave" + activeWaves.Count;
        wave.SetWaveController(this);
        if (canSimulate == null)
            wave.canStartSimulateWave = () => true;
        else
            wave.canStartSimulateWave = canSimulate;
    }
}
