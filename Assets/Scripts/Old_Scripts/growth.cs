using UnityEngine;
using System.Collections;

public class growth : MonoBehaviour {

    //calling cube provides: growth command, startScale (which also = currentScale at start)

    private GameObject self;

    //public float startScaleFloat = 0.1f;
    public float scaleUpMultiplierFloat = 1.5f;
    public float scaleDownMultiplierFloat = 0.5f;
    private float currentScaleFloat = 1f;
    public float speed = 0.7f;

    public Vector3 originalScaleVector;
    private Vector3 scaleMultiplierVector;
    private Vector3 targetScaleVector;
    private Vector3 maxScaleVector;
    private Vector3 minScaleVector;

    public bool growNow = false;
    public bool shrinkNow = false;

    private int updateCount = 0;

    // Use this for initialization
    void Start () {

        //Debug.Log("I'm born!");

        //self = gameObject;

        //scaleMultiplierVector = new Vector3(scaleUpMultiplierFloat, scaleUpMultiplierFloat, 1);
        /*
        maxScaleVector = originalScaleVector * scaleUpMultiplierFloat;
        minScaleVector = originalScaleVector * scaleDownMultiplierFloat;
        */
        maxScaleVector = Vector3.one * scaleUpMultiplierFloat;
        minScaleVector = Vector3.one * scaleDownMultiplierFloat;

        //Debug.Log("maxScaleVec = " + maxScaleVector);
        //Debug.Log("minScaleVec = " + minScaleVector);
    }
	
	// Update is called once per frame
	void Update () {

        //Debug.Log("Update #" + updateCount);

        //if smaller than original size, hover has left -- DESTROY SELF
        if (transform.localScale.x < originalScaleVector.x) {
            Object.Destroy(gameObject);
        }

        if (growNow == true) {
            Debug.Log("growNow = true");

            targetScaleVector = maxScaleVector;
            Debug.Log(targetScaleVector);
            iTween.ScaleUpdate(self, iTween.Hash("scale", targetScaleVector));
        } else if (shrinkNow == true) {
            Debug.Log("shrinkNow = true");
            targetScaleVector = minScaleVector;
            iTween.ScaleUpdate(self, iTween.Hash("scale", targetScaleVector));
        }

        //update currentScale?
        updateCount++;
    }
}
