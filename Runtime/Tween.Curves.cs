using UnityEngine;

namespace HomeTween
{
    public enum Curve
    {
        Linear,
        EaseIn,
        EaseOut,
        EaseInOut,
        BounceIn,
        BounceOut,
        BounceInOut,
        ElasticIn,
        ElasticOut,
        ElasticInOut
    }

    public static partial class Tween 
    { 
        private static TweenData _settings;
        private static TweenData Settings
        {
            get
            {
                if (_settings == null)
                {
                    _settings = Resources.Load<TweenData>("TweenData");
                    if (_settings == null) Debug.LogError("TweenData asset not found in Resources!");
                }
                return _settings;
            }
        }
        
        public static AnimationCurve Linear => GetCurve("Linear");
        public static AnimationCurve EaseInOut => GetCurve("EaseInOut");
        public static AnimationCurve BackIn => GetCurve("BackIn");
        public static AnimationCurve BackOut => GetCurve("BackOut");
        public static AnimationCurve ElasticOut => GetCurve("ElasticOut");
        public static AnimationCurve CircIn => GetCurve("CircIn");
        public static AnimationCurve ExpoOut => GetCurve("ExpoOut");
        public static AnimationCurve BounceOut => GetCurve("BounceOut");

        public static AnimationCurve GetCurve(string name)
        {
            if (Settings.library == null || Settings.library.Length == 0) 
            {
                Debug.LogWarning("TweenData library is empty or not initialized. Returning Linear as fallback.");
                return AnimationCurve.Linear(0, 0, 1, 1);
            }

            foreach(var item in Settings.library) 
            {
                if(item.name == name)
                {
                    return item.curve;
                }
            }
            Debug.LogWarning($"Curve '{name}' not found in TweenData. Returning Linear as fallback.");
            return AnimationCurve.Linear(0, 0, 1, 1);
        }

        public static AnimationCurve GetCurve(Curve curve)
        {
            return GetCurve(curve.ToString());
        }
    }
}