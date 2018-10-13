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

    private Queue<string> timingsList;
    private Queue<TimeStamp> timeStamps;
    private float startTime;

    public float noteSpeed = 20;

    struct TimeStamp
    {
        public int min;
        public int sec;
        public int ms;

        public int minEnd;
        public int secEnd;
        public int msEnd;

        public TimeStamp(float milliseconds) : this() // generates TimeStampFormat from milliseconds
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
            return min + ":" + sec + ":" + ms;
        }
    }

    // Use this for initialization
    void Start()
    {
        timingsList = new Queue<string>(new[] { "0:02:87-0:05:04", "0:06:66", "0:06:83", "0:06:99", "0:07:15-0:08:94", "0:09:46-0:10:39", "0:10:42-0:12:42", "0:13:61", "0:13:82", "0:13:94", "0:14:23 - 0:16:27", "0:17:08", "0:17:42", "0:17:71", "0:18:04" });
        timeStamps = new Queue<TimeStamp>();

        float PercentageBarMaxWidth = barContainer.sizeDelta.x;
        onePercentSize = (PercentageBarMaxWidth / 100f);

        spawnRythm(timingsList);
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > EnergyDrainTimer) {
            EnergyDrainTimer += EnergyDrainPeriod;
            IncreaseEnergy(-1);
        }
        if (timeStamps.Count > 0) {
            TimeStamp now = new TimeStamp(Time.time * 1000 - startTime);
            if (now >= timeStamps.Peek()) {
                timeStamps.Dequeue();
                spawnNote(noteSpeed);
            }
        }
    }
    public void spawnRythm(Queue<string> rythmQueue)
    {
        startTime = Time.time * 1000;
        while (timingsList.Count > 0) {
            TimeStamp crtTimeStamp = new TimeStamp();
            string timingString = timingsList.Dequeue();

            if (timingString.Contains("-")) {
                string[] splitStartStop = timingString.Split('-');
                string[] splitStopTime = splitStartStop[1].Split(':');
                crtTimeStamp.minEnd = Int32.Parse(splitStopTime[0]);
                crtTimeStamp.secEnd = Int32.Parse(splitStopTime[1]);
                crtTimeStamp.msEnd = Int32.Parse(splitStopTime[2]);
                timingString = splitStartStop[0];
            }
            string[] splitTime = timingString.Split(':');
            crtTimeStamp.min = Int32.Parse(splitTime[0]);
            crtTimeStamp.sec = Int32.Parse(splitTime[1]);
            crtTimeStamp.ms = Int32.Parse(splitTime[2]);
            timeStamps.Enqueue(crtTimeStamp);
        }        
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

