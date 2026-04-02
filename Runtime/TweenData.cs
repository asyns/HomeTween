using UnityEngine;

namespace HomeTween
{
    [CreateAssetMenu(fileName = "TweenData", menuName = "Tweening/Curve Data")]
    public class TweenData : ScriptableObject
    {
        [Header("🎨 Juice Cheat Sheet")]
        [TextArea(8, 15)] // Min 8 lines, Max 15
        public string notes = 
            "BackOut: Cards flying into hand (Adds weight)\n" +
            "ElasticOut: Buttons or 'New Turn' banners (The Boing)\n" +
            "ExpoOut: Screen fades or camera pans (Snappy)\n" +
            "BounceOut: Cards slammed on a table (Impact)\n" +
            "CircIn: Discarding or 'Sucking' into a hole (Fast Accel)\n" +
            "Step: Retro/Pixel UI jumps (Discrete movement)";

        [Header("📈 Curve Library")]
        public Curve[] library;

        [System.Serializable]
        public struct Curve 
        {
            public string name;
            public AnimationCurve curve;
            public Curve(string name, AnimationCurve curve)
            {
                this.name = name;
                this.curve = curve;
            }
        }

        // This runs when right-clicking > Create or when clicking "Reset" in the inspector
        private void Reset()
        {
            Debug.Log("Resetting TweenData and populating with default curves.");
            PopulateDefaults();
        }

        public void PopulateDefaults()
        {
            library = new Curve[]
            {
                new ("Linear", AnimationCurve.Linear(0, 0, 1, 1)),
                new ("EaseInOut", AnimationCurve.EaseInOut(0, 0, 1, 1)),
                
                // Back (Anticipation/Overshoot)
                new ("BackIn", new AnimationCurve(new Keyframe(0, 0, 0, -1.5f), new Keyframe(1, 1, 3f, 0))),
                new ("BackOut", new AnimationCurve(new Keyframe(0, 0, 0, 3f), new Keyframe(1, 1, -1.5f, 0))),
                
                // Circular (Starts slow, speeds up very fast)
                new ("CircIn", new AnimationCurve(new Keyframe(0, 0, 0, 0.1f), new Keyframe(1, 1, 2.5f, 0))),
                
                // Exponential (Extremely sharp acceleration/deceleration)
                new ("ExpoOut", new AnimationCurve(new Keyframe(0, 0, 0, 5f), new Keyframe(1, 1, 0, 0))),
                
                // Elastic (The "Boing")
                new ("ElasticOut", new AnimationCurve(
                    new Keyframe(0, 0), new Keyframe(0.3f, 1.2f), 
                    new Keyframe(0.6f, 0.9f), new Keyframe(1, 1))),

                // Bounce (Physical impacts)
                new ("BounceOut", new AnimationCurve(
                    new Keyframe(0, 0), 
                    new Keyframe(0.35f, 1f), new Keyframe(0.55f, 0.75f),
                    new Keyframe(0.75f, 1f), new Keyframe(0.88f, 0.93f),
                    new Keyframe(1, 1)))
            };
        }
    }
}