using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MidiParser;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using UnityEngine.Rendering.PostProcessing;

public class MidiReader : MonoBehaviour {

    private float timeBetweenTicks = (1f / 192f);
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip song;


    [SerializeField] GameObject[] objects;
    bool[] objectsActive;
    [SerializeField] float shrinkRate;

    MidiFile midiFile;

    private void Awake() {
        midiFile = new MidiFile("Assets/Resources/Music/drum2.mid");
        objectsActive = new bool[objects.Length];
    }
    private void Start() {
        Invoke("StartSong", 1f);
    }
    void StartSong() {
        Thread t = new Thread(new ThreadStart(ThreadMidi));
        t.Start();
        audioSource.PlayOneShot(song);
    }
    int boxSize = 3;
    private void Update() {
        for (int i = 0; i < objectsActive.Length; i++) {
            if (objectsActive[i] == true) {
                objects[i].transform.localScale = new Vector3(boxSize, boxSize, boxSize);
                objectsActive[i] = false;
            }
            if (objects[i].transform.localScale.x > shrinkRate) {
                objects[i].transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
            }
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
                    if (e.Note == 48) {
                        objectsActive[0] = true;
                    }
                    if (e.Note == 50) {
                        objectsActive[1] = true;
                    }
                    if (e.Note == 49) {
                        objectsActive[2] = true;
                    }
                    if (e.Note == 51) {
                        objectsActive[3] = true;
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