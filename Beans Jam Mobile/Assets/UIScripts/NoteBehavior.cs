using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NoteBehavior : MonoBehaviour, IPointerClickHandler
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
		if (registeredForDelete && noteSpeed > 0)
		{
			_despawntimeOffset -= Time.deltaTime;
			if (_despawntimeOffset <= 0.0f)
			{
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

	public void OnPointerClick(PointerEventData eventData)
	{
		var controller = GameObject.FindGameObjectWithTag("GameController");
		Debug.Log("HIT");

		// check Note position offset from center
		//float points;
		//float x = System.Math.Abs(touchedObj.transform.position.x);
		//if (_noteHitArea.GetComponent<RectTransform>().sizeDelta.x / 2 < x)
		//	points = -missedNotePenalty;
		//else
		//{
		//	if (x.Equals(0))
		//	{
		//		x = 0.001f;
		//	}
		//	if (_meatBags.Count > 0)
		//		points = 1 / mapNumber(x, 0, touchedObj.transform.position.x, 0, 1) * _meatBags.Count; //remap distance to 0-1
		//	else
		//		points = 1 / mapNumber(x, 0, touchedObj.transform.position.x, 0, 1);
		//}

		//float percentage = BluesGoal / 100 * points;
		//_UIController.GetComponent<GameUiScript>().IncreaseBlues(percentage); //Punkte von 0-1000
	}
}
