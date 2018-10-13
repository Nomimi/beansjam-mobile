using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeatbagBehavior : MonoBehaviour
{

	private bool _hastLeftSpawn = false;

	// Use this for initialization
	void Start()
	{

	}

	// Update is called once per frame
	void Update()
	{

	}

	void OnTriggerEnter(Collider other)
	{
		if (_hastLeftSpawn && other.CompareTag("Killbox"))
		{
			GameObject.FindGameObjectWithTag("GameController").GetComponent<GameManager>().RemoveMeatbag(gameObject);
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
