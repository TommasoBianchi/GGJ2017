﻿using System.Collections;
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
    private List<GameObject> waterlilies = new List<GameObject>();
    private List<Vector2> powerUpPos = new List<Vector2>();
    private List<GameObject> powerUps = new List<GameObject>();
    private bool canInstantiate = true;
    private float timeToSpawnAWave = 5;
    private WaveController waveController;

    private void Start()
    {
        waveController = FindObjectOfType<WaveController>();

        SpawnWaterlilies();
        SpawnPowerUp();
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
                waterlilies.Add(Instantiate(waterlily, Position, randomRotation));
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

    public List<Vector2> getWaterliliesPos()
    {
        return waterliliesPos;
    }

    private void Update()
    {
        for (int i = 0; i < waterliliesNumber; i++)
        {
            if (player.transform.position.x - waterlilies[i].transform.position.x > maxDistance)
                waterlilies[i].GetComponent<Transform>().position = new Vector3 (waterlilies[i].GetComponent<Transform>().position.x + maxDistance * 2, waterlilies[i].GetComponent<Transform>().position.y, 0);
            if (player.transform.position.x - waterlilies[i].transform.position.x < - maxDistance)
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x - maxDistance * 2, waterlilies[i].GetComponent<Transform>().position.y, 0);
            if (player.transform.position.y - waterlilies[i].transform.position.y > maxDistance)
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x, waterlilies[i].GetComponent<Transform>().position.y + maxDistance * 2, 0);
            if (player.transform.position.y - waterlilies[i].transform.position.y < - maxDistance)
                waterlilies[i].GetComponent<Transform>().position = new Vector3(waterlilies[i].GetComponent<Transform>().position.x, waterlilies[i].GetComponent<Transform>().position.y - maxDistance * 2, 0);
        }

        for (int i = 0; i < powerUpNumber; i++)
        {
            if (player.transform.position.x - powerUps[i].transform.position.x > maxDistance)
                powerUps[i].GetComponent<Transform>().position = new Vector3(player.GetComponent<Transform>().position.x + maxDistance , powerUps[i].GetComponent<Transform>().position.y, 0);
            if (player.transform.position.x - powerUps[i].transform.position.x < -maxDistance)
                powerUps[i].GetComponent<Transform>().position = new Vector3(player.GetComponent<Transform>().position.x - maxDistance , powerUps[i].GetComponent<Transform>().position.y, 0);
            if (player.transform.position.y - powerUps[i].transform.position.y > maxDistance)
                powerUps[i].GetComponent<Transform>().position = new Vector3(powerUps[i].GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y + maxDistance, 0);
            if (player.transform.position.y - powerUps[i].transform.position.y < -maxDistance)
                powerUps[i].GetComponent<Transform>().position = new Vector3(powerUps[i].GetComponent<Transform>().position.x, player.GetComponent<Transform>().position.y - maxDistance, 0);
        }

        if (Time.time > timeToSpawnAWave) 
            SpawnWave();
    }

    void SpawnWave()
    {
        Vector3 offset = Vector3.zero;
        while(offset.sqrMagnitude < 25)
            offset = new Vector3(Random.Range(-20f, 20f), Random.Range(-20f, 20f), 0);
        float waveSpeed = Random.Range(0.5f, 2f);
        float maxRadius = (3f - waveSpeed) * 5;
        waveController.SpawnWave(player.transform.position + offset, waveSpeed, maxRadius);
        timeToSpawnAWave = Time.time + 2;
    }
}
