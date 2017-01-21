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
        CheckInput();

        LinkedListNode<Wave> waveA = activeWaves.First;
        while(waveA != null)
        {
            LinkedListNode<Wave> waveB = waveA.Next;
            while (waveB != null)
            {
                float sqrDistanceBetweenCenters = (waveA.Value.transform.position - waveB.Value.transform.position).sqrMagnitude;
                float maxDistance = waveA.Value.maxRadius + waveB.Value.maxRadius;
                if (sqrDistanceBetweenCenters < maxDistance * maxDistance)
                {
                    waveA.Value.CheckCollision(waveB.Value);
                }
                waveB = waveB.Next;
            }
            waveA = waveA.Next;
        }
    }

    void CheckInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 pos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            pos.z = 0;
            Wave wave = (Instantiate(wavePrefab, pos, Quaternion.identity) as GameObject).GetComponent<Wave>();
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
}
