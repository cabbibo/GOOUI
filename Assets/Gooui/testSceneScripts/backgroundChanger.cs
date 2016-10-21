using UnityEngine;
using System.Collections;

public class backgroundChanger : MonoBehaviour {

  public GameObject bgButton;

	// Use this for initialization
	void Start () {

    bgButton.GetComponent<PressButton>().OnButtonFire += OnButtonFire;
	
	}
	
	// Update is called once per frame
	void Update () {
    ///GetComponent<Camera>().backgroundColor =  new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));
	}

  void OnButtonFire( GameObject b ){

    GetComponent<Camera>().backgroundColor =  new Color(Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f),Random.Range(0.0f,1.0f));//Color.red;
  }
}
