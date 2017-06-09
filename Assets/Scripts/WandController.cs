using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class WandController : MonoBehaviour
{
    private Valve.VR.EVRButtonId gripButton = Valve.VR.EVRButtonId.k_EButton_Grip;
    private Valve.VR.EVRButtonId triggerButton = Valve.VR.EVRButtonId.k_EButton_SteamVR_Trigger;
    private Valve.VR.EVRButtonId menuButton = Valve.VR.EVRButtonId.k_EButton_ApplicationMenu;

    private SteamVR_TrackedObject trackedObj;
    private SteamVR_Controller.Device controller { get { return SteamVR_Controller.Input((int)trackedObj.index); } }

    //we must track all possible interaction items within the controller's radius
    //using a hashset for rapid lookup
    HashSet<InteractableItem> objectsHoveringOver = new HashSet<InteractableItem>();

    private InteractableItem closestItem;
    private InteractableItem interactingItem;

    //Use this for initialization
    void Start ()
        {
        trackedObj = GetComponent<SteamVR_TrackedObject>();
        }

    //Update called once per frame
    void Update ()
    {
        if (controller == null)
        {
            Debug.Log("Controller not initialized");
            return;
        }

        /*
        if (controller.GetPressDown(gripButton) && pickup != null)
        {
            pickup.transform.parent = this.transform; //setting the cube's parent to be the wand. cube will follow wand.
            pickup.GetComponent<Rigidbody>().isKinematic = true;
        }
        if (controller.GetPressUp(gripButton) && pickup != null)
        {
            pickup.transform.parent = null;
            pickup.GetComponent<Rigidbody>().isKinematic = false;
        }
        */

        if (controller.GetPressDown(gripButton) || controller.GetPressDown(triggerButton)) {
            float minDistance = float.MaxValue;

            float distance;
            foreach (InteractableItem item in objectsHoveringOver) {
                //protecting against a problem where destroyed items still existed in this array
                if (item != null) {
                    distance = (item.transform.position - transform.position).sqrMagnitude;
                    //using sqrMagnitude bc distance can be negative. we just want magnitude

                    if (distance < minDistance) {
                        minDistance = distance;
                        closestItem = item;
                    }
                }
            }

            interactingItem = closestItem;

            if(interactingItem) { //check that there is an item to interact with

                //check that the item isn't already being iteracted with -- if the other controller is holding it
                if (interactingItem.IsInteracting()) {
                    interactingItem.EndInteraction(this);
                }

                interactingItem.BeginInteraction(this);
            }
        }

        if ((controller.GetPressUp(gripButton) || controller.GetPressUp(triggerButton)) && interactingItem != null) {
            interactingItem.EndInteraction(this);
        }

        //restart scene with menu
        if (controller.GetPressDown(menuButton)) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    } //end update

    private void OnTriggerEnter(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem) {  //check if the GameObj has an InteractableItem script -- the ground won't, so ignore
            objectsHoveringOver.Add(collidedItem); //add to hash set
        }
    }

    private void OnTriggerExit(Collider collider)
    {
        InteractableItem collidedItem = collider.GetComponent<InteractableItem>();
        if (collidedItem) {  
            objectsHoveringOver.Remove(collidedItem);
        }
    }

}