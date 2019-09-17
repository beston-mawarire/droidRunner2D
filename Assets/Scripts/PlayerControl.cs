using UnityEngine;
//learn how to make a 2d top down shooter

public class PlayerControl : MonoBehaviour {

	public float speed;

	public Rigidbody2D rb;

	public float offset = 0.0f;


	// Use this for initialization
	void Start () {
		speed = 3f;
	}

	void Update()
	{
		Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
	}
	
	/// <summary>
	/// This function is called every fixed framerate frame, if the MonoBehaviour is enabled.
	/// </summary>
	void FixedUpdate()
	{
		float x = Input.GetAxis("Horizontal") * Time.fixedDeltaTime * speed;
		float y = Input.GetAxis("Vertical") * Time.fixedDeltaTime * speed;
		// rb.transform.position = new Vector2(x,y);
		Vector2 newPos = new Vector2(rb.position.x + x, rb.position.y + y);
		rb.MovePosition(newPos);
	}
}

//game object movement
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
public class TopDownCharacterMovement : MonoBehaviour {
    private Rigidbody2D myRigidbody;
    [SerializeField] private float moveSpeed;
    // Use this for initialization
    void Start () {
        myRigidbody = GetComponent<Rigidbody2D>();
    }
  
  // Update is called once per frame
 void FixedUpdate () {
     Vector2 movingVector = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        myRigidbody.AddForce(movingVector * moveSpeed);
}
}
 */

//make game object follow mouse pointer
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
public class RotateToMouse : MonoBehaviour {
  public float offset = 0.0f;
  // Use this for initialization
  void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
        Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
    }
}
 */

//  Follow game object
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
public class FolowObject : MonoBehaviour {
    public GameObject target;
    public float moveSpeed;
    public float rotationSpeed;
    // Use this for initialization
    void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
        transform.position = Vector3.MoveTowards(transform.position, target.transform.position, moveSpeed * Time.deltaTime);
        Vector3 vectorToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, qt, Time.deltaTime * rotationSpeed);
    }
}
 */

//  Rotate gameobject to game object
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
// © 2017 TheFlyingKeyboard and released under MIT License
// theflyingkeyboard.net
public class RotateToObject : MonoBehaviour {
    public GameObject target;
    public float rotationSpeed;
    // Use this for initialization
    void Start () {
    
  }
  
  // Update is called once per frame
  void Update () {
        Vector3 vectorToTarget = target.transform.position - transform.position;
        float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
        Quaternion qt = Quaternion.AngleAxis(angle, Vector3.forward);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, qt, Time.deltaTime * rotationSpeed);
    }
}

path finding
https://www.youtube.com/watch?v=AKKpPmxx07w

tilemaps
https://www.youtube.com/watch?v=ryISV_nH8qw
https://www.youtube.com/watch?v=70sLhuE1sbc

mobile joystick
https://www.youtube.com/watch?v=nGYObojmkO4
https://www.youtube.com/watch?v=PyRvrD4OA1M

Android build
https://www.youtube.com/watch?v=nM0h5pQYQxM
https://www.youtube.com/watch?v=GEQSaF4LPgk
https://www.youtube.com/watch?v=aoby0Q4CqQ8
https://www.youtube.com/watch?v=4BD3y0NYNqk
https://www.youtube.com/watch?v=JFcM6sjBrZI
https://www.youtube.com/watch?v=qjYMHdhJSiE

Enemy spawning
https://www.youtube.com/watch?v=O2zODVcAq44
https://www.youtube.com/watch?v=A0PXmLadNrI
https://www.youtube.com/watch?v=AI8XNNRpTTw

Enemy AI follow
https://www.youtube.com/watch?v=rhoQd6IAtDo
https://www.youtube.com/watch?v=_Z1t7MNk0c4
 */