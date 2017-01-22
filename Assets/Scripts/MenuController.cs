﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {

	private bool ClickedOne;
	private int count;
	private float TimeClick = 0; 
	public EventSystem eventSystem;
	public GameObject[] buttons;
    
    bool isDeathPanelVisible = false;
	
	// Use this for initialization
	void Start () {
		//eventSystem.SetSelectedGameObject(buttons[0]);
		count = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
	}
	
	// Update is called once per frame
	void Update () {
        if (!isDeathPanelVisible)
            return;

		if (ClickedOne && Time.time-TimeClick>0.15f) {
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
}
