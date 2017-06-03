using UnityEngine;
using System.Collections;

public class HoverEffects_Cube : MonoBehaviour {

    public float fadePerSecond = 2.5f;
    private bool isCoroutineRunning = false;

    private Rigidbody rb;
    public float rbToggleDistance = 1f;
    private bool rbActivated = false;

    private Vector3 startPoint;
    private Vector3 updatePoint;
    private Vector3 distanceVector;

    // Use this for initialization
    void Start () {
        rb = GetComponent<Rigidbody>();

        startPoint = transform.position;
        updatePoint = startPoint;
    }
	
	// Update is called once per frame
	void Update () {
        //updatePoint = transform.position;
        //distanceVector = updatePoint - startPoint;

        //if((!rbActivated) && distanceVector.x > rbToggleDistance || distanceVector.y > rbToggleDistance || distanceVector.z > rbToggleDistance) {
        //    rbActivated = true;
        //    rb.isKinematic = false;
        //}
        //if ((rbActivated) && (distanceVector.x < rbToggleDistance && distanceVector.y < rbToggleDistance && distanceVector.z < rbToggleDistance)) {
        //    rbActivated = false;
        //    rb.isKinematic = true;
        //}
    }



    void OnTriggerStay (Collider hand) {
        if( !isCoroutineRunning) {
            isCoroutineRunning = true;
            StartCoroutine(LerpRed(1.0f, 1.0f));
        }
    }

    void OnTriggerExit (Collider hand) {
        isCoroutineRunning = true;
        StartCoroutine(LerpRed(1.0f, 1.0f));
    }

    IEnumerator LerpRed(float aValue, float aTime) {
        Renderer rend = GetComponent<Renderer>();
        float alpha = rend.material.color.a;
        for (float t = 0.0f; t < 1.0f; t += Time.deltaTime / aTime) {
            Color newColor = new Color(Mathf.Lerp(alpha, aValue, t), 0, 0, 1);
            rend.material.color = newColor;
            yield return null;
        }
        isCoroutineRunning = false;
    }

    /*
        IEnumerator FadeTo() {
        Material material = GetComponent<Renderer>().material;
        Color color = material.color;
        material.color = new Color(color.r, color.g, color.b, color.a + (fadePerSecond * Time.deltaTime));
    }*/

    //void OnTriggerEnter (Collider hand) {
    //    Debug.Log("hand enter");

    //    if (!handIn) {
    //        //fade up to full opacity
    //        StartCoroutine(LerpRed(1.0f, 1.0f));

    //    }
    //    handIn = true;
    //}
}
