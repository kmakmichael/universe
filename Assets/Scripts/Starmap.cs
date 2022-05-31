using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class Starmap : MonoBehaviour
{
    // input handling
    public InputActionReference toggleStarmap = null;

    public Starfield starfield;
    public Camera hmd;
    public MeshRenderer mapRender;
    public MeshRenderer needleRender;
    private readonly Vector3 microScale = Vector3.one * .6f;
    private readonly Vector3 macroScale = Vector3.one * 600f;

    private bool mapOpen = false;

    private void Awake() {
        // scaling and position
        transform.localScale = macroScale;
        transform.position = Vector3.zero;

        // input actions
        toggleStarmap.action.started += ToggleStarmap;
		
		// meshes that should be invisible
		mapRender.enabled = mapOpen;
        needleRender.enabled = mapOpen;
    }

    private void OnDestroy() {
        toggleStarmap.action.started -= ToggleStarmap;
    }

    public void ToggleStarmap(InputAction.CallbackContext ctxt) {
        mapOpen = !mapOpen;
        if (mapOpen) {
            transform.position = hmd.transform.position + hmd.transform.forward * 0.85f;
            transform.localScale = microScale;
            starfield.Minimize();
        } else {
            transform.position = Vector3.zero;
            transform.localScale = macroScale;
            starfield.Maximize();
        }
        mapRender.enabled = mapOpen;
        needleRender.enabled = mapOpen;
    }
}
