using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public float SpeedBoost;
    public float powerUpTimer;
    public Sprite icon;

    private float startingTimer;
    private PlayerController playerController;

    public enum PowerUpType { Speed, Shield, Waves, Waterlily };
    public PowerUpType powerUpType;
    private bool Used = false;

    private static WaveController waveController;

    void Start()
    {
        if (waveController == null)
            waveController = FindObjectOfType<WaveController>();
    }


    private void Update ()
    {
        if (Time.time >= startingTimer + powerUpTimer && Used)
            Deactivate(playerController);
    }

    public void Activate(PlayerController player)
    {
        Debug.Log(powerUpType);
        playerController = player;
        startingTimer = Time.time;
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                player.speed += SpeedBoost;
                Used = true;
                break;
            case PowerUpType.Shield:
                player.hasShield = true;
                Used = true;
                break;
            case PowerUpType.Waves:
                waveController.SpawnWave(player.transform.position + 0.1f * player.speed * player.transform.up, 12, 15, 1.5f);
                break;
            case PowerUpType.Waterlily:
                break;
        }
    }

    private void Deactivate(PlayerController player)
    {
        Used = false;
        switch (powerUpType)
        {
            case PowerUpType.Speed:
                player.speed -= SpeedBoost;
                break;
            case PowerUpType.Shield:
                player.hasShield = false;
                break;
            case PowerUpType.Waves:
                break;
            case PowerUpType.Waterlily:
                break;
        }        
    }
}
