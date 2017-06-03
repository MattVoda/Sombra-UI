using UnityEngine;
using System.Collections;

public class riseAndSpinAnimationActivator : MonoBehaviour {

    private Animator animator;



    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "GameController") {
            //instantiate and animate kid
            Debug.Log("ready for some secondary actions!");
            //animator.SetBool("isHovering", true);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "GameController") {
            //unanimate and destroy kid
            //animator.SetBool("isHovering", false);
        }
    }
}
