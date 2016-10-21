using UnityEngine;
using System.Collections;

public class PressButton : MonoBehaviour {

  // creating an event to be able to use more fluidly!
  public delegate void ButtonFire(GameObject t);
  public event ButtonFire OnButtonFire;

  public delegate void ButtonRelease(GameObject t);
  public event ButtonRelease OnButtonRelease;

  public delegate void ButtonToggleOn(GameObject t);
  public event ButtonToggleOn OnButtonToggleOn;

  public delegate void ButtonToggleOff(GameObject t);
  public event ButtonToggleOff OnButtonToggleOff;


  public delegate void ButtonPushed( GameObject t , bool toggled );
  public event ButtonPushed WhileButtonPushed;

  public delegate void SliderPushed( GameObject t , float value );
  public event SliderPushed WhileSliderPushed;

  public delegate void SliderXYPushed( GameObject t , Vector2 value );
  public event SliderXYPushed WhileSliderXYPushed;

  public GameObject HandL;
  public GameObject HandR;

  public AudioClip hoverOverNote;
  public AudioClip hoverOverLoop;
  public AudioClip pressNote;
  public AudioClip pressLoop;

  private AudioSource s_hoverOverNote;
  private AudioSource s_hoverOverLoop;
  private AudioSource s_pressNote;
  private AudioSource s_pressLoop;

  // Number of verts in X & Y
  public int resolutionX;
  public int resolutionY;

  // Width and Height of button
  public float width;
  public float height;

  public float pushDistance;

  private MeshFilter mesh;

  public float triggerVal;
  public float downVal = 0;
  public float toggleVal = 0;

  public bool hovering;
  public bool pressing;
  public bool toggled = false;

  //public float Value;

  string Title;

  public float ValueY;
  public float ValueX;
  public Vector2 ValueXY;

  public float SliderValue;

  private GameObject enteredObject;

  private bool inside = false;



  public void setButtonTexture( Texture texture ){
    GetComponent<Renderer>().material.SetTexture("_ButtonTexture", texture );
  }

