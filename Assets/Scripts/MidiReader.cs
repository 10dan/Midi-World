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

    List<MidiFile> midis = new List<MidiFile>();

    private void Awake() {
        //Set the active notes list to same length as the corresponding number of objects.
        objectsActive = new bool[objects.Length];

        //Create the midi files and store them in list called midis.
        string path = "Assets/Resources/Midis/";
        DirectoryInfo d = new DirectoryInfo(path);
        FileInfo[] files = d.GetFiles();
        foreach (FileInfo fi in files) {
            if (fi.Extension.EndsWith(".mid")) {
                midis.Add(new MidiFile(fi.FullName));
            }
        }


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
        int[] currentEvents = new int[midis.Count];
        MidiEvent e;
        while (true) {
            //Go through all midi tracks
            for (int i = 0; i < midis.Count; i++) {
                //Set e to the next scheduled event. (track[0] is default, ableton exports that way)
                e = midis[i].Tracks[0].MidiEvents[currentEvents[i]];
                //While t == e.Time check if its a note event, if not move on. (While loop incase 2 events are at same time)
                while (t == e.Time) {
                    if (e.MidiEventType == MidiEventType.NoteOn) {
                        //print(e.Note);
                        if (e.Note == 36) {
                            objectsActive[0] = true;
                        }
                        if (e.Note == 38) {
                            objectsActive[2] = true;
                        }
                        if (e.Note == 40) {
                            objectsActive[3] = true;
                        }
                        if (e.Note == 41) {
                            objectsActive[1] = true;
                        }
                    }
                    //If we get to the end of the list of events reset the counter.
                    if (currentEvents[i] < midis[i].Tracks[0].MidiEvents.Count-1) {
                        currentEvents[i]++;
                    } else {
                        currentEvents[i] = 0;
                    }
                    e = midis[i].Tracks[0].MidiEvents[currentEvents[i]];
                }
            }

            //If not at the end of the 4 bars, ticks increase,
            if (t <= 96 * 4 * 4) {
                t++;
            } else { //Otherwise loop.
                t = 0;
            }
            //Wait for one tick.
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