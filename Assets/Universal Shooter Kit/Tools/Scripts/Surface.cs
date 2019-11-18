// GercStudio
// © 2018-2019

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.USK.Scripts
{
    public class Surface : MonoBehaviour
    {
        public SurfaceParameters Material;
        [HideInInspector] public bool isCover;
        [HideInInspector] public Transform Sparks;
        [HideInInspector] public Transform Hit;
        [HideInInspector] public AudioClip HitAudio;
        [HideInInspector] public List<AudioClip> ShellDropSounds;
        //[HideInInspector] public AudioClip[] FootstepsAudios;
       // public List<SurfaceParameters.FootstepsSounds> FootstepsSounds;
        [HideInInspector] public SurfaceParameters.FootstepsSounds[] FootstepsSounds;
        [HideInInspector] public GameObject Shadow;

        void Awake()
        {
            if (Material)
            {
                Array.Resize(ref FootstepsSounds, Material.footstepsSounds.Length);
                
                Sparks = Material.Sparks;
                Hit = Material.Hit;
                FootstepsSounds = Material.footstepsSounds;
                ShellDropSounds = Material.ShellDropSounds;
                HitAudio = Material.HitAudio;
            }
        }
    }
}


