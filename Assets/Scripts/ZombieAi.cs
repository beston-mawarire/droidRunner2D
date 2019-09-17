using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieAi : MonoBehaviour {
	public GameObject target;
	public float speed;

	private Transform playerPos;//use transform to get position of object

	private float offset;

	// Use this for initialization
	void Start () {
		playerPos = target.transform;//getting reference to playe transform
		offset = -92f;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = Vector2.MoveTowards(transform.position, playerPos.position, speed * Time.deltaTime);
		/////
		Vector3 difference = target.transform.position - transform.position;
        difference.Normalize();
        float rotation_z = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(0f, 0f, rotation_z + offset);
	}
}
