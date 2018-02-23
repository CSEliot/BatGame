using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartScene : MonoBehaviour {

    public GameObject instructionals;

    private float timeSpan_readingInstructions = 25f;
    private float previousTimeCheck = 0;
    private float oneSecond = 1f;
    private bool readingInstructions = false;
    private float time_DoneReadingInstructions;

	// Use this for initialization
	void Start () {
        previousTimeCheck = Time.time;
        MusicManager._SwitchTo(0);
    }

    // Update is called once per frame
    void Update()
    {
        if(readingInstructions) {
            if (Time.time - previousTimeCheck > oneSecond)
            {
                previousTimeCheck = Time.time;
                int time_RemainingTime = Convert.ToInt32(time_DoneReadingInstructions - Time.time);
                GameObject.Find("TimeLeftUI").GetComponent<Text>().text = "" + time_RemainingTime;
                if (time_RemainingTime < 1 || Input.GetKey(KeyCode.L))
                {
                    LoadGamz();
                }
            }
        }
    }
    public void StartGame()
    {
        //time_DoneReadingInstructions = Time.time + timeSpan_readingInstructions;
        //readingInstructions = true;
        instructionals.SetActive(true);
        MusicManager._SwitchTo(1);
    }
    
    public void LoadGamz()
    {
        SceneManager.LoadScene(1);
    }
}
