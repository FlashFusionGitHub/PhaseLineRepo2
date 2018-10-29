using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AdjustSlider : MonoBehaviour {

    //reference to the buttons, we do this so we can disable them
    public Button increaseButton;
    public Button decreaseButton;

    //the amount of increase per click
    float increasePerClick = 0.1f;

    //from the click event, call this function, (true) to increase (false) to decrease
    public void AdjustValue(bool increment)
    {
        //clamp the slider between its min and max value
        GetComponent<Slider>().value = Mathf.Clamp(GetComponent<Slider>().value + (increment ? increasePerClick : -increasePerClick), GetComponent<Slider>().minValue, GetComponent<Slider>().maxValue);

        //disable the button if it reaches its min max value
        increaseButton.interactable = GetComponent<Slider>().value < GetComponent<Slider>().maxValue;
        decreaseButton.interactable = GetComponent<Slider>().value > GetComponent<Slider>().minValue;
    }
}
