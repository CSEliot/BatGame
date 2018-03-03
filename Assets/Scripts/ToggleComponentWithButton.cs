using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ToggleComponentWithButton : MonoBehaviour {

    private Component[] components;

    public string ToggleButton;

	// Use this for initialization
	void Start () {
        components = gameObject.GetComponents<Component>();
	}
	
	// Update is called once per frame
	void Update () {
	    	
	}


}
