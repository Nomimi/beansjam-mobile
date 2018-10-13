using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour {

    private float noteSpeed;
    private RectTransform tf;

	// Use this for initialization
	void Start () {
        tf = gameObject.GetComponent<RectTransform>();
	}
	
	// Update is called once per frame
	void Update () {
        tf.position = new Vector3(tf.position.x + (noteSpeed*Time.deltaTime), tf.position.y, tf.position.z);
	}  

    public void InitNoteSpeed(float speed)
    {
        noteSpeed = speed;
    }

    void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}
