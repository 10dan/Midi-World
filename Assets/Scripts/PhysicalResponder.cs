using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicalResponder : MonoBehaviour {
    [SerializeField] float shrinkRate = 0.95f;
    enum Behavior {
        Explode, Contract, Color
    }
    [SerializeField] Behavior behavior;
    private Vector3 originalSize;

    void Start() {
        originalSize = transform.localScale;
    }
    private void FixedUpdate() {
        if (transform.localScale.x > 0) {
            transform.localScale = Vector3.Scale(transform.localScale, new Vector3(shrinkRate, shrinkRate, shrinkRate));
        }
    }
    public void Activate() {
        transform.localScale = originalSize;
    }
}
