using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoteBehavior : MonoBehaviour
{

    private float noteSpeed;
    private RectTransform tf;
    bool registeredForDelete = false;
    float _despawntimeOffset;

    // Use this for initialization
    void Start()
    {
        tf = gameObject.GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        tf.position = new Vector3(tf.position.x + (noteSpeed * Time.deltaTime), tf.position.y, tf.position.z);
        if (registeredForDelete && noteSpeed > 0) {
            _despawntimeOffset -= Time.deltaTime;
            if (_despawntimeOffset <= 0.0f) {
                Destroy(gameObject);
            }
        }       
    }

    public void InitNoteSpeed(float speed, float despawntimeOffset)
    {
        noteSpeed = speed;
        _despawntimeOffset = despawntimeOffset / 1000;
    }

    void OnBecameInvisible()
    {
        registeredForDelete = true;        
    }

}
