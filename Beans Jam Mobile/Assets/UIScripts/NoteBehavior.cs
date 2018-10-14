using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{

    private float noteSpeed;
    private RectTransform tf;
    bool registeredForDelete = false;
    float targetTime;

    // Use this for initialization
    void Start()
    {
        tf = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        tf.position = new Vector3(tf.position.x + (noteSpeed * Time.deltaTime), tf.position.y, tf.position.z);
        if (registeredForDelete) {
            targetTime -= Time.deltaTime;
            if (targetTime <= 0.0f) {
                Destroy(gameObject);
            }
        }

    }

    public void InitNoteSpeed(float speed)
    {
        noteSpeed = speed;
    }

    void OnBecameInvisible()
    {
        
        if (gameObject.GetComponentInChildren<Renderer>().isVisible == false) {
            float targetTime = gameObject.GetComponentsInChildren<RectTransform>()[1].sizeDelta.x * (noteSpeed * Time.deltaTime);
            registeredForDelete = true;           
        }
    }
    
}
