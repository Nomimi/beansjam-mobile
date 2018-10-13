using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUiScript : MonoBehaviour
{

    public RectTransform energyBar;
    public RectTransform bluesBar;
    public RectTransform barContainer;
    public RectTransform noteAcceptanceArea;
    public RectTransform noteBackgroundArea;
    public GameObject notePrefab;
    public Canvas canvas;

    public float speed = 1;

    public int SetEnergyBarFillPercentage;
    public int SetBluesBarFillPercentage;

    private float onePercentSize;
    private float EnergyCurrentPerc = 100f;
    private float BluesCurrentPerc = 10f;

    private float EnergyDrainTimer = 1f;
    public float EnergyDrainPeriod = 0.25f;

    private Queue<string> rythmList; //= { 0:01:05 - 0:02:82, 0:04:58, };
    private float startTime;

    struct TimeStamp
    {        
        int min;
        int sec;
        int ms;
    }

    // Use this for initialization
    void Start()
    {        
        float PercentageBarMaxWidth = barContainer.sizeDelta.x;
        onePercentSize = (PercentageBarMaxWidth / 100f);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > EnergyDrainTimer) {
            EnergyDrainTimer += EnergyDrainPeriod;
            IncreaseEnergy(-1);            
        }
    }
    public void spawnRythm(Queue<bool> rythmQueue)
    {
        startTime = Time.time * 1000;
        executeRythm(rythmQueue, startTime);
    }
    public void executeRythm(Queue<bool> rythmQueue, float startTime)
    {
        float elapsedMS = (Time.time * 1000) - startTime;
        TimeStamp nextTimeStamp;
        string[] splitTime = rythmList.Dequeue().Split(':');
        nextTimeStamp splitTime[0]
    }
    public void spawnNote(float speed)
    {
        float xpos = noteBackgroundArea.position.x;
        float ypos = noteBackgroundArea.position.y;
        float zpos = noteBackgroundArea.position.z;

        (Instantiate(notePrefab, new Vector3(xpos, ypos, zpos), Quaternion.identity, canvas.transform) as GameObject).GetComponent<NoteBehavior>().InitNoteSpeed(speed); 
    }

    public void IncreaseEnergy(float percentageToAdd)
    {
        float perc = EnergyCurrentPerc + percentageToAdd;
        if (perc > 100f)
            perc = 100f;
        else if (perc < 0f)
            perc = 0f;

        energyBar.sizeDelta = new Vector2(perc * onePercentSize, energyBar.sizeDelta.y);
        EnergyCurrentPerc = perc;
    }
    public void IncreaseBlues(float percentageToAdd)
    {
        float perc = BluesCurrentPerc + percentageToAdd;
        if (perc > 100f)
            perc = 100f;
        else if (perc < 0f)
            perc = 0f;

        bluesBar.sizeDelta = new Vector2(perc * onePercentSize, bluesBar.sizeDelta.y);
        BluesCurrentPerc = perc;
    }
}

