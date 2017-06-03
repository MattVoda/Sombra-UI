using UnityEngine;
using System.Collections;

public class growAnimationActivator : MonoBehaviour {

	private Animator animator;

    void Awake() {
        animator = GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "GameController") {
            animator.SetBool("isHovering", true);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "GameController") {
            animator.SetBool("isHovering", false);
        }
    }
}
