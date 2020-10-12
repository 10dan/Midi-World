using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicalResponder : MonoBehaviour {
    [SerializeField] int track = 0; //The midi track this responder belongs to.
    [SerializeField] int id = 0; //The identification number.

    private MidiReader midiReader;

    private void Start() {
        midiReader = GameObject.FindObjectOfType<MidiReader>();
    }

    private void FixedUpdate() {
        //If this particular block is active. TODO: change depending on behavior.
        if(midiReader.objectsActive[track][id] == true) {
            midiReader.objectsActive[track][id] = false;
            foreach(Transform child in transform) {
                child.GetComponent<PhysicalResponder>().Activate();
            }
        }
    }
}
