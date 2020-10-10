using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using MidiParser;
using System.Linq;
using System.Threading;
using System.Diagnostics;

public class MidiReader : MonoBehaviour {

    [SerializeField] float timeBetweenTicks;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip song;
    [SerializeField] AudioClip[] ns;

    [SerializeField] GameObject kickBox;
    [SerializeField] GameObject snareBox;
    [SerializeField] GameObject hatBox;
    [SerializeField] float shrinkRate = 0.1f;

    bool playing = false;

    List<int> kicksList = new List<int>();
    List<int> hatsList = new List<int>();
    List<int> snareList = new List<int>();
    int[] kicks;
    int[] hats;
    int[] snares;

    private void Awake() {
        MidiFile midiFile = new MidiFile("Assets/Resources/Music/drum2.mid");
        print(midiFile.TicksPerQuarterNote);
        foreach (var track in midiFile.Tracks) {
            foreach (var midiEvent in track.MidiEvents) {
                if (midiEvent.MidiEventType == MidiEventType.NoteOn) {
                    if (midiEvent.Note == 50) {
                        hatsList.Add(midiEvent.Time);
                    } else if (midiEvent.Note == 48) {
                        kicksList.Add(midiEvent.Time);
                    } else if (midiEvent.Note == 49) {
                        snareList.Add(midiEvent.Time);
                    }
                }
            }
        }
        hats = hatsList.ToArray();
        kicks = kicksList.ToArray();
        snares = snareList.ToArray();



    }
    private void Start() {
        Invoke("StartSong", 1f);
    }
    void StartSong() {

        Thread t = new Thread(new ThreadStart(ThreadMidi));
        t.Start();
        audioSource.PlayOneShot(song);
    }

    bool playHat, playSnare, playKick = false;
    int boxSize = 2;
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

        if (hatBox.transform.localScale.x > shrinkRate) {
            hatBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
        if (snareBox.transform.localScale.x > shrinkRate) {
            snareBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
        if (kickBox.transform.localScale.x > shrinkRate) {
            kickBox.transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
    }

    int kickI, snareI, hatI = 0; //Index of next time of drum.
    int t = 0; //Time in ticks
    public void ThreadMidi() {
        while (hatI < hats.Length) {
            int timeOfNextHat = hats[hatI];
            int timeOfNextKick = kicks[kickI];
            int timeOfNextSnare = hats[snareI];
            if ((t == timeOfNextHat) && (hatI < hats.Length)) {
                playHat = true;
                hatI++;
            }
            if ((t == timeOfNextSnare) && (snareI < snares.Length)) {
                playSnare = true;
                snareI++;
            }
            if ((t == timeOfNextKick) && (kickI < kicks.Length)) {
                playKick = true;
                kickI++;
            }
            t++;
            var durationTicks = Mathf.Round(0.00520833333f * Stopwatch.Frequency);
            var sw = Stopwatch.StartNew();

            while (sw.ElapsedTicks < durationTicks) {

            }
        }
    }
}