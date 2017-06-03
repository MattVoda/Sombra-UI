using UnityEngine;
using System.Collections;

public class vertLinePosition_Finder : MonoBehaviour {

    public float offset = 0.2f;

    public Vector3 vertLinePos;
    private Vector3 parentPos;
    private Vector3 parentScale;
     
	// Use this for initialization
	void Start () {

        parentPos = transform.parent.position; //maybe do localPos here?
        parentScale = transform.parent.localScale;

        vertLinePos = new Vector3();

	}
	
	// Update is called once per frame
	void Update () {
        vertLinePos.x = ((parentScale.x / 2) + offset) * -1;
        transform.position = vertLinePos;
    }
}
