using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicalResponder : MonoBehaviour {
    [SerializeField] int track = 0; //The midi track this responder belongs to.
    [SerializeField] int id = 0; //The identification number.

    enum Behavior {
        Explode, Contract, Color
    }
    [SerializeField] Behavior behavior;
    [SerializeField] float shrinkRate=0.02f;
    private MidiReader midiReader;

    //Info needed for scaling.
    private Vector3 originalSize;

    private void Start() {
        originalSize = transform.localScale;
        midiReader = GameObject.FindObjectOfType<MidiReader>();
    }
    private void FixedUpdate() {
        if(midiReader.objectsActive[track][id] == true) {
            transform.localScale = new Vector3(2, 2, 2);
            midiReader.objectsActive[track][id] = false;
        }
        if(transform.localScale.x > shrinkRate) {
            transform.localScale -= new Vector3(shrinkRate, shrinkRate, shrinkRate);
        }
    }
}
