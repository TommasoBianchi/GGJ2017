using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : MonoBehaviour {

    public GameObject waterlily;
    public GameObject powerUpShield;
    public GameObject powerUpSpeed;
    public GameObject powerUpWaterlily;
    public GameObject powerUpWaves; 
    public GameObject player;
    public float maxDistance;
    public float waterlilyRadius;
    public float powerUpRadius;
    public int waterliliesNumber;
    public int powerUpNumber;

    private List<Vector2> waterliliesPos = new List<Vector2>();
    private List<Vector2> powerUpPos = new List<Vector2>();
    private bool canInstantiate = true;


    private void Start()
    {
        SpawnWaterlilies();
        SpawnPowerUp();
    }

    private void SpawnWaterlilies()
    {
        for (int i=0; i < waterliliesNumber; i++)
        {
            waterliliesPos.Add(new Vector2(0, 0));
        }

        for (int i=0; i < waterliliesNumber; i++)
        {
            float randomPosX = Random.Range(player.transform.position.x - maxDistance, player.transform.position.x + maxDistance);
            float randomPosY = Random.Range(player.transform.position.y - maxDistance, player.transform.position.y + maxDistance);
            waterliliesPos[i] = new Vector2(randomPosX, randomPosY);
            for (int j=0; j < i; j++)
            {
                float distance = Mathf.Sqrt(Mathf.Pow((waterliliesPos[j][0] - waterliliesPos[i][0]), 2) + Mathf.Pow((waterliliesPos[j][1] - waterliliesPos[i][1]), 2)); // Pitagora's theorem
                if (distance < waterlilyRadius * 2)
                {
                    canInstantiate = false;
                    j = i;
                    i--;
                }                    
            }
            
            if (canInstantiate == true)
            {
                Vector3 Position = new Vector3(randomPosX, randomPosY, 0);
                Instantiate(waterlily, Position, waterlily.transform.rotation);
            }
            canInstantiate = true;
        }      
    }

    private void SpawnPowerUp()
    {
        for (int i = 0; i < powerUpNumber; i++)
        {
            powerUpPos.Add(new Vector2(0, 0));
        }

        for (int i = 0; i < powerUpNumber; i++)
        {
            float randomPosX = Random.Range(player.transform.position.x - maxDistance, player.transform.position.x + maxDistance);
            float randomPosY = Random.Range(player.transform.position.y - maxDistance, player.transform.position.y + maxDistance);
            powerUpPos[i] = new Vector2(randomPosX, randomPosY);
            for (int j = 0; j < waterliliesNumber; j++) // no sovrapposizione ninfea-powerUp
            {
                float distance = Mathf.Sqrt(Mathf.Pow((waterliliesPos[j][0] - powerUpPos[i][0]), 2) + Mathf.Pow((waterliliesPos[j][1] - powerUpPos[i][1]), 2)); // Pitagora's theorem
                if (distance < waterlilyRadius + powerUpRadius)
                {
                    canInstantiate = false;
                    j = i;
                    i--;
                }
            }

            for (int j = 0; j < i; j++) // no sovrapposizione powerUp-powerUp
            {
                float distance = Mathf.Sqrt(Mathf.Pow((powerUpPos[j][0] - powerUpPos[i][0]), 2) + Mathf.Pow((powerUpPos[j][1] - powerUpPos[i][1]), 2)); // Pitagora's theorem
                if (distance < powerUpRadius * 2)
                {
                    canInstantiate = false;
                    j = i;
                    i--;
                }
            }

            if (canInstantiate == true)
            {
                Vector3 Position = new Vector3(randomPosX, randomPosY, 0);
                int r = Random.Range(0, 4);
                switch (r)
                {
                    case 0:                   
                        Instantiate(powerUpShield, Position, powerUpShield.transform.rotation);
                        break;
                    case 1:
                        Instantiate(powerUpSpeed, Position, powerUpSpeed.transform.rotation);
                        break;
                    case 2:
                        Instantiate(powerUpWaterlily, Position, powerUpWaterlily.transform.rotation);
                        break;
                    case 3:
                        Instantiate(powerUpWaves, Position, powerUpWaves.transform.rotation);
                        break;
                }
            }
            canInstantiate = true;
        }
    }

    public List<Vector2> getWaterliliesPos()
    {
        return waterliliesPos;
    }
}
