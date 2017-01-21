using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {

	public float speed;
	public AudioController audioController;
	public GameObject DeathPanel;
	public float turningSpeed;
	public float turningRadius;
	public bool hasShield;
    public GameObject shield;

    public Image powerupIcon;

	private PowerUp currentPowerUp;
	private bool KeyPressed = false;
	private float FirstClick = 0;
	private int count = 0;
    private float probabilityToSpeak = 0;

	// Use this for initialization
	void Start () {
		hasShield = false;
	}
	
	// Update is called once per frame
	void Update () {
		count++;
		if(KeyPressed && Time.time - FirstClick > 0.2f)
			KeyPressed = false;

		gameObject.transform.position += transform.up * speed * Time.deltaTime;
		gameObject.transform.position = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, 0);
		if (!KeyPressed && Input.GetKey("space")) {
			gameObject.transform.RotateAround(transform.position, Vector3.back, (turningRadius * Time.deltaTime));
		}
		if (Input.GetKeyDown ("space")) {
            //gameObject.transform.RotateAround(transform.position, Vector3.back, turningRadius);
            probabilityToSpeak += 0.05f;
			if (!KeyPressed) {
				KeyPressed = true;
				FirstClick = Time.time;
			}
			else {
				if (currentPowerUp != null) {
					currentPowerUp.Activate(this);
                    powerupIcon.GetComponentInParent<Animator>().SetBool("PanelUp", false);
                    currentPowerUp = null;
				}
			}
		}		

        if(Random.value < probabilityToSpeak)
        {
            audioController.Play(AudioController.SfxType.Voice);
            probabilityToSpeak = 0;
        }
	}

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "powerup")
        {
			audioController.Play(AudioController.SfxType.PowerUpPickup);
            currentPowerUp = other.GetComponent<PowerUp>();
            powerupIcon.sprite = currentPowerUp.icon;
            powerupIcon.GetComponentInParent<Animator>().SetBool("PanelUp", true);
            float RandomX = Random.Range(-20000, 20000);
            float RandomY = Random.Range(-20000, 20000);
            other.gameObject.transform.position = new Vector3(RandomX, RandomY, 0);
        }
    }

	public bool WaveTouch () {
        if (hasShield)
        {
            hasShield = false;
            shield.SetActive(false);
            return false;
        }
        else
        {
            audioController.Play(AudioController.SfxType.GameOver);
			DeathPanel.SetActive(true);
            FindObjectOfType<MenuController>().ActivateDeathPanel();         
			speed = 0;
            return true;
        }
	}
}