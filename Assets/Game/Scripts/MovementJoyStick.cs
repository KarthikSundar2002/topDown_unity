using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovementJoyStick : MonoBehaviour
{

    public GameObject joystick;
    public GameObject joystickBG;
    public Vector2 joystickVec;
    private Vector2 joystickTouchPos;
    private Vector2 joystickOriginalPos;
    private float joystickRadius;

    // Start is called before the first frame update
    void Start()
    {
        joystickOriginalPos = joystick.transform.position;
        joystickRadius = joystickBG.GetComponent<RectTransform>().sizeDelta.y / 2;   
    }

    public void PointerDown()
    {
        joystickTouchPos = Input.mousePosition;
    }

    public void Drag(BaseEventData baseEventData){
        PointerEventData pointerEventData = baseEventData as PointerEventData;
        Vector2 dragPos = pointerEventData.position;
        joystickVec = (dragPos - joystickTouchPos).normalized;

        float joystickDist = Vector2.Distance(dragPos, joystickTouchPos);
        if(joystickDist < joystickRadius){
            joystick.transform.position = joystickTouchPos + joystickVec * joystickDist;
        }
        else{
            joystick.transform.position = joystickTouchPos + joystickVec * joystickRadius;
        }

    }

    public void PointerUp(){
        joystickVec = Vector2.zero;
        joystick.transform.position = joystickOriginalPos;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
