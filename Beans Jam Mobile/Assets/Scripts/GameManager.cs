using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Members

	private GameObject _dinoHead;

	#endregion Members

	#region Properties

	public float Hunger;

	public float Saturation;

	public int Blues;

	public int BluesGoal;

	#endregion Properties

	// Use this for initialization
	void Start()
	{
		_dinoHead = GameObject.FindGameObjectWithTag("Player");
	}

	// Update is called once per frame
	void Update()
	{
		if (Input.touchCount > 0)
		{
			foreach (var touch in Input.touches)
			{
				RaycastHit hit;
				var touchedObj = ReturnClickedObject(touch, out hit);

				if (touchedObj.CompareTag("Player"))
				{
					//Set the position to the mouse position
					_dinoHead.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).x, Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
						0.0f);
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			var touchedObj = ReturnClickedObject(out hit);

			if (touchedObj.CompareTag("Player"))
			{
				//Set the head position to the mouse position
				_dinoHead.transform.position = new Vector3(0.0f, Camera.main.ScreenToWorldPoint(Input.mousePosition).y,
					Camera.main.ScreenToWorldPoint(Input.mousePosition).z);
			}

			if (touchedObj.CompareTag("Note"))
			{
				// See if Note is in Circle
			}
		}
	}

	//Method to Return Clicked Object
	GameObject ReturnClickedObject(out RaycastHit hit)
	{
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
		{
			target = hit.collider.gameObject;
		}
		return target;
	}

	//Method to Return Clicked Object
	GameObject ReturnClickedObject(Touch t, out RaycastHit hit)
	{
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay(t.position);
		if (Physics.Raycast(ray.origin, ray.direction * 10, out hit))
		{
			target = hit.collider.gameObject;
		}
		return target;
	}

	IEnumerator ApplyHunger()
	{
		for (float i = 100; i >= 0 ; i -= Hunger)
		{

			// TODO set UI 
			yield return null;
		}
	}
}
