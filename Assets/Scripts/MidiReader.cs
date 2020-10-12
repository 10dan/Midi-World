using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MidiParser;
using System.Linq;
using System.Threading;
using System.Diagnostics;
using UnityEngine.Rendering.PostProcessing;
using System;

public class MidiReader : MonoBehaviour {

    private float timeBetweenTicks = (1f / 192f);

    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip song;
    List<MidiFile> midis = new List<MidiFile>();
    List<List<int>> uniqueNotes = new List<List<int>>();
    List<MusicalResponder> responders;
    public bool[][] objectsActive;
    Thread musicThread;

    private void Awake() {
        //Create the midi files and store them in list called midis.
        string path = "Assets/Resources/Midis/";
        DirectoryInfo d = new DirectoryInfo(path);
        FileInfo[] files = d.GetFiles();
        foreach (FileInfo fi in files) {
            if (fi.Extension.EndsWith(".mid")) {
                midis.Add(new MidiFile(fi.FullName));
            }
        }

        //Analize the number of tracks, and number of notes, update in uniqueNotes[][].
        AnalizeMidi();

        //Set the active notes list to same length as the corresponding number of objects.
        objectsActive = new bool[uniqueNotes.Count][];
        for (int i = 0; i < uniqueNotes.Count; i++) {
            objectsActive[i] = new bool[uniqueNotes[i].Count];
        }

        musicThread = new Thread(new ThreadStart(ThreadMidi));
        musicThread.Start();
        audioSource.PlayOneShot(song);
    }
    private void Update() {
        //If the music ends.
        if (!(audioSource.isPlaying)) {
            //Reset event pointers and tick counter t.
            audioSource.PlayOneShot(song);
            t = 0;
            currentEvents  = new int[midis.Count];
        }
    }

    private void AnalizeMidi() {
        for (int i = 0; i < midis.Count; i++) {
            MidiTrack track = midis[i].Tracks[0];
            uniqueNotes.Add(new List<int>());
            foreach (MidiEvent e in track.MidiEvents) {
                if (e.MidiEventType == MidiEventType.NoteOn) {
                    if (uniqueNotes[i].Contains(e.Note)) {
                        //Do nothing as note as been accounted for.
                    } else {
                        uniqueNotes[i].Add(e.Note);
                    }
                }
            }
        }
    }
    int t;
    int[] currentEvents;
    public void ThreadMidi() {
        t = 0; //Time in ticks
        currentEvents = new int[midis.Count];
        MidiEvent e;
        while (true) {

            //Go through all midi tracks
            for (int i = 0; i < midis.Count; i++) {

                //Set e to the next scheduled event. (track[0] is default, ableton exports that way)
                e = midis[i].Tracks[0].MidiEvents[currentEvents[i]];

                //While t == e.Time check if its a note event, if not move on. (While loop incase 2 events are at same time)
                while (t == e.Time) {
                    if (e.MidiEventType == MidiEventType.NoteOn) {
                        ActivateNote(i, e.Note);
                    }

                    //If we get to the end of the list of events reset the counter.
                    if (currentEvents[i] < midis[i].Tracks[0].MidiEvents.Count - 1) {
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

    private void ActivateNote(int track, int note) {
        for (int j = 0; j < uniqueNotes[track].Count; j++) {
            if (note == uniqueNotes[track][j]) {
                objectsActive[track][j] = true;
            }
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