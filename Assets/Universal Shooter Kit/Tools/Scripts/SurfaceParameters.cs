// GercStudio
// © 2018-2019

using System;
using System.Collections.Generic;
using UnityEngine;

namespace GercStudio.USK.Scripts
{

    [CreateAssetMenu(fileName = "Surface Material", menuName = "Surface Preset")]
    public class SurfaceParameters : ScriptableObject
    {
        public Transform Sparks;
        public Transform Hit;
        public AudioClip HitAudio;
        
        public List <AudioClip> ShellDropSounds;

        [Serializable]
        public class FootstepsSounds
        {
            public List<AudioClip> FootstepsAudios = new List<AudioClip>();
        }
        
        public FootstepsSounds[] footstepsSounds = new FootstepsSounds[0];
        public int currentTag;
        public int tagsCount;
        public int inspectorTab;
        public Inputs inputs;
    }

}


