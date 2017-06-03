using UnityEngine;
using System.Collections;

public class OpenClosePanelTest : MonoBehaviour 
    {
    // Fields
    private IAnimationToken animToken;
     
    // Methods
     
    public void Open()
    {
        animToken.SafeCancel();
         
        Vector2 startPos = transform.localPosition;
         
        animToken = AnimationHelper.Animate(Time.time, 1f, (t) => {
            transform.localPosition = Vector2.Lerp(startPos, new Vector2(100f, 0f), EasingFunctions.easeInOut(t));
        });
    }
     
    public void Close()
    {
        animToken.SafeCancel();
         
        Vector2 startPos = transform.localPosition;
         
        animToken = AnimationHelper.Animate(Time.time, 1f, (t) => {
            transform.localPosition = Vector2.Lerp(startPos, Vector2.zero, EasingFunctions.easeInOut(t));
        });
    }
}
