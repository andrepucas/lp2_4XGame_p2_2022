// This class is completely authored by the user: https://forum.unity.com/members/kmeboe.119040/
// From a unity forum reply to the post: https://forum.unity.com/threads/scroll-view-does-not-scroll-with-mousewheel-when-mouse-is-over-a-button-inside-the-scroll-view.848848/
// Its being used as an extension of the TMP UI element Scroll Rect, to fix a a mouse scroll wheel bug.
// If the mouse was over the widget's input fields, the mouse scroll wouldn't work.
// This problem only happened with widgets that were instantiated on runtime.

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UpgradedScrollRect : ScrollRect, IPointerEnterHandler, IPointerExitHandler
{
    private static string mouseScrollWheelAxis = "Mouse ScrollWheel";
    private bool swallowMouseWheelScrolls = true;
    private bool isMouseOver = false;
 
    public void OnPointerEnter(PointerEventData eventData)
    {
        isMouseOver = true;
    }
 
    public void OnPointerExit(PointerEventData eventData)
    {
        isMouseOver = false;
    }
 
    private void Update()
    {
        // Detect the mouse wheel and generate a scroll. This fixes the issue where Unity will prevent our ScrollRect
        // from receiving any mouse wheel messages if the mouse is over a raycast target (such as a button).
        if (isMouseOver && IsMouseWheelRolling())
        {
            var delta = UnityEngine.Input.GetAxis(mouseScrollWheelAxis);
 
            PointerEventData pointerData = new PointerEventData(EventSystem.current);
            pointerData.scrollDelta = new Vector2(0f, delta);
 
            swallowMouseWheelScrolls = false;
            OnScroll(pointerData);
            swallowMouseWheelScrolls = true;
        }
    }
 
    public override void OnScroll(PointerEventData data)
    {
        if (IsMouseWheelRolling() && swallowMouseWheelScrolls)
        {
            // Eat the scroll so that we don't get a double scroll when the mouse is over an image
        }
        else
        {
            // Amplify the mousewheel so that it matches the scroll sensitivity.
            if (data.scrollDelta.y < -Mathf.Epsilon)
                data.scrollDelta = new Vector2(0f, -scrollSensitivity);
            else if (data.scrollDelta.y > Mathf.Epsilon)
                data.scrollDelta = new Vector2(0f, scrollSensitivity);
 
            base.OnScroll(data);
        }
    }
 
    private static bool IsMouseWheelRolling()
    {
        return UnityEngine.Input.GetAxis(mouseScrollWheelAxis) != 0;
    }
}
