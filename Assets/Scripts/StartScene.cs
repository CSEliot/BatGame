using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class StartScene : MonoBehaviour {

    public GameObject instructionals;

	// Use this for initialization
	void Start () {
        MusicManager._SwitchTo(0);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void StartGame()
    {
        instructionals.SetActive(true);
        Tools.DelayFunction(LoadGamz, 25.00f);
        MusicManager._SwitchTo(1);
    }
    
    private void LoadGamz()
    {
        SceneManager.LoadScene(1);
    }
}
