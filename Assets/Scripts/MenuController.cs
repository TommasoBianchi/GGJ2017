using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour {

	private bool ClickedOne;
	private int count;
	private float TimeClick = 0; 
	public EventSystem eventSystem;
	public GameObject[] buttons;
	public GameObject MusicButton, audioSource;
	
	public Sprite[] images = new Sprite[2];
	public bool laUso;

    bool isDeathPanelVisible = false;
	
	// Use this for initialization
	void Start () {
		//eventSystem.SetSelectedGameObject(buttons[0]);
		count = 0;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
		MusicSettings(MusicButton, PlayerPrefs.GetInt("music", 0));
        Debug.Log("Music is " + PlayerPrefs.GetInt("music"));
	}
	
	// Update is called once per frame
	void Update () {
        if (laUso && !isDeathPanelVisible)
            return;

		if (ClickedOne && Time.time-TimeClick>0.2f) {
			eventSystem.SetSelectedGameObject(buttons[count]);
			count++;
			if (count >= buttons.Length)
				count = 0;
			ClickedOne = false;
		}
		if (Input.GetKeyDown ("space")) {
			if (!ClickedOne) {
				TimeClick = Time.time;
				ClickedOne = true;
			}
			else {
				eventSystem.currentSelectedGameObject.GetComponent<Button>().onClick.Invoke();
				Debug.Log("submit");
				ClickedOne = false;
			}
		}		
	}

    public void ActivateDeathPanel()
    {
        isDeathPanelVisible = true;
    }

    public void ReloadCurrentScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

	public void MusicOnOff () {
		MusicButton.GetComponent<Image>().sprite = images[1 - PlayerPrefs.GetInt("music")];
		Debug.Log("Setting music " + (1 - PlayerPrefs.GetInt("music")));
		PlayerPrefs.SetInt("music", 1 - PlayerPrefs.GetInt("music"));
		PlayerPrefs.Save();
		Debug.Log(PlayerPrefs.GetInt("music"));
		if (PlayerPrefs.GetInt("music") == 0)
			audioSource.GetComponent<AudioSource>().mute = false;
		else
			audioSource.GetComponent<AudioSource>().mute = true;
	}
	private void MusicSettings (GameObject musics, int i) {
		musics.GetComponent<Image>().sprite = images[i];
		Debug.Log(i + "Music Settings: " + PlayerPrefs.GetInt("music"));
		if (PlayerPrefs.GetInt("music") == 0)
			audioSource.GetComponent<AudioSource>().mute = false;
		else
			audioSource.GetComponent<AudioSource>().mute = true;
	}

	public void loadScene (int i) {
		Debug.Log("Load scene with music = " + PlayerPrefs.GetInt("music"));
		SceneManager.LoadScene(i);
	}
	public void ExitGame () {
		Application.Quit();
	}
}
