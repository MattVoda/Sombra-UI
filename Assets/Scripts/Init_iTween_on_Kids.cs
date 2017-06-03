using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Init_iTween_on_Kids : MonoBehaviour {


    private void Awake() {
        
    }

    // Use this for initialization
    void Start () {
        iTween.Init(this.gameObject);
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
