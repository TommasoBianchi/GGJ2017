using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerUp : MonoBehaviour {

    public float SpeedBoost;
    public float powerUpTimer;

    private float startingTimer;
    private PlayerController playerController;

    enum PowerUpType { Speed, Shield, Waves, Waterlily };
    PowerUpType powerUpType;


    private void Update ()
    {
        if (Time.time >= startingTimer + powerUpTimer)
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
                break;
            case PowerUpType.Shield:
                player.hasShield = true;
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
