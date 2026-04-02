using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomeTween
{
    public static partial class Tween 
    { 
        // "Invisible" MonoBehaviour to run coroutines without needing to attach to existing objects
        private class TweenRunner : MonoBehaviour { }
        private static TweenRunner _instance;
        private static TweenRunner Instance
        {
            get
            {
                if (_instance == null)
                {
                    GameObject go = new GameObject("[Tween Runner]");
                    _instance = go.AddComponent<TweenRunner>();
                    UnityEngine.Object.DontDestroyOnLoad(go); // Keeps it alive between scenes
                    go.hideFlags = HideFlags.HideAndDontSave; // Keeps hierarchy clean
                }
                return _instance;
            }
        }
    
        private static readonly Dictionary<(object target, TweenType type), Coroutine> ActiveTweens = new Dictionary<(object target, TweenType type), Coroutine>();
        public static TweenParams Params => new TweenParams();

        #region Internal
        private static Coroutine Run(object target, TweenType type, Action<float> update, TweenParams p)
        {
            Stop(target, type); 
        
            var key = (target, type);
            Coroutine routine = Instance.StartCoroutine(Value(p,
                // Update
                t => {
                    if (target != null && !target.Equals(null))
                        update(t);
                    else
                        Stop(target, type);
                },
                // OnComplete
                () => {
                    ActiveTweens.Remove(key);
                    p.onComplete?.Invoke();
                })
            );

            ActiveTweens[key] = routine;
            return routine;
        }

        private static IEnumerator Value(TweenParams p, Action<float> onUpdate, Action onComplete)
        {
            if (p.delay > 0)
            {
                yield return p.useUnscaledTime ? new WaitForSeconds(p.delay) : new WaitForSecondsRealtime(p.delay);
            }

            var curve = p.curve ?? Linear;
            int completedLoops = 0;
            bool isForward = true;

            while (p.loops == -1 || completedLoops <= p.loops)
            {
                float elapsed = 0f;
                while (elapsed < p.duration)
                {
                    elapsed += p.useUnscaledTime ? Time.unscaledDeltaTime : Time.deltaTime;
                    float t = Mathf.Clamp01(elapsed / p.duration);
                    float evaluatedT = isForward ? t : 1f - t;
                
                    onUpdate(curve.Evaluate(evaluatedT));
                    yield return null;
                }

                onUpdate(curve.Evaluate(isForward ? 1f : 0f));

                if (p.loopType == LoopType.None) break;
                if (p.loopType == LoopType.PingPong)
                {
                    isForward = !isForward;
                    if (isForward) completedLoops++;
                }
                else 
                { 
                    completedLoops++; 
                }

                if (p.loops != -1 && completedLoops > p.loops) break;
            }
            onComplete?.Invoke();
        }
    
        public static Coroutine StartInternalCoroutine(IEnumerator routine)
        {
            return Instance.StartCoroutine(routine);
        }
        #endregion
    }

    public enum RotationMode { Shortest, Euler }

    public class TweenParams
    {
        public float duration = 1f;
        public Action onComplete = null;
        public bool useUnscaledTime = false;
        public bool useLocal = false;
        public AnimationCurve curve = null;
        public Tween.LoopType loopType = Tween.LoopType.None;
        public RotationMode rotationMode = RotationMode.Shortest;
        public int loops = 0;
        public float delay = 0f; 
        public TweenParams SetDuration(float d) { duration = d; return this; }
        public TweenParams SetEase(AnimationCurve c) { curve = c; return this; }
        public TweenParams SetCallback(Action cb) { onComplete = cb; return this; }
        public TweenParams SetLoops(Tween.LoopType type, int count) { loopType = type; loops = count; return this; }
        public TweenParams SetRotationMode(RotationMode mode) { rotationMode = mode; return this; }
        public TweenParams SetUnscaled(bool unscaled) { useUnscaledTime = unscaled; return this; }
        public TweenParams SetDelay(float d) { delay = d; return this; }
        public TweenParams SetLocal(bool local) { useLocal = local; return this; }

        // Static helper for quick access
        public static TweenParams Default => new TweenParams();
    }
}
