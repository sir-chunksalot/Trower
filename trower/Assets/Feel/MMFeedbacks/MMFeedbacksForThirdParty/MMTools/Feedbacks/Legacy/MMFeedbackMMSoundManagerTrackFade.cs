﻿using MoreMountains.Tools;
using System;
using UnityEngine;

namespace MoreMountains.Feedbacks
{
    /// <summary>
    /// This feedback will let you fade all the sounds on a specific track at once. You will need a MMSoundManager in your scene for this to work.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("Audio/MMSoundManager Track Fade")]
    [FeedbackHelp("This feedback will let you fade all the sounds on a specific track at once. You will need a MMSoundManager in your scene for this to work.")]
    public class MMFeedbackMMSoundManagerTrackFade : MMFeedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.SoundsColor; } }
#endif

        /// the duration of this feedback is the duration of the fade
        public override float FeedbackDuration { get { return FadeDuration; } }

        [Header("MMSoundManager Track Fade")]
        /// the track to fade the volume on
        [Tooltip("the track to fade the volume on")]
        public MMSoundManager.MMSoundManagerTracks Track;
        /// the duration of the fade, in seconds
        [Tooltip("the duration of the fade, in seconds")]
        public float FadeDuration = 1f;
        /// the volume to reach at the end of the fade
        [Tooltip("the volume to reach at the end of the fade")]
        [Range(MMSoundManagerSettings._minimalVolume, MMSoundManagerSettings._maxVolume)]
        public float FinalVolume = MMSoundManagerSettings._minimalVolume;
        /// the tween to operate the fade on
        [Tooltip("the tween to operate the fade on")]
        public MMTweenType FadeTween = new MMTweenType(MMTween.MMTweenCurve.EaseInOutQuartic);

        /// <summary>
        /// On Play, triggers a fade event, meant to be caught by the MMSoundManager
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            MMSoundManagerTrackFadeEvent.Trigger(MMSoundManagerTrackFadeEvent.Modes.PlayFade, Track, FadeDuration, FinalVolume, FadeTween);
        }
    }
}