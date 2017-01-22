using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GameController : MonoBehaviour {

    public GameObject[] obstaclesPrefabs;
    public GameObject[] IAPrefabs;
    public GameObject GameTutorial;
    public GameObject rockPrefab;
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
    public int IANumber;

    private List<Vector2> waterliliesPos = new List<Vector2>();
    private List<GameObject> waterlilies = new List<GameObject>();
    private List<Vector2> powerUpPos = new List<Vector2>();
    private List<GameObject> powerUps = new List<GameObject>();
    private List<IA> IAObjects = new List<IA>();
    private bool canInstantiate = true;
    private float timeToSpawnAWave = 2;
    private WaveController waveController;
    private bool Go = false;


    private void Start()
    {
        waveController = FindObjectOfType<WaveController>();
    }

    int frameCount = 0;
    private void StartGame()
    {
        switch (frameCount)
        {
            case 0:
                SpawnWaterlilies();
                break;
            case 1:
                SpawnPowerUp();
                break;
            case 2:
                SpawnIA();
                break;
        }

        frameCount++;
    }

    private void SpawnWaterlilies()
    {
        GameObject waterliliesGroup = new GameObject("waterliliesGroup");
        
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
                float distance = Mathf.Sqrt(Mathf.Pow((waterliliesPos[j].x - waterliliesPos[i].x), 2) + Mathf.Pow((waterliliesPos[j].y - waterliliesPos[i].y), 2)); // Pitagora's theorem
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
                Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
                waterlilies.Add(Instantiate(obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length)], Position, randomRotation));
                waterlilies[waterlilies.Count - 1].transform.localScale = Vector3.one * Random.Range(3f, 6f);
                waterlilies[waterlilies.Count - 1].GetComponent<Animator>().SetFloat("Speed", Random.Range(0.2f, 2f));
            }
            canInstantiate = true;
        }
        for (int i = 0; i < waterliliesNumber; i++)
        {
            waterlilies[i].transform.parent = waterliliesGroup.transform;
        }
    }

    private void SpawnPowerUp()
    {
        GameObject powerUpGroup = new GameObject("powerUpGroup");

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
                float distance = Mathf.Sqrt(Mathf.Pow((waterliliesPos[j].x - powerUpPos[i].x), 2) + Mathf.Pow((waterliliesPos[j].y - powerUpPos[i].y), 2)); // Pitagora's theorem
                if (distance < waterlilyRadius + powerUpRadius)
                {
                    canInstantiate = false;
                    break;
                }
            }

            for (int j = 0; j < i && canInstantiate; j++) // no sovrapposizione powerUp-powerUp
            {
                float distance = Mathf.Sqrt(Mathf.Pow((powerUpPos[j].x - powerUpPos[i].x), 2) + Mathf.Pow((powerUpPos[j].y - powerUpPos[i].y), 2)); // Pitagora's theorem
                if (distance < powerUpRadius * 2)
                {
                    canInstantiate = false;
                    j = i;
                }
            }

            if (canInstantiate == true)
            {
                Vector3 Position = new Vector3(randomPosX, randomPosY, 0);
                int r = Random.Range(0, 4);
                switch (r)
                {
                    case 0:
                        powerUps.Add(Instantiate(powerUpShield, Position, powerUpShield.transform.rotation));
                        break;
                    case 1:
                        powerUps.Add(Instantiate(powerUpSpeed, Position, powerUpSpeed.transform.rotation));
                        break;
                    case 2:
                        powerUps.Add(Instantiate(powerUpWaterlily, Position, powerUpWaterlily.transform.rotation));
                        break;
                    case 3:
                        powerUps.Add(Instantiate(powerUpWaves, Position, powerUpWaves.transform.rotation));
                        break;
                }
                powerUps[powerUps.Count - 1].GetComponent<Animator>().SetBool("Active", true);
            }
            else
                i--;
            canInstantiate = true;           
        }

        for (int i = 0; i < powerUpNumber; i++)
        {
            powerUps[i].transform.parent = powerUpGroup.transform;
        }
    }

    private void SpawnIA()
    {
        GameObject IAGroup = new GameObject("IAGroup");

        for (int i = 0; i < IANumber; i++)
        {
            float randomPosX = Random.Range(player.transform.position.x - maxDistance / 2, player.transform.position.x + maxDistance / 2);
            float randomPosY = Random.Range(player.transform.position.y - maxDistance / 2, player.transform.position.y + maxDistance / 2);
            Vector3 spawnPosition = new Vector3(randomPosX, randomPosY, 0);
            bool canSpawn = false;
            float epsilon = 0.4f;

            do
            {
                canSpawn = true;
                for (int j = 0; j < waterlilies.Count; j++)
                {
                    if((waterlilies[j].transform.position - spawnPosition).sqrMagnitude < epsilon * epsilon)
                    {
                        canSpawn = false;
                        break;
                    }
                }
                if (!canSpawn)
                {
                    randomPosX = Random.Range(player.transform.position.x - maxDistance / 2, player.transform.position.x + maxDistance / 2);
                    randomPosY = Random.Range(player.transform.position.y - maxDistance / 2, player.transform.position.y + maxDistance / 2);
                    spawnPosition = new Vector3(randomPosX, randomPosY, 0);
                }
            } while (!canSpawn);

            Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
            GameObject newIA = Instantiate(IAPrefabs[Random.Range(0, IAPrefabs.Length)], spawnPosition, randomRotation) as GameObject;
            newIA.transform.SetParent(IAGroup.transform);
            IAObjects.Add(newIA.GetComponent<IA>());
        }
    } 

    public GameObject[] getWaterlilies()
    {
        return waterlilies.ToArray();
    }

    public IA[] getIAObjects()
    {
        return IAObjects.ToArray();
    }

    private void Update()
    {
        if (Input.GetKeyDown("space")) {
            Go = true;
            GameTutorial.SetActive(false);
            StartGame();
        }
        if (!Go)
            return;
        for (int i = 0; i < waterlilies.Count; i++)
        {
            if (player.transform.position.x - waterlilies[i].transform.position.x > maxDistance)
            {
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x + maxDistance * 2, waterlilies[i].GetComponent<Transform>().position.y, 0);
                waterliliesPos[i] = new Vector2(waterliliesPos[i].x + maxDistance * 2, waterliliesPos[i].y);
            }

            if (player.transform.position.x - waterlilies[i].transform.position.x < -maxDistance)
            {
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x - maxDistance * 2, waterlilies[i].GetComponent<Transform>().position.y, 0);
                waterliliesPos[i] = new Vector2(waterliliesPos[i].x - maxDistance * 2, waterliliesPos[i].y);
            }
            if (player.transform.position.y - waterlilies[i].transform.position.y > maxDistance)
            {
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x, waterlilies[i].GetComponent<Transform>().position.y + maxDistance * 2, 0);
                waterliliesPos[i] = new Vector2(waterliliesPos[i].x, waterliliesPos[i].y + maxDistance * 2);
            }
            if (player.transform.position.y - waterlilies[i].transform.position.y < -maxDistance)
            {
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x, waterlilies[i].GetComponent<Transform>().position.y - maxDistance * 2, 0);
                waterliliesPos[i] = new Vector2(waterliliesPos[i].x, waterliliesPos[i].y - maxDistance * 2);
            }

            waterlilies[i].GetComponent<Collider2D>().enabled = waterlilies[i].GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).IsTag("Idle");
        }

        for (int i = 0; i < powerUps.Count; i++)
        {
            if (player.transform.position.x - powerUps[i].transform.position.x > maxDistance)
            {
                powerUps[i].GetComponent<Transform>().position = new Vector3(powerUps[i].GetComponent<Transform>().position.x + maxDistance * 2, powerUps[i].GetComponent<Transform>().position.y, 0);
                powerUpPos[i] = new Vector2(powerUps[i].GetComponent<Transform>().position.x + maxDistance * 2, powerUps[i].GetComponent<Transform>().position.y);
            }
            if (player.transform.position.x - powerUps[i].transform.position.x < -maxDistance)
            {
                powerUps[i].GetComponent<Transform>().position = new Vector3(powerUps[i].GetComponent<Transform>().position.x - maxDistance * 2, powerUps[i].GetComponent<Transform>().position.y, 0);
                powerUpPos[i] = new Vector2(powerUps[i].GetComponent<Transform>().position.x - maxDistance * 2, powerUps[i].GetComponent<Transform>().position.y);
            }                
            if (player.transform.position.y - powerUps[i].transform.position.y > maxDistance)
            {
                powerUps[i].GetComponent<Transform>().position = new Vector3(powerUps[i].GetComponent<Transform>().position.x, powerUps[i].GetComponent<Transform>().position.y + maxDistance * 2, 0);
                powerUpPos[i] = new Vector2(powerUps[i].GetComponent<Transform>().position.x, powerUps[i].GetComponent<Transform>().position.y + maxDistance * 2);
            }               
            if (player.transform.position.y - powerUps[i].transform.position.y < -maxDistance)
            {
                powerUps[i].GetComponent<Transform>().position = new Vector3(powerUps[i].GetComponent<Transform>().position.x, powerUps[i].GetComponent<Transform>().position.y - maxDistance * 2, 0);
                powerUpPos[i] = new Vector2(powerUps[i].GetComponent<Transform>().position.x, powerUps[i].GetComponent<Transform>().position.y - maxDistance * 2);
            }
                
        }

        if (Time.time > timeToSpawnAWave) 
            SpawnWave();
    }

    void SpawnWave()
    {
        Vector3 offset = Vector3.zero;
        while(offset.sqrMagnitude < 25)
            offset = new Vector3(Random.Range(-10f, 10f), Random.Range(-10f, 10f), 0);
        Vector3 direction = player.transform.up * 30;
        Vector3 spawnPosition = player.transform.position + offset + direction;
        Vector3 secondSpawnPosition = new Vector3(Random.Range(-50f, 50f), Random.Range(-50f, 50f), 0);

        for (int i = 0; i < waterliliesPos.Count; i++)
        {
            if (((Vector3)waterliliesPos[i] - spawnPosition).sqrMagnitude < 0.5f)
                return;
        }

        float waveSpeed = Random.Range(0.5f, 2f);
        float maxRadius = (3f - waveSpeed) * 5;
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        GameObject rock = Instantiate(rockPrefab, spawnPosition, randomRotation) as GameObject;
        Animator rockAnimator = rock.GetComponent<Animator>();

        waveController.SpawnWave(spawnPosition, waveSpeed, maxRadius,
            canSimulate: () => rockAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f);
        //waveController.SpawnWave(secondSpawnPosition, waveSpeed, maxRadius,
        //    canSimulate: () => rockAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime > 0.7f);
        timeToSpawnAWave = Time.time + Random.Range(0.05f, 0.8f);
    }
    
    public void SpawnWaterlily(Vector3 pos)
    {
        Quaternion randomRotation = Quaternion.Euler(0, 0, Random.Range(0, 360f));
        GameObject waterlilyObj = Instantiate(obstaclesPrefabs[Random.Range(0, obstaclesPrefabs.Length)], player.transform.position, randomRotation) as GameObject;
        waterlilies.Add(waterlilyObj);
        waterliliesPos.Add(waterlilyObj.transform.position);        
    }
}
