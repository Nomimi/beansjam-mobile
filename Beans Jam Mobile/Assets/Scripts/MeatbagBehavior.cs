using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MeatbahBehavior : MonoBehaviour
{

	#region Members

	private GameObject _walkSpace;

	private NavMeshAgent _navMeshAgent;

	private float _nextActionTime = 0.0f;

	private bool _hastLeftSpawn = false;

	#endregion Members

	#region Properties

	#endregion Properties

	// Use this for initialization
	void Start()
	{
		_walkSpace = GameObject.FindGameObjectWithTag("Spawn");
		_navMeshAgent = gameObject.GetComponent<NavMeshAgent>();
		_navMeshAgent.autoRepath = true;
		_navMeshAgent.destination = _walkSpace.transform.position +
																_walkSpace.transform.forward * Random.Range(-1f, 1f) +
																_walkSpace.transform.right * Random.Range(-1f, 1f);
	}

	// Update is called once per frame
	void Update()
	{
		if (Time.time > _nextActionTime && _navMeshAgent.remainingDistance < 0.1)
		{
			_nextActionTime += Random.Range(5, 10);
			if (Random.value < 0.97f)
			{
				_navMeshAgent.destination = _walkSpace.transform.position +
																		_walkSpace.transform.forward * Random.Range(-1f, 1f) +
																		_walkSpace.transform.right * Random.Range(-1f, 1f); //RandomNavSphere(transform.position, Random.Range(0.5f, 1.5f), -1);
			}
			else
			{
				// Small chance that meatbag leaves
				_navMeshAgent.destination = GameObject.FindGameObjectWithTag("Killbox").transform.position;
			}

		}
	}

	public static Vector3 RandomNavSphere(Vector3 origin, float distance, int layermask)
	{
		Vector3 randomDirection = UnityEngine.Random.insideUnitSphere * distance;

		randomDirection += origin;

		NavMeshHit navHit;

		NavMesh.SamplePosition(randomDirection, out navHit, distance, layermask);

		return navHit.position;
	}

	void OnTriggerEnter(Collider other)
	{
		if (_hastLeftSpawn && other.CompareTag("Killbox"))
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().RemoveMeatbag(gameObject);
			Destroy(gameObject);
		}
		else if (other.CompareTag("Head"))
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().Eat(gameObject);
			Destroy(gameObject);
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.CompareTag("Killbox"))
		{
			_hastLeftSpawn = true;
		}
	}
}
