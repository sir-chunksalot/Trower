﻿using MoreMountains.Feedbacks;
using UnityEngine;

namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>
    /// This feedback allows you to control chromatic aberration intensity over time. It requires you have in your scene an object with a PostProcessVolume
    /// with Chromatic Aberration active, and a MMChromaticAberrationShaker component.
    /// </summary>
    [AddComponentMenu("")]
    [FeedbackPath("PostProcess/Chromatic Aberration")]
    [FeedbackHelp("This feedback allows you to control chromatic aberration intensity over time. It requires you have in your scene an object with a PostProcessVolume " +
                  "with Chromatic Aberration active, and a MMChromaticAberrationShaker component.")]
    public class MMFeedbackChromaticAberration : MMFeedback
    {
        /// a static bool used to disable all feedbacks of this type at once
        public static bool FeedbackTypeAuthorized = true;
        /// sets the inspector color for this feedback
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }
#endif

        [Header("Chromatic Aberration")]
        /// the channel to emit on
        [Tooltip("the channel to emit on")]
        public int Channel = 0;
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float Duration = 0.2f;
        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        public bool ResetTargetValuesAfterShake = true;
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(0f, 1f)]
        public float RemapIntensityZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(0f, 1f)]
        public float RemapIntensityOne = 1f;

        [Header("Intensity")]
        /// the curve to animate the intensity on
        [Tooltip("the curve to animate the intensity on")]
        public AnimationCurve Intensity = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the multiplier to apply to the intensity curve
        [Tooltip("the multiplier to apply to the intensity curve")]
        [Range(0f, 1f)]
        public float Amplitude = 1.0f;
        /// whether or not to add to the initial intensity
        [Tooltip("whether or not to add to the initial intensity")]
        public bool RelativeIntensity = false;

        /// the duration of this feedback is the duration of the shake
        public override float FeedbackDuration { get { return ApplyTimeMultiplier(Duration); } set { Duration = value; } }

        /// <summary>
        /// Triggers a chromatic aberration shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            float intensityMultiplier = Timing.ConstantIntensity ? 1f : feedbacksIntensity;
            MMChromaticAberrationShakeEvent.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, intensityMultiplier,
                ChannelData(Channel), ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, Timing.TimescaleMode);
        }

        /// <summary>
        /// On stop we stop our transition
        /// </summary>
        /// <param name="position"></param>
        /// <param name="feedbacksIntensity"></param>
        protected override void CustomStopFeedback(Vector3 position, float feedbacksIntensity = 1)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            base.CustomStopFeedback(position, feedbacksIntensity);
            MMChromaticAberrationShakeEvent.Trigger(Intensity, FeedbackDuration, RemapIntensityZero, RemapIntensityOne, RelativeIntensity, stop: true);
        }
    }
}