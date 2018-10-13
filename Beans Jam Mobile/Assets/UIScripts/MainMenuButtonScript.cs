using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuButtonScript : MonoBehaviour
{    
    public Animator anim;
    int TriggerHash = Animator.StringToHash("TriggerMenuReset");
    // Use this for initialization
    void Start () {
       
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void PlayGame()
    {
        SceneManager.LoadScene(2);
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayLevel(int scene)
    {
        SceneManager.LoadScene(scene);
    }

    public void TriggerMenuBack()
    {
        anim.SetTrigger(TriggerHash);
    }

    public void TriggerMenuLevels()
    {
        anim.SetTrigger(TriggerHash);
    }
}
