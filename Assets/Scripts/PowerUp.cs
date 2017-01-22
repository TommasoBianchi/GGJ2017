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
    private static GameController gameController;

    void Start()
    {
        if (waveController == null)
            waveController = FindObjectOfType<WaveController>();
        if (gameController == null)
            gameController = FindObjectOfType<GameController>();
    }


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
                player.audioController.Play(AudioController.SfxType.Sprint);
                break;
            case PowerUpType.Shield:
                player.hasShield = true;
                Used = true;
                player.shield.SetActive(true);
                player.shield.GetComponent<Animator>().SetBool("ShieldActive", true);
                player.audioController.Play(AudioController.SfxType.Bubble);
                break;
            case PowerUpType.Waves:
                waveController.SpawnWave(player.transform.position + 0.1f * player.speed * player.transform.up, 12, 15, 1.5f);
                player.audioController.Play(AudioController.SfxType.Wave);
                break;
            case PowerUpType.Waterlily:
                gameController.SpawnWaterlily(player.transform.position);
                player.audioController.Play(AudioController.SfxType.Flower);
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
                //player.shield.SetActive(false);
                player.shield.GetComponent<Animator>().SetBool("ShieldActive", false);
                break;
            case PowerUpType.Waves:
                break;
            case PowerUpType.Waterlily:
                break;
        }

        float RandomX = Random.Range(-gameController.maxDistance, gameController.maxDistance);
        float RandomY = Random.Range(-gameController.maxDistance, gameController.maxDistance);
        transform.position = new Vector3(RandomX, RandomY, 0);
        GetComponent<Animator>().SetBool("Active", true);
    }
}
