using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dinoController : MonoBehaviour
{
	public Animator Anim;
	int TriggerHash = Animator.StringToHash("buttonclick");

	public void EatTrigger()
	{
		Anim.SetTrigger(TriggerHash);
	}
}