  // Use this for initialization
  void Awake () {

    ValueY = 0;
    ValueX = 0;
    ValueXY = new Vector2( 0 , 0);

    CreateMesh();

    // Make our collider so it fits on the button properly
    BoxCollider b = GetComponent<BoxCollider>();
    b.center = new Vector3( 0 , pushDistance * .5f , 0 );
    b.size = new Vector3( width , pushDistance , height );
  

    // Make a base to the button so we know how far to press
    GetComponent<LineRenderer>().SetPosition(0, new Vector3(-width*.5f,0,height*.5f));
    GetComponent<LineRenderer>().SetPosition(1, new Vector3(-width*.5f,0,-height*.5f));
    GetComponent<LineRenderer>().SetPosition(2, new Vector3(width*.5f,0,-height*.5f));
    GetComponent<LineRenderer>().SetPosition(3, new Vector3(width*.5f,0,height*.5f));
    GetComponent<LineRenderer>().SetPosition(4, new Vector3(-width*.5f,0,height*.5f));

    /*

      Create the different Audio Sources,
      so we can easily play each clip without
      the sounds taking over each others sources

    */

    s_hoverOverNote = transform.gameObject.AddComponent<AudioSource>();
    s_hoverOverNote.clip = hoverOverNote;
    s_hoverOverNote.loop = false;
    s_hoverOverNote.spatialize = true;
    s_hoverOverNote.volume = 1;

    s_pressNote = transform.gameObject.AddComponent<AudioSource>();
    s_pressNote.clip = pressNote;
    s_pressNote.loop = false;
    s_pressNote.spatialize = true;
    s_pressNote.volume = 1;

    s_hoverOverLoop = transform.gameObject.AddComponent<AudioSource>();
    s_hoverOverLoop.clip = hoverOverLoop;
    s_hoverOverLoop.loop = true;
    s_hoverOverLoop.spatialize = true;
    s_hoverOverLoop.volume = 0;
    s_hoverOverLoop.Play();

    s_pressLoop = transform.gameObject.AddComponent<AudioSource>();
    s_pressLoop.clip = pressLoop;
    s_pressLoop.loop = true;
    s_pressLoop.spatialize = true;
    s_pressLoop.volume = 0;
    s_pressLoop.Play();
  
  }

  
  // Update is called once per frame
  void Update () {



    triggerVal = 0;
    
    // tmp vector to help w/ garbage... I think?
    Vector3 d;

    if( enteredObject ){
    
      d = transform.InverseTransformPoint( enteredObject.transform.position );

      if( d.x < -width/1.5  || d.x > width/1.5 || d.z < -height/1.5 || d.z > height/1.5){
        OnPressUp();
      }
    
    }

    if( hovering == true ){

      d = transform.InverseTransformPoint( enteredObject.transform.position );

      triggerVal = 1 - (d.y / pushDistance);

    }

    if( pressing == true ){

      d = transform.InverseTransformPoint( enteredObject.transform.position );
      ValueY = -d.z / height;
      ValueY += .5f;

      ValueX = -d.x / width;
      ValueX += .5f;

      ValueX = Mathf.Clamp( ValueX , 0 , 1);
      ValueY = Mathf.Clamp( ValueY , 0 , 1);
      ValueXY = new Vector2( ValueX , ValueY );

      triggerVal = 2;
      s_pressLoop.pitch = 1f;

      if(WhileButtonPushed   != null) WhileButtonPushed(transform.gameObject , toggled );
      if(WhileSliderPushed   != null) WhileSliderPushed(transform.gameObject , ValueY );
      if(WhileSliderXYPushed != null) WhileSliderXYPushed(transform.gameObject, ValueXY );

    }

    if( hovering == true ){
      s_hoverOverLoop.volume = triggerVal;
    }else{
      s_hoverOverLoop.volume = 0;
    }

    if( hovering == false && inside == false && pressing == false ){ enteredObject = null; }
    
    // make haptics happen!
    if( enteredObject != null ){

      SteamVR_TrackedObject tObj = enteredObject.GetComponent<SteamVR_TrackedObject>();
      var device = SteamVR_Controller.Input((int)tObj.index);
      var v = triggerVal * triggerVal * triggerVal * 200;
      device.TriggerHapticPulse((ushort)v);
    
    }


    // update renderer w/ values
    GetComponent<Renderer>().material.SetVector( "_HandL" , HandL.transform.position );
    GetComponent<Renderer>().material.SetVector( "_HandR" , HandR.transform.position );
    GetComponent<Renderer>().material.SetFloat("_TriggerVal", triggerVal );
    GetComponent<Renderer>().material.SetFloat("_DownVal", downVal );
    GetComponent<Renderer>().material.SetFloat("_ToggleVal", toggleVal );
    GetComponent<Renderer>().material.SetFloat("_SliderVal", ValueY );
    GetComponent<Renderer>().material.SetFloat("_yVal", ValueY );
    GetComponent<Renderer>().material.SetFloat("_xVal", ValueX );

    SliderValue = ValueY;
  
  }

