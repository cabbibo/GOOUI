using UnityEngine;
using System.Collections;

public class ChangeSaturationLightnessWithSliderXY : MonoBehaviour {

  public GameObject saturationLightnessSlider;

  // Use this for initialization
  void Start () {

    saturationLightnessSlider.GetComponent<PressButton>().WhileSliderXYPushed += WhileSliderXYPushed;
  
  }
  
  // Update is called once per frame
  void Update () {
    ///GetComponent<Camera>().backgroundColor =  new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
  }

  void WhileSliderXYPushed( GameObject b , Vector2 value ){

    Color c = Camera.main.GetComponent<Camera>().backgroundColor;
    float h=0.0f; 
    float s=0.0f; 
    float v=0.0f;

    Color.RGBToHSV( c , out h , out s , out v);
    s = value.x;
    v = value.y;


    Camera.main.GetComponent<Camera>().backgroundColor =  Color.HSVToRGB( h,s,v);//new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));//Color.red;
  }
}
