using System.Collections;
using System.Collections.Generic;
using System.Security.Policy;
using UnityEngine;
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

	private GameObject _UIController;

	private GameObject _noteHitArea;

	private float _nextActionTime = 0.0f;

	private Image _gameOverPanel;

	private ParticleSystem[] _bloodSpatters;

	private Text _gameOverText;

	private Animator _anim;

	#endregion Members

	#region Properties

	public float Hunger;

	public float Saturation;

	public int Blues;

	public int BluesGoal;

	public bool Running;

	public float missedNotePenalty;

	public float MeatbagSpawnProbability;

	public GameObject MeatBagPrefab;

	public float MinSpawnTime;

	int _eatAnimHash = Animator.StringToHash("Dino_Eat");
	#endregion Properties

	// Use this for initialization
	void Start()
	{
		_dinoHead = GameObject.FindGameObjectWithTag("Player");
		_meatBags = new List<GameObject>();
		_spawnArea = GameObject.FindGameObjectWithTag("Spawn");
		_bloodSpatters = GetComponentsInChildren<ParticleSystem>();
		_gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<Text>();
		_gameOverText.CrossFadeAlpha(0f, 0f, true);
		_gameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel").GetComponent<Image>();
		Color c = _gameOverPanel.color;
		c.a = 0;
		_gameOverPanel.color = c;
		_noteHitArea = GameObject.FindGameObjectWithTag("NoteHitArea");
		_UIController = GameObject.FindGameObjectWithTag("UIController");
		_anim = GetComponent<Animator>();


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
					_anim.Play(_eatAnimHash);
				}

				foreach (var particleSystem in _bloodSpatters)
				{
					particleSystem.Play();
				}

				if (touchedObj.CompareTag("Note"))
				{
					// check Note position offset from center
					float points;
					RectTransform tf = touchedObj.GetComponent<RectTransform>();
					float x = System.Math.Abs(tf.sizeDelta.x);
					if (tf.sizeDelta.x / 2 < x)
						points = -missedNotePenalty;
					else
					{
						if (x == 0)
							x = 0.001f;
						points = 1 / mapNumber(x, 0, tf.sizeDelta.x, 0, 1); //remap distance to 0-1
					}

					float percentage = BluesGoal / 100 * points;
					_UIController.GetComponent<GameUiScript>().IncreaseBlues(percentage); //Punkte von 0-1000
				}
			}
		}

		#endregion InputHandling
	}

	//Method to Return Clicked Object
	GameObject ReturnClickedObject(out RaycastHit hit)
	{
		GameObject target = null;
		Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
		if (Physics.Raycast(ray.origin, ray.direction * 50, out hit))
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
			_UIController.GetComponent<GameUiScript>().setEnergyPercentage(i);
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

	IEnumerator TransitionToGameOver()
	{
		Color c = _gameOverPanel.color;
		for (float i = 0f; i <= 2; i += 0.2f)
		{
			c.a = i;
			_gameOverPanel.color = c;
			yield return new WaitForSeconds(.1f);
		}
	}


	float mapNumber(float s, float a1, float a2, float b1, float b2) //maps value s from one range to the other
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}
}