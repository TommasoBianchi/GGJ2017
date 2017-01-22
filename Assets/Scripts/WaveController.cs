using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Threading;

public class WaveController : MonoBehaviour {

    public GameObject wavePrefab;

    LinkedList<Wave> activeWaves = new LinkedList<Wave>();
    IA.WaveInfo[] waveInfoArray;
    GameController gameController;
    PlayerController player;
    
	void Start () {
        gameController = FindObjectOfType<GameController>();
        player = FindObjectOfType<PlayerController>();
	}
	
	void Update ()
    {
        CheckCollisions();
    }

    void CheckCollisions()
    {
        GameObject[] waterlilies = gameController.getWaterlilies();
        Vector3[] waterliliesPositions = waterlilies.Select(w => w.transform.position).ToArray();
        float[] waterliliesRadius = waterlilies.Select(w => 1.4f * w.transform.localScale.x / 4f).ToArray();
        IA[] IAObjects = gameController.getIAObjects();

        //Thread thread = new Thread(new ThreadStart(() =>
        //{
            LinkedListNode<Wave> waveA = activeWaves.First;
            while (waveA != null)
            {
                // Check collisions with other waves
                LinkedListNode<Wave> waveB = waveA.Next;
                while (waveB != null)
                {
                    //float sqrDistanceBetweenCenters = (waveA.Value.center - waveB.Value.transform.position).sqrMagnitude;
                    //float maxDistance = waveA.Value.maxRadius + waveB.Value.maxRadius;
                    //if (sqrDistanceBetweenCenters < maxDistance * maxDistance)
                    //{
                    bool[] waveAVertices = waveA.Value.CheckCollisionWithWave(waveB.Value);
                    bool[] waveBVertices = waveB.Value.CheckCollisionWithWave(waveA.Value);
                    waveA.Value.SetActiveVertices(waveAVertices);
                    waveB.Value.SetActiveVertices(waveBVertices);
                    //}
                    waveB = waveB.Next;
                }

                // Check collisions with waterlilies
                waveA.Value.CheckCollisionsWithWaterlilies(waterliliesPositions, waterliliesRadius);

                // Check collisions with the player
                waveA.Value.CheckCollisionWithPlayer(player);

                // Check collisions with IA
                waveA.Value.CheckCollisionWithIA(IAObjects);

                waveA = waveA.Next;
            }
        //}));
        //thread.Start();
    }
    
    public LinkedListNode<Wave> AddWave(Wave wave)
    {
        waveInfoArray = null;
        return activeWaves.AddFirst(wave);
    }

    public void RemoveWave(LinkedListNode<Wave> wave)
    {
        waveInfoArray = null;
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

    public IA.WaveInfo[] GetWaves()
    {
        if (waveInfoArray == null)
            waveInfoArray = activeWaves.Select(w => w.GetInfo()).ToArray();
        return waveInfoArray;
    }
}
