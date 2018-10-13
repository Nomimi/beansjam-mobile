using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.PostProcessing;
using UnityEngine.UI;

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

	private ParticleSystem[] _bloodSpatters;

	private Text _gameOverText;

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
		_bloodSpatters = GetComponentsInChildren<ParticleSystem>();
		_gameOverText = GameObject.FindGameObjectWithTag("GameOverPanel").GetComponent<Text>();
		_gameOverText.CrossFadeAlpha(0f, 0f, true);

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
			if (touchedObj != null)
			{
				if (touchedObj.CompareTag("Player"))
				{
					//Set the head position to the mouse position
					//_dinoHead.transform.position = new Vector3(Camera.main.ScreenToWorldPoint(Input.mousePosition).y, 0,
					//	Camera.main.ScreenToWorldPoint(Input.mousePosition).z);
				}

				if (touchedObj.CompareTag("Note"))
				{
					// check Note position offset from center
					RectTransform tf = touchedObj.GetComponent<RectTransform>();
					int points = 1000 - (int)(tf.sizeDelta.x * tf.sizeDelta.x);
				}
			}
		}

		#endregion InputHandling

		foreach (var particleSystem in _bloodSpatters)
		{
			particleSystem.Play();
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
	for (float i = 100; i >= 0; i -= Hunger)
	{
		Saturation -= Hunger;
		i = Saturation;
		// TODO set UI 
		// TODO set Dino Model
		yield return null;
	}
	// i >= 0
	Running = false;
	// Load Game-Over Screen
	_gameOverText.CrossFadeAlpha(1f, 2f, true);
	StartCoroutine("TransitionToGameOver");
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
	return spawnPoint;
}

public void RemoveMeatbag(GameObject meatBag)
{
	_meatBags.Remove(meatBag);
}

IEnumerator TransitionToGameOver()
{
	var cam = Camera.main;
	for (float i = 0; i <= 150; i += 1.5f)
	{

		cam.focalLength = i;

		yield return null;
	}

}
}
