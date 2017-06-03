using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportEntry : MonoBehaviour {

    public Tree_2 tree;
    private Rigidbody rb;

    public int myIndex;

    public float activationDistance = 3.5f;

    float touched = 0.0f;
    float DC = 1.0f;

	// Use this for initialization
	void Start () {

        rb = GetComponent<Rigidbody>();	

	}
	
	// Update is called once per frame
	void Update () {
		
	}


    private void OnTriggerStay(Collider other) {
        touched += DC * Time.deltaTime;
        //Debug.Log("touch = " + touched);
        if (touched >= 1) {
            tree.ReportEntry(myIndex);
        }
    }


    //private void OnCollisionStay(Collision collision) {
    //    //tree1.
    //    tree1.ReportEntry(myIndex);
    //}

    //private void OnCollisionExit(Collision collision) {
    //    //tree1.exit
    //    //tree1.ReportExit(myIndex);
    //}

}
