using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MidiParser;
using System.Linq;
using System.Threading;
using System.Diagnostics;

public class MidiReader : MonoBehaviour {

    private float timeBetweenTicks = (1f / 192f);
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip song;


    [SerializeField] GameObject[] objects;
    bool[] objectsActive;

    [SerializeField] GameObject kickBox;
    [SerializeField] GameObject snareBox;
    [SerializeField] GameObject hatBox;
    [SerializeField] GameObject openBox;
    [SerializeField] float shrinkRate;

    MidiFile midiFile;

    private void Awake() {
        midiFile = new MidiFile("Assets/Resources/Music/drum2.mid");
    }
    private void Start() {
        Invoke("StartSong", 1f);
    }
    void StartSong() {
        Thread t = new Thread(new ThreadStart(ThreadMidi));
        t.Start();
        audioSource.PlayOneShot(song);
    }

    bool playHat, playSnare, playKick, playOpen = false;
    int boxSize = 3;
    private void Update() {
        if (playHat) {
            hatBox.transform.localScale = new Vector3(boxSize, boxSize, boxSize);
            playHat = false;
        }
        if (playSnare) {
            snareBox.transform.localScale = new Vector3(boxSize, boxSize, boxSize);
            playSnare = false;
        }
        if (playKick) {
            kickBox.transform.localScale = new Vector3(boxSize, boxSize, boxSize);
            playKick = false;
        }
        if (playOpen) {
            openBox.transform.localScale = new Vector3(boxSize, boxSize, boxSize);
            playOpen = false;
        }

        if (hatBox.transform.localScale.x > shrinkRate) {
            hatBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
        if (snareBox.transform.localScale.x > shrinkRate) {
            snareBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
        if (kickBox.transform.localScale.x > shrinkRate) {
            kickBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
        if (openBox.transform.localScale.x > shrinkRate) {
            openBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
    }


    public void ThreadMidi() {
        int t = 0; //Time in ticks
        int currentEventIndex = 0;
        MidiEvent e;
        while (true) {
            e = midiFile.Tracks[0].MidiEvents[currentEventIndex];
            while (t == e.Time) {
                if (e.MidiEventType == MidiEventType.NoteOn) {
                    if(e.Note == 48) {
                        playKick = true;
                    }
                    if(e.Note == 50) {
                        playHat = true;
                    }
                    if(e.Note == 49) {
                        playSnare = true;
                    }
                    if(e.Note == 51) {
                        playOpen = true;
                    }
                }
                currentEventIndex++;
                e = midiFile.Tracks[0].MidiEvents[currentEventIndex];
            }
            t++;
            PauseThreadForSeconds(timeBetweenTicks);
        }
    }

    public void PauseThreadForSeconds(float waitTime) {
        var durationTicks = Mathf.Round(waitTime * Stopwatch.Frequency);
        var sw = Stopwatch.StartNew();
        while (sw.ElapsedTicks < durationTicks) {
            //Do nothing. Just wait.
        }
    }
}