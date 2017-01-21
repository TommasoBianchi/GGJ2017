using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject waterlily;
    public GameObject player;
    public float maxWaterlilyDistance;
    public int waterliliesNumber;
    public float waterlilyRadius;

    private List<Vector2> waterliliesPos = new List<Vector2>();
    private bool canInstantiateWaterlily = true;

    private void SpawnWaterlilies()
    {
        for (int i=0; i < waterliliesNumber; i++)
        {
            waterliliesPos.Add(new Vector2(0, 0));
        }

        for (int i=0; i < waterliliesNumber; i++)
        {
            float randomPosX = Random.Range(player.transform.position.x - maxWaterlilyDistance, player.transform.position.x + maxWaterlilyDistance);
            float randomPosY = Random.Range(player.transform.position.y - maxWaterlilyDistance, player.transform.position.y + maxWaterlilyDistance);
            waterliliesPos[i] = new Vector2(randomPosX, randomPosY);
            Debug.Log(waterliliesPos[i]);
            for (int j=0; j < i; j++)
            {
                float distance = Mathf.Sqrt(Mathf.Pow((waterliliesPos[j][0] - waterliliesPos[i][0]), 2) + Mathf.Pow((waterliliesPos[j][1] - waterliliesPos[i][1]), 2)); // Pitagora's theorem
                if (distance < waterlilyRadius)
                {
                    canInstantiateWaterlily = false;
                    j = i;
                    i--;
                }                    
            }
            
            if (canInstantiateWaterlily == true)
            {
                Vector3 WaterlilyPosition = new Vector3(randomPosX, randomPosY, 0);
                Instantiate(waterlily, WaterlilyPosition, waterlily.transform.rotation);
            }
            canInstantiateWaterlily = true;
        }      
    }

    private void Start()
    {
        SpawnWaterlilies();
    }
}
