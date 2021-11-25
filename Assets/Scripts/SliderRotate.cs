using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class SliderRotate : MonoBehaviour
{
    // Assign in the inspector
    public GameObject objectToRotate;
    public Slider slider;
 
    // Preserve the original and current orientation
    private float previousValue;
 
    void Awake ()
    {
        // Assign a callback for when this slider changes
        this.slider.onValueChanged.AddListener (this.OnSliderChanged);
 
        // And current value
        this.previousValue = this.slider.value;
    }
 
    void OnSliderChanged (float value)
    {
        objectToRotate.transform.Rotate (Vector3.up, slider.value, Space.World);
 
        // Set our previous value for the next change
        this.previousValue = value;
    }
}
