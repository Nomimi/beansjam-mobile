using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
	#region Const

	private const float SpawnOffset = 4f;

	#endregion
	#region Members

	private GameObject _dino;

	private List<GameObject> _meatBags;

	private GameObject[] _notes;

	private GameObject _spawnArea;

	private GameObject _UIController;

	private GameObject _noteHitArea;

	private float _nextActionTime = 0.0f;

	private Image _gameOverPanel;

	private ParticleSystem[] _bloodSpatters;

	private Text _gameOverText;

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

	public bool Running;

	public float missedNotePenalty;

	public float MeatbagSpawnProbability;

	public GameObject MeatBagPrefab;

	public float MinSpawnTime;

	int _eatAnimHash = Animator.StringToHash("Dino_Eat");

	public int Level;
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

		// UI stuff
		_gameOverText = GameObject.FindGameObjectWithTag("GameOverText").GetComponent<Text>();
		_gameOverText.CrossFadeAlpha(0f, 0f, true);
		_gameOverPanel = GameObject.FindGameObjectWithTag("GameOverPanel").GetComponent<Image>();
		Color c = _gameOverPanel.color;
		c.a = 0;
		_gameOverPanel.color = c;
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

		_dinoBluesNoInst = _sounds.Single(x => x.clip.name.Contains("DinoBlues_ohne"));
		_dinoFairNoInst = _sounds.Single(x => x.clip.name.Contains("Dinofair_ohne"));
		_fressAtackeNoInst = _sounds.Single(x => x.clip.name.Contains("Fressattacke_ohne"));

		var script = GetComponentInChildren<GameUiScript>();
		switch (levelvarscripot.LEVEL)
		{
			case 0:
				script.TriggerUiScript(levelvarscripot.LEVEL);
				_dinoBluesNoInst.Play();
				break;
			case 1:
				script.TriggerUiScript(levelvarscripot.LEVEL);
				_dinoFairNoInst.Play();
				break;
			default:
				script.TriggerUiScript(2);
				_fressAtackeNoInst.Play();
				break;
		}
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
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			_anim.Play(_eatAnimHash);
			RaycastHit hit;
			var touchedObj = ReturnClickedObject(out hit);
			if (touchedObj != null)
			{
				if (touchedObj.CompareTag("Player"))
				{
					_anim.Play(_eatAnimHash);
					int rand = Random.Range(0, _schnappSounds.Count());
					_schnappSounds[rand].Play();
				}
			}
			_notes = GameObject.FindGameObjectsWithTag("Note");
			foreach (GameObject note in _notes)
			{
				if (ReturnClickedUiObject(note.GetComponent<Image>()))
				{
					Debug.Log("note hit!");
					// check Note position offset from center
					float points;
					RectTransform tf = touchedObj.GetComponent<RectTransform>();
					float x = System.Math.Abs(tf.sizeDelta.x);
					if (_noteHitArea.GetComponent<RectTransform>().sizeDelta.x / 2 < x)
						points = -missedNotePenalty;
					else
					{
						if (x.Equals(0))
						{
							x = 0.001f;
						}
						points = 1 / mapNumber(x, 0, tf.sizeDelta.x, 0, 1) * _meatBags.Count; //remap distance to 0-1
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
		if (imgIn.Raycast(new Vector2(20, 20), Camera.main))
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
			// TODO set Dino Model
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

		// TODO sound
	}
}