  void CreateMesh(){
   
    MeshFilter mf = GetComponent<MeshFilter>();
    
    var m = new Mesh();
    mf.mesh = m;
    
    Vector3[] vertices = new Vector3[ resolutionY * resolutionX ];
    Vector2[] uvs = new Vector2[ resolutionY * resolutionX ];
    int[] triangles = new int[ (resolutionX-1) *  (resolutionY-1) * 2 * 3];
    
    for( int i = 0; i < resolutionY; i++ ){
      for( int j = 0; j < resolutionX; j++ ){

        float x = ((float)j+.5f) / (float)resolutionX;
        float z = ((float)i+.5f) / (float)resolutionY; 

        float uvX = x;
        float uvY = z;

        x -= .5f;
        z -= .5f;

        x *= width;
        z *= height;

        vertices[ i * resolutionX + j ] = new Vector3( x , pushDistance , z);
        uvs[ i * resolutionX + j ] = new Vector2( uvX , uvY );

      }
    }


    for( int i = 0; i < resolutionY-1; i++ ){
      for( int j = 0; j < resolutionX-1; j++ ){

        int baseID = i * (resolutionX-1) + j;
        baseID *= 6;

        int t1 = (j+0) + (i+0) * resolutionX;
        int t2 = (j+0) + (i+1) * resolutionX;
        int t3 = (j+1) + (i+1) * resolutionX;
        int t4 = (j+1) + (i+0) * resolutionX;

        triangles[ baseID + 0 ] = t1;
        triangles[ baseID + 1 ] = t2;
        triangles[ baseID + 2 ] = t3;
        triangles[ baseID + 3 ] = t1;
        triangles[ baseID + 4 ] = t3;
        triangles[ baseID + 5 ] = t4;

      }
    }
    


    m.vertices = vertices;
    m.uv = uvs;
    m.triangles = triangles;

    m.RecalculateNormals();

    //mesh.mesh = m;

  }

  void OnTriggerEnter( Collider o ){

    if( o.gameObject.tag == "Hand" ){
    
      inside = true;
      enteredObject = o.gameObject;

      Vector3 d = transform.InverseTransformPoint( o.gameObject.transform.position );
      
      if( d.y > pushDistance * .8f ){
        OnHoverOver();
      }

      if( d.y < pushDistance * .1f * transform.localScale.y && pressing == true ){
        OnPressUp();
      }
    }


   

  }

  

  void OnTriggerStay(){

  }

  void OnTriggerExit(Collider o){

    if( o.gameObject.tag == "Hand"){

      inside = false;

      if( hovering == true ){

        Vector3 d = transform.InverseTransformPoint( o.gameObject.transform.position );
      
        if( d.y < pushDistance * .1f ){
          OnPressDown(d);
        }else{
          GetComponent<Renderer>().material.SetVector( "_Debug" , new Vector3(1,1,1));
        }
      
      }

      hovering = false;
    }

  }

  void OnHoverOver(){

    Vector3 d = transform.InverseTransformPoint( enteredObject.transform.position );
    float v = -d.z / height;
    
    hovering = true;
    GetComponent<Renderer>().material.SetVector( "_Debug" , new Vector3(0,1,1));
    s_hoverOverNote.pitch = .8f + (v+1) * 2;
    s_hoverOverNote.Play();
    
  }

  public void OnPressDown(Vector3 d){

    float v = -d.z / height;
    pressing = true;
    s_pressNote.pitch = 1f;//.8f + v * 4;
    s_pressNote.Play();
    
    s_pressLoop.volume = 1;
    downVal = 1;

    GetComponent<Renderer>().material.SetVector( "_Debug" , new Vector3(0,1,0));
    
    if(OnButtonFire != null) OnButtonFire(transform.gameObject);

    if( toggled == false ){
      ToggleOn();
    }else if( toggled == true ){
      ToggleOff();
    }

    //toggleVal = 1;

    


  }

  public void ToggleOn(){
    toggleVal = 1;
    toggled = true;
    if( OnButtonToggleOn != null ) OnButtonToggleOn( transform.gameObject );
  }

  public void ToggleOff(){
    toggleVal = 0;
    toggled = false;
    if( OnButtonToggleOff != null ) OnButtonToggleOff( transform.gameObject );
  }

  public void OnPressUp(){

    if( inside == true ){
      hovering = true;
    }

    downVal = 0;

    s_pressLoop.volume = 0;
    pressing = false;

    if(OnButtonRelease != null) OnButtonRelease(transform.gameObject);

    GetComponent<Renderer>().material.SetVector( "_Debug" , new Vector3(1,1,0));
  }


}

