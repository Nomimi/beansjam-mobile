using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;

public class GameManager : MonoBehaviour
{
	#region Const

	private const float SpawnOffset = 4f;

	#endregion
	#region Members

	private GameObject _dinoHead;

	private List<GameObject> _meatBags;

	private GameObject _spawnArea;

	private float _nextActionTime = 0.0f;

	#endregion Members

	#region Properties

	public float Hunger;

	public float Saturation;

	public int Blues;

	public int BluesGoal;

	public bool Running;

	public float MeatbagSpawnProbability;

	public GameObject MeatBagPrefab;

	public float MinSpawnTime;

	#endregion Properties

	// Use this for initialization
	void Start()
	{
		_dinoHead = GameObject.FindGameObjectWithTag("Player");
		_meatBags = new List<GameObject>();
		_spawnArea = GameObject.FindGameObjectWithTag("Spawn");

		StartCoroutine("ApplyHunger");
	}

	// Update is called once per frame
	void Update()
	{
		#region Meatbag creation

		//var creationProbability = Blues * MeatbagSpawnRate; // Note: * random?
		float fBlues = Blues;
		var spawn = fBlues / 100;
		if (Time.time > _nextActionTime && Random.value < fBlues)
		{
			_nextActionTime += MinSpawnTime;
			var meatBag = Instantiate(MeatBagPrefab, GetSpawnPoint(), new Quaternion());
			_meatBags.Add(meatBag);
		}

		#endregion


		#region InputHandling

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

			//if (touchedObj.CompareTag("Note"))
			//{
			//	// See if Note is in Circle
			//}
		}

		#endregion InputHandling

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
		for (float i = 100; i >= 0; i -= Hunger)
		{
			i = Saturation -= Hunger;
			// TODO set UI 
			// TODO set Dino Model
			yield return null;
		}
		// i >= 0
		Running = false;
		// Load Game-Over Screen
	}

	void OnCollisionEnter(Collider c)
	{
		if (c.CompareTag("MeatBag"))
		{
			Saturation += 10;
			Destroy(c.gameObject);

		}
	}

	Vector3 GetSpawnPoint()
	{
		Vector3 spawnPoint = _spawnArea.transform.position;
		if (Random.value < 0.5f)
		{
			// Spawn left
			spawnPoint -= _spawnArea.transform.right * SpawnOffset;
		}
		else
		{
			// Spawn right
			spawnPoint += _spawnArea.transform.right * SpawnOffset;
		}

		//if (Random.value < 0.5f)
		//{
		//	spawnPoint -= _spawnArea.transform.forward * 0.4f;
		//}
		//else
		//{
		//	spawnPoint += _spawnArea.transform.forward * 0.4f;
		//}

		return spawnPoint;
	}

	public void RemoveMeatbag(GameObject meatBag)
	{
		_meatBags.Remove(meatBag);
	}
}
