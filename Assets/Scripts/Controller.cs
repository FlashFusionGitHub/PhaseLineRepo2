using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

/*Referencing InControl in multiple scripts was causing problems,
this class fixed most issues*/

public class Controller : MonoBehaviour {

    InputDevice m_controller;

    public int m_playerIndex;

    private void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    // Update is called once per frame
    void Update() {
        try {
            m_controller = InputManager.Devices[m_playerIndex];
        }
        catch {
            Debug.Log("Player " + m_playerIndex + " controller not working");
            return;
        }
    }

    //ACTIONS
    public bool Action1WasPress() { return m_controller.Action1.WasPressed; }

    public bool Action2WasPress() { return m_controller.Action2.WasPressed; }

    public bool Action3WasPress() { return m_controller.Action3.WasPressed; }

    public bool Action4WasPress() { return m_controller.Action4.WasPressed; }

    //DPAD
    public bool DpadUpWasPress() { return m_controller.DPadUp.WasPressed; }

    public bool DpadDownWasPress() { return m_controller.DPadDown.WasPressed; }

    public bool DpadLeftWasPress() { return m_controller.DPadLeft.WasPressed; }

    public bool DpadRightWasPress() { return m_controller.DPadRight.WasPressed; }

    //BUMPERS
    public bool LeftBumperWasPressed() { return m_controller.LeftBumper.WasPressed; }

    public bool RightBumperWasPressed() { return m_controller.RightBumper.WasPressed; }

    //BUMPERS
    public bool LeftBumperIsHeld() { return m_controller.LeftBumper.IsPressed; }

    public bool RightBumperIsHeld() { return m_controller.RightBumper.IsPressed; }

    //TRIGGERS
    public bool LeftTriggerWasPress() { return m_controller.LeftTrigger.WasPressed; }

    public bool RightTriggerWasPress() { return m_controller.RightTrigger.WasPressed; }

    public bool LeftTriggerIsHeld() { return m_controller.LeftTrigger.IsPressed; }

    public bool RightTriggerIsHeld() { return m_controller.RightTrigger.IsPressed; }

    //MENU
    public bool MenuWasPress() { return m_controller.MenuWasPressed; }

    //ANALOG STICKS
    public TwoAxisInputControl LeftAnalogStick() { return m_controller.LeftStick; }

    public TwoAxisInputControl RightAnalogStick() { return m_controller.RightStick; }

    //ANALOG STICK BUTTONS
    public bool RightStickButton() { return m_controller.RightStickButton.WasPressed; }

    public bool LeftStickButton() { return m_controller.LeftStickButton.WasPressed; }
}
