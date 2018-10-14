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
    public GameObject noteTrailPrefab;

    public int SetEnergyBarFillPercentage;
    public int SetBluesBarFillPercentage;

    private float onePercentSize;
    private float EnergyCurrentPerc = 100f;
    private float BluesCurrentPerc = 10f;

    private Queue<string> timingsList;
    private Queue<TimeStamp> timeStamps;
    private float startTime;

    public float noteSpeed = 20;

    public AudioSource songFileNurInst;
    public AudioSource songFileOhneInst;
    public AudioSource songFileFull;

    public struct TimeStamp
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
        public bool hasEnd()
        {
            if (minEnd != 0 || secEnd != 0 || msEnd != 0)
                return true;
            return false;
        }
        public float getMilliseconds()
        {
            float res = 0;
            res += min * 60000;
            res += sec * 1000;
            res += ms*10;
            return res;
        }
        public float getMillisecondsEnd()
        {
            float res = 0;
            res += minEnd * 60000;
            res += secEnd * 1000;
            res += msEnd * 10;
            return res;
        }
        public float deltaTime()
        {
            float res = 0;
            res += minEnd * 60000;
            res += secEnd * 1000;
            res += msEnd * 10;
            return res-getMilliseconds();
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

        setBluesPercentage(10);

        spawnRythm(timingsList);
    }

    // Update is called once per frame
    void Update()
    {
        if (timeStamps.Count > 0) {
            float width = noteBackgroundArea.sizeDelta.x / 2; // adjust to note spawnpoint
            float offset = width * (noteSpeed * Time.deltaTime);
            TimeStamp now = new TimeStamp((Time.time * 1000 + offset) - startTime);
            if (now >= timeStamps.Peek()) {
                TimeStamp temp = timeStamps.Dequeue();
                SpawnNote(temp);
            }
        }
        else {
            spawnRythm(timingsList); // loops NoteSpawn
        }
    }
    public void spawnRythm(Queue<string> rythmQueue)
    {
        startTime = Time.time * 1000;
        Queue<string> timingsListtemp = new Queue<string>(timingsList);
        while (timingsListtemp.Count > 0) {
            TimeStamp crtTimeStamp = new TimeStamp();
            string timingString = timingsListtemp.Dequeue();

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
        songFileOhneInst.Play();        
    }
    public void SpawnNote(TimeStamp timeStamp)
    {
        float xpos = noteBackgroundArea.position.x; //if tweeked check update() for time calculation
        float ypos = noteBackgroundArea.position.y;
        float zpos = noteBackgroundArea.position.z;        

        GameObject thatNote = Instantiate(notePrefab, new Vector3(xpos, ypos, zpos), Quaternion.identity, noteBackgroundArea.transform);
        float timeOffset = 1;
        if (timeStamp.hasEnd()) {
            var dist = timeStamp.deltaTime() * (noteSpeed * Time.deltaTime);
            timeOffset = dist / (noteSpeed * Time.deltaTime);
        }
        thatNote.GetComponent<NoteBehavior>().InitNoteSpeed(noteSpeed, timeOffset);
        if (timeStamp.hasEnd()) {
            GameObject thatNoteTrail = Instantiate(noteTrailPrefab, new Vector3(xpos, ypos, zpos), Quaternion.identity, thatNote.transform);
            //hatNoteTrail.GetComponent<RectTransform>().position = new Vector3(xpos, ypos, zpos);
            //thatNoteTrail.GetComponent<NoteBehavior>().InitNoteSpeed(speed);
            float size = timeStamp.deltaTime() * (noteSpeed * Time.deltaTime); // adjust note trail size
            thatNoteTrail.GetComponent<RectTransform>().sizeDelta = new Vector2(size, thatNoteTrail.GetComponent<RectTransform>().sizeDelta.y);
        }
    }

    public void IncreaseEnergy(float percentageToAdd)
    {
        float perc = EnergyCurrentPerc + percentageToAdd;
        if (perc > 100f)
            perc = 100f;
        else if (perc < 0f)
            perc = 0f;

        energyBar.sizeDelta = new Vector2(perc * onePercentSize, energyBar.sizeDelta.y); // m = dur * speed
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
    public void setEnergyPercentage(float setPercentage)
    {
        if (setPercentage > 100f)
            setPercentage = 100f;
        else if (setPercentage < 0f)
            setPercentage = 0f;

        energyBar.sizeDelta = new Vector2(setPercentage * onePercentSize, energyBar.sizeDelta.y);
        EnergyCurrentPerc = setPercentage;
    }
    public void setBluesPercentage(float setPercentage)
    {
        if (setPercentage > 100f)
            setPercentage = 100f;
        else if (setPercentage < 0f)
            setPercentage = 0f;

        bluesBar.sizeDelta = new Vector2(setPercentage * onePercentSize, bluesBar.sizeDelta.y);
        BluesCurrentPerc = setPercentage;
    }
}

