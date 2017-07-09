using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReportEntry_2 : MonoBehaviour {

    private Rigidbody rb;
    private June_Rewrite_Child jrc;
    private June_Rewrite jr;

    float touched = 0.0f;
    float DC = 1.0f;

    public int myIndexPositionInParent = -1;

    private void Awake() {
        jrc = gameObject.GetComponent<June_Rewrite_Child>();
        jr = gameObject.GetComponentInParent<June_Rewrite>();
    }

    void Start () {
        rb = GetComponent<Rigidbody>();
    }

    private void OnTriggerStay(Collider other) {
        touched += DC * Time.deltaTime;
        if (touched >= 1) {
            jrc.enabled = true;
            if (myIndexPositionInParent != -1) {
                jr.EntryReportedByChild(myIndexPositionInParent);
            }
        }
    }
}
