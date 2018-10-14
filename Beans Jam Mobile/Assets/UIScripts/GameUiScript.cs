using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameUiScript : MonoBehaviour
{

    RectTransform energyBar;
    RectTransform bluesBar;
    RectTransform barContainer;
    RectTransform noteAcceptanceArea;
    RectTransform noteBackgroundArea;
    GameObject notePrefab;
    GameObject notePrefab1;
    GameObject notePrefab2;
    GameObject notePrefab3;
    GameObject notePrefab4;
    GameObject noteTrailPrefab;
    public List<GameObject> noteList;

    public int SetEnergyBarFillPercentage;
    public int SetBluesBarFillPercentage;

    private float onePercentSize;
    private float EnergyCurrentPerc = 100f;
    private float BluesCurrentPerc = 10f;

    private Queue<string> timingsList0;
    private Queue<string> timingsList1;
    private Queue<string> timingsList2;

    private Queue<TimeStamp> timeStamps;
    private float startTime;

    public float noteSpeed = 20;

    public bool startPlaying = false;

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
            res += ms * 10;
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
            return res - getMilliseconds();
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
        //DinoBlues Level 1
        timingsList0 = new Queue<string>(new[] { "0:02:85","0:04:45","0:04:78","0:06:35","0:06:69","0:08:24","0:08:59","0:10:45","0:12:03",
            "0:12:36","0:13:92","0:14:27","0:15:82","0:16:17","0:18:04","0:18:98","0:19:91","0:20:88","0:21:82","0:22:76","0:23:72","0:24:07",
            "0:24:67","0:25:61","0:26:51","0:27:52","0:28:45","0:29:12","0:29:41","0:30:35","0:31:29","0:32:25","0:33:18-0:35:43","0:35:74",
            "0:36:06","0:36:39","0:36:69","0:37:02","0:39:83","0:40:16","0:40:48","0:40:81","0:43:32","0:43:62","0:44:23","0:44:58","0:47:39",
            "0:48:04","0:48:37","0:49:90","0:50:27","0:51:20","0:52:12","0:53:70","0:54:07","0:54:96","0:55:91-0:58:09" });

        timingsList1 = new Queue<string>(new[] { "0:01:11-0:02:19","0:03:21","0:03:57","0:05:33","0:06:05","0:06:76-0:07:70","0:08:86","0:09:23",
            "0:10:28","0:10:98","0:11:69","0:12:29-0:13:21","0:14:50","0:14:89","0:15:92","0:16:62","0:17:01","0:17:33","0:18:06-0:18:97","0:20:16",
            "0:20:54","0:21:58","0:22:28","0:22:98","0:23:71-0:24:62","0:26:16,0:26:52","0:27:24-0:28:20","0:28:98","0:29:35","0:30:06-0:30:81",
            "0:31:80","0:32:16","0:32:88","0:33:57","0:33:94","0:34:29","0:34:64","0:35:02-0:35:74","0:37:11,0:37:46","0:38:54-0:39:27","0:39:92",
            "0:40:29","0:41:34","0:41:69","0:42:03","0:42:38","0:42:76","0:43:10","0:44:16","0:44:53","0:44:88","0:45:24","0:45:58","0:45:80",
            "0:46:27","0:46:99","0:47:69","0:48:39","0:49:12-0:49:76" });

        timingsList2 = new Queue<string>(new[] { "" });

        timeStamps = new Queue<TimeStamp>();

        energyBar = GameObject.FindGameObjectWithTag("EnergyBar").GetComponent<RectTransform>();
        bluesBar = GameObject.FindGameObjectWithTag("BluesBar").GetComponent<RectTransform>();
        barContainer = GameObject.FindGameObjectWithTag("BarContainer").GetComponent<RectTransform>();
        noteAcceptanceArea = GameObject.FindGameObjectWithTag("NoteAcceptanceArea").GetComponent<RectTransform>();
        noteBackgroundArea = GameObject.FindGameObjectWithTag("NoteBackground").GetComponent<RectTransform>();

        float PercentageBarMaxWidth = barContainer.sizeDelta.x;
        onePercentSize = (PercentageBarMaxWidth / 100f);

        setBluesPercentage(10);
    }

    void TriggerUiScript(int levelInteger)
    {
        if (levelInteger == 0)
            spawnRythm(timingsList0);
        if (levelInteger == 1)
            spawnRythm(timingsList1);
        if (levelInteger == 2)
            spawnRythm(timingsList2);
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
    }
    public void spawnRythm(Queue<string> rythmQueue)
    {
        startTime = Time.time * 1000;
        Queue<string> timingsListtemp = new Queue<string>(rythmQueue);
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
        startPlaying = true;
    }
    public void SpawnNote(TimeStamp timeStamp)
    {
        float xpos = noteBackgroundArea.position.x; //if tweeked check update() for time calculation
        float ypos = noteBackgroundArea.position.y;
        float zpos = noteBackgroundArea.position.z;

        int rand = UnityEngine.Random.Range(0, 5);        
        GameObject tempprefab = noteList[rand];
        GameObject thatNote = Instantiate(tempprefab, new Vector3(xpos, ypos, zpos), Quaternion.identity, noteBackgroundArea.transform);
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

