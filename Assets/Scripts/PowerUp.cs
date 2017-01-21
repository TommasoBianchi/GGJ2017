using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public float SpeedBoost;
    public float powerUpTimer;

    private float startingTimer;
    private PlayerController playerController;

    public enum PowerUpType { Speed, Shield, Waves, Waterlily };
    public PowerUpType powerUpType;
    private bool Used = false;


    private void Update ()
    {
        if (Time.time >= startingTimer + powerUpTimer && Used)
            Deactivate(playerController);
    }

    public void Activate(PlayerController player)
    {
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
                break;
            case PowerUpType.Waterlily:
                break;
        }
    }

    private void Deactivate(PlayerController player)
    {
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
        Destroy(gameObject);
    }
}
