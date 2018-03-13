using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowURL : MonoBehaviour {

    //https://drive.google.com/open?id=12twN47wYobGx-G_Z7eiZhMfrPHaIfjb6
    public string URL;
    
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void GoToURL() {
        Application.OpenURL("https://drive.google.com/open?id=12twN47wYobGx-G_Z7eiZhMfrPHaIfjb6");
    }
}
