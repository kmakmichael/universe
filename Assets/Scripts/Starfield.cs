using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Globalization;
using UnityEngine.XR;
using UnityEngine.InputSystem;
[RequireComponent(typeof(ParticleSystem))]

public class Starfield : MonoBehaviour {
    // movement
    private const float moveSpeed = 0.004f;
    public Camera hmd;
    public InputActionReference moveHori = null;
    public InputActionReference moveUp = null;
    public InputActionReference moveDown = null;
    private Vector2 axisVal;
    private float moveUpVal;
    private float moveDownVal;
    private Vector3 moveVec;
    private Quaternion mquat;

    // scaling
    private const float scaleSpeed = 0.5f;
    public InputActionReference scaleUp = null;
    public InputActionReference scaleDown = null;
    private float scaleUpVal;
    private float scaleDownVal;

    // state
    private bool mapOpen = false;

    public struct Galaxy {
        public Galaxy(Vector3 p) {
            pos = p;
        }
        public Galaxy(float x, float y, float z) {
            pos = new Vector3(x, y, z);
        }
        public Vector3 pos { get; }

        public override string ToString() => $"({pos.ToString()})";
    }

    // Data importing
    public TextAsset csvFile;
    List<Galaxy> glist;


    // Particle System
    public ParticleSystem.Particle[] particles;
    public ParticleSystem particleSystem;
    private float scaleFactor = 1000f;
    private const float visualScale = .05f;
    int numAlive;
    bool needsUpdate;


    void Start() {
        ReadCSV();
        PopulateSystems();
        needsUpdate = true;
    }


    void LateUpdate() {
        InitializeIfNeeded();
        ParticleSystem.EmitParams emitOverride = new ParticleSystem.EmitParams();
        particleSystem.SetParticles(particles,numAlive);
        particleSystem.Emit(emitOverride, glist.Count);
        numAlive = particleSystem.GetParticles(particles);
        PopulateSystems();
        if (needsUpdate) {
            UpdateSystems();
        }
    }


    private void InitializeIfNeeded() {
        if (particleSystem == null)
            particleSystem = GetComponent<ParticleSystem>();

        if (particles == null || particles.Length < particleSystem.main.maxParticles)
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
    }


    private void PopulateSystems() {
        for (int i=0; i < glist.Count; i++) {
            particles[i].position = glist[i].pos * scaleFactor;
            particles[i].velocity = new Vector3(0,0,0);
        }
    }

    private void UpdateSystems() {
        if (mapOpen) {
            particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        } else {
            for (int i=0; i < glist.Count; i++) {
                particles[i].position = glist[i].pos * scaleFactor;
            }
        }
        needsUpdate = false;
    }


    private void ReadCSV() {
        var split = csvFile.text.Split(new char[] {'\n'});
        glist = new List<Galaxy>();
        for (int i = 1; i < split.Length; i++) {
            string[] coords = split[i].Split(new char[] {','});
            CultureInfo ci = (CultureInfo)CultureInfo.CurrentCulture.Clone();
            ci.NumberFormat.CurrencyDecimalSeparator = ".";
            var ps = float.Parse(coords[3], NumberStyles.Any, ci);
            Vector3 coordVec = new Vector3(float.Parse(coords[3], NumberStyles.Any, ci), float.Parse(coords[5], NumberStyles.Any, ci), float.Parse(coords[4], NumberStyles.Any, ci));
            if (!coordVec.Equals(Vector3.zero)) {
                glist.Add(new Galaxy(coordVec));
            }
        }
    }


    void Update() {
        // movement
        axisVal = moveHori.action.ReadValue<Vector2>();
        moveUpVal = moveUp.action.ReadValue<float>();
        moveDownVal = moveDown.action.ReadValue<float>();
        moveVec.Set(axisVal.x, moveDownVal - moveUpVal, axisVal.y);
        mquat.SetFromToRotation(Vector3.forward, hmd.transform.forward);
        moveVec = mquat * moveVec;
        if (mapOpen) {
            moveVec[1] = moveUpVal - moveDownVal;
            moveVec *= 50f;
        } else {
            moveVec *= -1.0f;
        }
        transform.localPosition += moveVec * moveSpeed * Time.deltaTime;

        // scaling
        scaleUpVal = scaleUp.action.ReadValue<float>();
        scaleDownVal = scaleDown.action.ReadValue<float>();
        scaleFactor = (scaleUpVal, scaleDownVal) switch {
            (1, 0) => scaleFactor * (1.0f + (scaleSpeed * Time.deltaTime)),
            (0, 1) => scaleFactor / (1.0f + (scaleSpeed * Time.deltaTime)),
            _ => scaleFactor,
        };
    }

    
    public void Minimize() {
        mapOpen = true;
        transform.localScale *= visualScale;
        scaleFactor /= visualScale;
        needsUpdate = true;
    }

    public void Maximize() {
        mapOpen = false;
        transform.localScale /= visualScale;
        scaleFactor *= visualScale;
        needsUpdate = true;
    }
}
