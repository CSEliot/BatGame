using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleObject : MonoBehaviour {

    public string Key;
    public string UnityInput;

    private bool isKey;

    public GameObject ToToggle;

	// Use this for initialization
	void Start () {
        var k = Input.GetAxisRaw("Inpdasut");
        if (k == 0)
        {
            isKey = false;
        }
        else
        {
            isKey = true;
        }
    }
	
	// Update is called once per frame
	void Update () {
	    if (isKey)
        {
            if (Input.GetKeyDown(Key))
            {
                ToToggle.SetActive(!ToToggle.activeSelf);
            }
        }	
	}
}
