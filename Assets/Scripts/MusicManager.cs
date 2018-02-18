using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{

    public AudioClip[] Songs;
    public float[] TransitionPos;
    public float[] TransitionLength;

    public AudioSource MusicBox1;
    public AudioSource MusicBox2;
    public AudioSource MusicTransitionBox;
    private float fadeLength;
    private bool musicBox1IsActive;

    private bool isSwitching;
    private int currentSongNum;
    private int nextSongNum;

    public float targetVolume;

    // Use this for initialization
    void Awake()
    {
        musicBox1IsActive = true;
        isSwitching = false;
        currentSongNum = -1;
        nextSongNum = -1;
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SwitchSong(int SongNum)
    {
        //CBUG.Do("Switch");
        if (isSwitching || currentSongNum == SongNum)
        {
            nextSongNum = SongNum;
            return;
        }

        //CBUG.Do("Switch");
        currentSongNum = SongNum;
        isSwitching = true;
        nextSongNum = -1;
        if (musicBox1IsActive)
        {
            //CBUG.Do("Fading out 1");
            musicBox1IsActive = false;
            fadeLength = TransitionLength[SongNum];
            //MusicBox1.time = TransitionPos[SongNum];
            MusicBox2.Stop();
            MusicBox2.clip = Songs[SongNum];
            MusicBox2.Play();
            StartCoroutine(LerpVolume1(targetVolume, 0.0f));
            StartCoroutine(LerpVolume2(0f, targetVolume));
        }
        else
        {
            //CBUG.Do("Fading out 2");
            musicBox1IsActive = true;
            fadeLength = TransitionLength[SongNum];
            //MusicBox2.time = TransitionPos[SongNum];
            MusicBox1.Stop();
            MusicBox1.clip = Songs[SongNum];
            MusicBox1.Play();
            StartCoroutine(LerpVolume1(0f, targetVolume));
            StartCoroutine(LerpVolume2(targetVolume, 0.0f));
        }
    }

    public IEnumerator LerpVolume1(float from, float to)
    {
        bool lerpUp = false;

        if (from < to)
            lerpUp = true;

        float startTime = Time.time;
        if (lerpUp)
        {
            while (MusicBox1.volume <= to - 0.01f)
            {
                MusicBox1.volume = Mathf.Lerp(from, to, (Time.time - startTime) / fadeLength);
                yield return null;
            }
        }
        else
        {
            while (MusicBox1.volume >= to + 0.01f)
            {
                MusicBox1.volume = Mathf.Lerp(from, to, (Time.time - startTime) / fadeLength);
                yield return null;
            }
        }

        isSwitching = false;
        //CBUG.Do("IsSwitching IS OFF in 1");
        //Only on LerpVolume1 ...??
        if (nextSongNum != -1)
        {
            SwitchSong(nextSongNum);
        }
    }

    public IEnumerator LerpVolume2(float from, float to)
    {
        bool lerpUp = false;

        if (from < to)
            lerpUp = true;

        //CBUG.Do("Lerping 2");

        float startTime = Time.time;
        if (lerpUp)
        {
            //CBUG.Do("LERP UP");
            while (MusicBox2.volume <= to - 0.01f)
            {
                MusicBox2.volume = Mathf.Lerp(from, to, (Time.time - startTime) / fadeLength);
                //CBUG.Do("Lerp in Vol2: " + MusicBox2.volume);
                yield return null;
            }
        }
        else
        {
            //CBUG.Do("LERP DOWN");
            //CBUG.Do("Playing down to " + to + " and currentVol for 2 is: " + MusicBox2.volume);
            while (MusicBox2.volume >= to + 0.01f)
            {
                MusicBox2.volume = Mathf.Lerp(from, to, (Time.time - startTime) / fadeLength);
                //CBUG.Do("Lerp in Vol2: " + MusicBox2.volume);
                yield return null;
            }
        }

        isSwitching = false;
        //CBUG.Do("IsSwitching IS OFF in 2");
        //NOT Only on LerpVolume1 ...??
        if (nextSongNum != -1)
        {
            SwitchSong(nextSongNum);
        }
    }

    public static void _SwitchTo(int songNum)
    {
        GameObject.FindGameObjectWithTag("MUSIC").GetComponent<MusicManager>().SwitchSong(songNum);
    }


}
