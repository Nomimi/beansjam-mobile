using System;
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

    private Queue<string> timingsList; //= { 0:01:05 - 0:02:82, 0:04:58, };
    private Queue<TimeStamp> timeStamps;
    private float startTime;

    struct TimeStamp
    {        
        public int min;
        public int sec;
        public int ms;

        public TimeStamp(float milliseconds) : this()
        {
            min = (int)milliseconds / 60000;
            milliseconds -= min * 60000;
            sec = (int)milliseconds / 1000;
            milliseconds -= sec * 1000;
            ms = (int)milliseconds % 1000;
            if (ms > 100)
                ms /= 10;
        }
        public static bool operator >=(TimeStamp t1, TimeStamp t2)
        {
            if (t1.min >= t2.min)
                if (t1.sec >= t2.sec)
                    if (t1.ms >= t2.ms)
                        return true;
            return false;
        }
        public static bool operator <=(TimeStamp t1, TimeStamp t2)
        {
            if (t1.min <= t2.min)
                if (t1.sec <= t2.sec)
                    if (t1.ms <= t2.ms)
                        return true;
            return false;
        }

        public override string ToString()
        {
            return min+":" + sec + ":" + ms;
        }
    }

    // Use this for initialization
    void Start()
    {
        TimeStamp a1 = new TimeStamp(50f);
        TimeStamp a2 = new TimeStamp(1050f);
        TimeStamp a3 = new TimeStamp(2500f);
        TimeStamp a4 = new TimeStamp(62500f);
        timeStamps = new Queue<TimeStamp>();
        Debug.Log(a1 + "\n" );
        Debug.Log(a2 + "\n" );
        Debug.Log(a3 + "\n");
        Debug.Log(a4 + "\n");

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
        if(timeStamps.Count > 0) {
           // (Time.time * 1000 - startTime).ToString();

           // TimeStamp now =
          // if(( >= timeStamps.Peek())

        }
    }
    public void spawnRythm(Queue<bool> rythmQueue)
    {
        startTime = Time.time * 1000;
        foreach (string timing in timingsList) {
            TimeStamp crtTimeStamp = new TimeStamp();
            string[] splitTime = timingsList.Dequeue().Split(':');
            crtTimeStamp.min = Int32.Parse(splitTime[0]);
            crtTimeStamp.sec = Int32.Parse(splitTime[1]);
            crtTimeStamp.ms = Int32.Parse(splitTime[2]);
            timeStamps.Enqueue(crtTimeStamp);
        }
        executeRythm(rythmQueue, startTime);
    }
    public void executeRythm(Queue<bool> rythmQueue, float startTime)
    {
       
        
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

