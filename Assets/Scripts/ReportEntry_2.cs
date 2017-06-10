using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportEntry_2 : MonoBehaviour {

    private Rigidbody rb;
    float touched = 0.0f;
    float DC = 1.0f;

    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other) {
        touched += DC * Time.deltaTime;
        if (touched >= 1) {
            gameObject.GetComponent<June_Rewrite>().enabled = true;
        }
    }
}
