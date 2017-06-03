using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleTree : MonoBehaviour {

    private Tree_1 tree1;

	// Use this for initialization
	void Start () {
        tree1 = this.GetComponent<Tree_1>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other) {
        tree1.enabled = true;
        Debug.Log("tree1  ENABLED");
    }

    private void OnTriggerExit(Collider other) {
        tree1.UnSplay();
        //tree1.enabled = false;
        Debug.Log("DISABLED");
    }
}
