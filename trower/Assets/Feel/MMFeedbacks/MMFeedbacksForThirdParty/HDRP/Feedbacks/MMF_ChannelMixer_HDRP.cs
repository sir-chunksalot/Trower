﻿using MoreMountains.Feedbacks;
using UnityEngine;
namespace MoreMountains.FeedbacksForThirdParty
{
    /// <summary>	
    /// This feedback allows you to control channel mixer's red, green and blue over time.	
    /// It requires you have in your scene an object with a Volume 	
    /// with Channel Mixer active, and a MMChannelMixerShaker_HDRP component.	
    /// </summary>	
    [AddComponentMenu("")]
#if MM_HDRP
	[FeedbackPath("PostProcess/Channel Mixer HDRP")]
#endif
    [FeedbackHelp("This feedback allows you to control channel mixer's red, green and blue over time." +
                  "It requires you have in your scene an object with a Volume" +
                  "with Channel Mixer active, and a MM Channel Mixer HDRP component.")]
    public class MMF_ChannelMixer_HDRP : MMF_Feedback
    {
        /// a static bool used to disable all feedbacks of this type at once	
        public static bool FeedbackTypeAuthorized = true;
        /// sets the inspector color for this feedback	
#if UNITY_EDITOR
        public override Color FeedbackColor { get { return MMFeedbacksInspectorColors.PostProcessColor; } }
#endif
        /// the duration of this feedback is the duration of the shake	
        public override float FeedbackDuration { get { return ApplyTimeMultiplier(ShakeDuration); } set { ShakeDuration = value; } }
        public override bool HasChannel => true;
        public override bool HasRandomness => true;

        [MMFInspectorGroup("Color Grading", true, 10)]
        /// the duration of the shake, in seconds
        [Tooltip("the duration of the shake, in seconds")]
        public float ShakeDuration = 1f;
        /// whether or not to add to the initial intensity
        [Tooltip("whether or not to add to the initial intensity")]
        public bool RelativeIntensity = true;
        /// whether or not to reset shaker values after shake
        [Tooltip("whether or not to reset shaker values after shake")]
        public bool ResetShakerValuesAfterShake = true;
        /// whether or not to reset the target's values after shake
        [Tooltip("whether or not to reset the target's values after shake")]
        public bool ResetTargetValuesAfterShake = true;

        [MMFInspectorGroup("Red", true, 13)]
        /// the curve used to animate the red value on
        [Tooltip("the curve used to animate the red value on")]
        public AnimationCurve ShakeRed = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(-200f, 200f)]
        public float RemapRedZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(-200f, 200f)]
        public float RemapRedOne = 200f;

        [MMFInspectorGroup("Green", true, 12)]
        /// the curve used to animate the green value on
        [Tooltip("the curve used to animate the green value on")]
        public AnimationCurve ShakeGreen = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(-200f, 200f)]
        public float RemapGreenZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(-200f, 200f)]
        public float RemapGreenOne = 200f;

        [MMFInspectorGroup("Blue", true, 11)]
        /// the curve used to animate the blue value on
        [Tooltip("the curve used to animate the blue value on")]
        public AnimationCurve ShakeBlue = new AnimationCurve(new Keyframe(0, 0), new Keyframe(0.5f, 1), new Keyframe(1, 0));
        /// the value to remap the curve's 0 to
        [Tooltip("the value to remap the curve's 0 to")]
        [Range(-200f, 200f)]
        public float RemapBlueZero = 0f;
        /// the value to remap the curve's 1 to
        [Tooltip("the value to remap the curve's 1 to")]
        [Range(-200f, 200f)]
        public float RemapBlueOne = 200f;

        /// <summary>
        /// Triggers a color adjustments shake
        /// </summary>
        /// <param name="position"></param>
        /// <param name="attenuation"></param>
        protected override void CustomPlayFeedback(Vector3 position, float feedbacksIntensity = 1.0f)
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }
            float intensityMultiplier = ComputeIntensity(feedbacksIntensity, position);
            MMChannelMixerShakeEvent_HDRP.Trigger(ShakeRed, RemapRedZero, RemapRedOne,
                ShakeGreen, RemapGreenZero, RemapGreenOne,
                ShakeBlue, RemapBlueZero, RemapBlueOne,
                FeedbackDuration,
                RelativeIntensity, intensityMultiplier, ChannelData, ResetShakerValuesAfterShake, ResetTargetValuesAfterShake, NormalPlayDirection, ComputedTimescaleMode);
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

            MMChannelMixerShakeEvent_HDRP.Trigger(ShakeRed, RemapRedZero, RemapRedOne,
                ShakeGreen, RemapGreenZero, RemapGreenOne,
                ShakeBlue, RemapBlueZero, RemapBlueOne,
                FeedbackDuration,
                RelativeIntensity, channelData: ChannelData, stop: true);
        }

        /// <summary>
        /// On restore, we put our object back at its initial position
        /// </summary>
        protected override void CustomRestoreInitialValues()
        {
            if (!Active || !FeedbackTypeAuthorized)
            {
                return;
            }

            MMChannelMixerShakeEvent_HDRP.Trigger(ShakeRed, RemapRedZero, RemapRedOne,
                ShakeGreen, RemapGreenZero, RemapGreenOne,
                ShakeBlue, RemapBlueZero, RemapBlueOne,
                FeedbackDuration,
                RelativeIntensity, channelData: ChannelData, restore: true);
        }
    }
}