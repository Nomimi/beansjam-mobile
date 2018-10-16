using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	#region Const

	private const float SpawnOffset = 8f;

	#endregion
	#region Members

	private GameObject _dino;

	private List<GameObject> _meatBags;

	private GameObject[] _notes;

	private GameObject _spawnArea;

	private GameObject _UIController;

	private GameObject _noteHitArea;

	private float _nextActionTime = 0.0f;

	private ParticleSystem[] _bloodSpatters;

	private Animator _anim;

	private AudioSource[] _sounds;

	private List<AudioSource> _eatingSounds;
	private List<AudioSource> _schnappSounds;

	private AudioSource _dinoBluesNoInst;
	private AudioSource _dinoFairNoInst;
	private AudioSource _fressAtackeNoInst;
	#endregion Members

	#region Properties

	public float Hunger;

	public float Saturation;

	public int Blues;

	public int BluesGoal;

	public bool Running = true;

	public float missedNotePenalty;

	public float MeatbagSpawnProbability;

	public GameObject MeatBagPrefab;

	public float MinSpawnTime;


	int _eatAnimHash = Animator.StringToHash("Dino_Eat");
	private AudioSource _dinoBluesInst;
	private AudioSource _fressAtackeInst;
	private AudioSource _dinoFairInst;
	private AudioSource _curTrack;

	#endregion Properties

	// Use this for initialization
	void Start()
	{
		// Dino stuff
		_dino = GameObject.FindGameObjectWithTag("Player");
		_bloodSpatters = _dino.GetComponentsInChildren<ParticleSystem>();
		_anim = _dino.GetComponent<Animator>();

		// other GameObjects
		_meatBags = new List<GameObject>();
		_spawnArea = GameObject.FindGameObjectWithTag("Spawn");

		_UIController = GameObject.FindGameObjectWithTag("UIController");

		_noteHitArea = GameObject.FindGameObjectWithTag("NoteHitArea");

		SetAllSounds();

		StartCoroutine("ApplyHunger");
	}

	void SetAllSounds()
	{
		_sounds = gameObject.GetComponentsInChildren<AudioSource>();

		_eatingSounds = _sounds.Where(x => x.clip.name.Contains("fressen")).ToList();
		_schnappSounds = _sounds.Where(x => x.clip.name.Contains("schnapp")).ToList();

		_dinoBluesNoInst = _sounds.Where(x => x.clip.name.Contains("DinoBlues_ohne")).ToList()[0];
		_dinoFairNoInst = _sounds.Where(x => x.clip.name.Contains("Dinofair_ohne")).ToList()[0];
		_fressAtackeNoInst = _sounds.Where(x => x.clip.name.Contains("Fressattacke_ohne")).ToList()[0];

		_dinoBluesInst = _sounds.Where(x => x.clip.name.Contains("DinoBlues_nur")).ToList()[0];
		_dinoFairInst = _sounds.Where(x => x.clip.name.Contains("Dinofair_nur")).ToList()[0];
		_fressAtackeInst = _sounds.Where(x => x.clip.name.Contains("Fressattacke_nur")).ToList()[0];


		var script = GetComponentInChildren<GameUiScript>();
		switch (levelvarscripot.LEVEL)
		{
			case 0:
				script.TriggerUiScript(levelvarscripot.LEVEL);
				_curTrack = _dinoBluesInst;
				_dinoBluesNoInst.Play();
				break;
			case 1:
				script.TriggerUiScript(levelvarscripot.LEVEL);
				_curTrack = _dinoFairInst;
				_dinoFairNoInst.Play();
				break;
			default:
				script.TriggerUiScript(2);
				_curTrack = _fressAtackeInst;
				_fressAtackeNoInst.Play();
				break;
		}

		_curTrack.volume = 0.5f;
		_curTrack.Play();
	}

	// Update is called once per frame
	void Update()
	{
		if (!Running || Saturation <= 0)
		{
			SceneManager.LoadScene(2);
		}

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

		//if (Input.touchCount > 0)
		//{
		//	foreach (var touch in Input.touches)
		//	{
		//		RaycastHit hit;
		//		var touchedObj = ReturnClickedObject(touch, out hit);

		//		if (touchedObj.CompareTag("Player"))
		//		{
		//		}
		//	}
		//}

		if (Input.GetMouseButtonDown(0))
		{
			RaycastHit hit;
			var touchedObj = ReturnClickedObject(out hit);
			if (touchedObj != null)
			{
				if (touchedObj.CompareTag("Player") || touchedObj.CompareTag("Head"))
				{
					_anim.Play(_eatAnimHash);
					int rand = Random.Range(0, _schnappSounds.Count());
					_schnappSounds[rand].Play();
					_UIController.GetComponent<GameUiScript>().IncreaseEnergy(Saturation);
				}
				else if (touchedObj.CompareTag("Note"))
				{
					StartCoroutine("PlayInstrument");
					float points;
					float x = System.Math.Abs(touchedObj.transform.position.x);
					if (_noteHitArea.GetComponent<RectTransform>().sizeDelta.x / 2 < x)
						points = -missedNotePenalty;
					else
					{
						if (x.Equals(0))
						{
							x = 0.001f;
						}
						// TODO
						//if (_meatBags.Count > 0)
						//	points = 1 / mapNumber(x, 0, touchedObj.transform.position.x, 0, 1) * _meatBags.Count; //remap distance to 0-1
						//else
						//	points = 1 / mapNumber(x, 0, touchedObj.transform.position.x, 0, 1);
						points = _meatBags.Count;
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

	//Method to Return Clicked Ui Object
	bool ReturnClickedUiObject(Image imgIn)
	{
		if (imgIn.Raycast(new Vector2(10, 100), Camera.main))
			return true;
		return false;
	}

	IEnumerator ApplyHunger()
	{
		for (float i = 100; i >= 0; i -= Hunger)
		{
			Saturation -= Hunger;
			i = Saturation;
			_UIController.GetComponent<GameUiScript>().setEnergyPercentage(Saturation);
			yield return null;
		}
		// i >= 0
		Running = false;
		// Load Game-Over Screen
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


	float mapNumber(float s, float a1, float a2, float b1, float b2) //maps value s from one range to the other
	{
		return b1 + (s - a1) * (b2 - b1) / (a2 - a1);
	}

	public void Eat(GameObject meatbag)
	{
		Saturation += 10;
		_meatBags.Remove(meatbag);
		foreach (var splat in _bloodSpatters)
		{
			splat.Play();
		}

		int rand = Random.Range(0, _eatingSounds.Count());
		_eatingSounds[rand].Play();
	}

	IEnumerator PlayInstrument()
	{
		_curTrack.volume = 1;
		for (int i = 0; i < 6; i++)
		{
			_curTrack.volume -= 0.1f;
			yield return new WaitForSeconds(0.1f);

		}

	}

	public void SetGameActive(bool b)
	{
		Running = false;
	}
}