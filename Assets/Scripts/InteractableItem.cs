using UnityEngine;
using System.Collections;


public class InteractableItem : MonoBehaviour {

    private bool isScaled = false;

    public Rigidbody rigidbody;
    private bool currentlyInteracting;

    private float velocityFactor = 20000f;
    private Vector3 posDelta;

    private Quaternion rotationDelta;
    private float rotationFactor = 400f;
    private float angle;
    private Vector3 axis;

    private WandController attachedWand;

    private Transform interactionPoint;

	// Use this for initialization
	void Start () {
        rigidbody = GetComponent<Rigidbody>();
        interactionPoint = new GameObject().transform; //an empty object's transform
        velocityFactor /= rigidbody.mass;  //realistic -- bigger mass = smaller velocity factor
	}
	
	// Update is called once per frame
	void Update () {
	    //update velocity of obj -- make it do stuff!
        if (attachedWand && currentlyInteracting) {
            posDelta = attachedWand.transform.position - interactionPoint.position;
            this.rigidbody.velocity = posDelta * velocityFactor * Time.fixedDeltaTime;

            rotationDelta = attachedWand.transform.rotation * Quaternion.Inverse(interactionPoint.rotation);
            rotationDelta.ToAngleAxis(out angle, out axis);

            if (angle > 180) { //if you have a big rotation to make, take the path of least resistance
                angle -= 360;
            }

            this.rigidbody.angularVelocity = (Time.fixedDeltaTime * angle * axis) * rotationFactor;
        }
	}

    public void BeginInteraction(WandController wand) {
        attachedWand = wand;
        interactionPoint.position = wand.transform.position;
        interactionPoint.rotation = wand.transform.rotation;
        interactionPoint.SetParent(transform, true);

        currentlyInteracting = true;
    }

    public void EndInteraction(WandController wand) {
        if (wand == attachedWand) {
            attachedWand = null;
            currentlyInteracting = false;
        }
    }

    public bool IsInteracting() {
        return currentlyInteracting;
    }
}
