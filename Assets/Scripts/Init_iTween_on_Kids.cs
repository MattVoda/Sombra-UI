using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Init_iTween_on_Kids : MonoBehaviour {


    private void Awake() {
        iTween.Init(this.gameObject);
    }

    // Use this for initialization
    void Start () {
        
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
