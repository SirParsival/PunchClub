using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ActionDPad : UIBehaviour, IBeginDragHandler, IEndDragHandler, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    public enum ActionPadDirection
    {
        Up = 1,
        UpRight = 2,
        Right = 3,
        DownRight,
        Down,
        DownLeft,
        Left,
        UpLeft,
        None = 999
    };

    [SerializeField]
    float radius = 1;

    [HideInInspector]
    bool isHeld;

    [SerializeField]
    Sprite[] directionalSprites;

    [Serializable]
    public class JoystickMoveEvent : UnityEvent<ActionPadDirection> { }
    public JoystickMoveEvent OnValueChange;

    private ActionPadDirection UpdateTouchSprite(Vector2 direction)
    {
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;

        if (angle < 0)
        {
            angle += 360;
        }

        ActionPadDirection currentPadDirection = ActionPadDirection.None;
        if (angle <= 22.5f || angle > 337.5f)
        {
            currentPadDirection = ActionPadDirection.Up;
        }
            
        else if (angle > 22.5 && angle <= 67.5)
        {
            currentPadDirection = ActionPadDirection.UpRight;
        }
           
        else if (angle > 67.5 && angle <= 112.5)
        {
            currentPadDirection = ActionPadDirection.Right;
        }
            
        else if (angle > 112.5 && angle <= 157.5)
        {
            currentPadDirection = ActionPadDirection.DownRight;
        }
            
        else if (angle > 157.5 && angle <= 202.5)
        {
            currentPadDirection = ActionPadDirection.Down;
        }
            
        else if (angle > 202.5 && angle <= 247.5)
        {
            currentPadDirection = ActionPadDirection.DownLeft;
        }
            
        else if (angle > 247.5 && angle <= 292.5)
        {
            currentPadDirection = ActionPadDirection.Left;
        }
           
        else if (angle > 292.5 && angle <= 337.5)
        {
            currentPadDirection = ActionPadDirection.UpLeft;
        }
           

        int index = 0;
        if (currentPadDirection != ActionPadDirection.None)
        {
            index = (int)currentPadDirection;
        }

        GetComponent<Image>().sprite = directionalSprites[index];
        return currentPadDirection;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (!IsActive())
        {
            return;
        }

        RectTransform thisRect = transform as RectTransform;
        Vector2 touchDir;
        bool didConvert = RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, 
            eventData.enterEventCamera, out touchDir);

        if (touchDir.sqrMagnitude > radius * radius)
        {
            touchDir.Normalize();
            isHeld = true;
            ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
            OnValueChange.Invoke(currentDirection);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        OnValueChange.Invoke(ActionPadDirection.None);
        GetComponent<Image>().sprite = directionalSprites[0];
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (isHeld)
        {
            RectTransform thisRect = transform as RectTransform;
            Vector2 touchDir;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
            touchDir.Normalize();

            ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
            OnValueChange.Invoke(currentDirection);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        RectTransform thisRect = transform as RectTransform;
        Vector2 touchDir;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(thisRect, eventData.position, eventData.enterEventCamera, out touchDir);
        touchDir.Normalize();

        ActionPadDirection currentDirection = UpdateTouchSprite(touchDir);
        OnValueChange.Invoke(currentDirection);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnValueChange.Invoke(ActionPadDirection.None);
        GetComponent<Image>().sprite = directionalSprites[0];
    }
}
