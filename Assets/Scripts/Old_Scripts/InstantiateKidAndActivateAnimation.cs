using UnityEngine;
using System.Collections;

public class InstantiateKidAndActivateAnimation : MonoBehaviour {

    private Animator animator;
    private bool prefabCurrentlyAlive = false;
    private GameObject prefabInScene;

    public GameObject prefabToInstantiateAndAnimate;


    void Awake() {
          
    }

    void Start() {

        /*
        if (prefabToInstantiateAndAnimate) {
            //animator = prefabToInstantiateAndAnimate.GetComponent<Animator>();
        } else {
            Debug.Log("Add prefab to instantiate and animate");
        }
        */
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.tag == "GameController") {
            //instantiate and animate kid
            if(prefabCurrentlyAlive == false) {
                prefabInScene = Instantiate(prefabToInstantiateAndAnimate, transform, true) as GameObject;
                animator = prefabInScene.GetComponent<Animator>();
            }
            animator.SetBool("isHovering", true);
        }
    }
    void OnTriggerExit(Collider other) {
        if (other.gameObject.tag == "GameController") {
            //unanimate
            animator.SetBool("isHovering", false);
            //kid should destroy self after completing anim
        }
    }
}
