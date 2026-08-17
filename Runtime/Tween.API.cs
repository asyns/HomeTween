using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace HomeTween
{
    public static partial class Tween
    {
        public enum TweenType { Move, Scale, Resize, Rotate, Fade, Volume, Colour, OffsetMin, OffsetMax }
        public enum LoopType { None, Restart, PingPong }

        #region Sequences
        public static Sequence CreateSequence() => new Sequence();
        public static Coroutine RunSequence(List<Func<IEnumerator>> steps, int loops)
        {
            return Instance.StartCoroutine(SequenceProcessor(steps, loops));
        }

        public static void StopSequence(Coroutine sequenceJob)
        {
            if (sequenceJob != null)
            {
                Instance.StopCoroutine(sequenceJob);
            }
        }

        private static IEnumerator SequenceProcessor(List<Func<IEnumerator>> steps, int loops)
        {
            int completedLoops = 0;
            while (true)
            {
                foreach (var step in steps)
                {
                    yield return step();
                }
                // If loops is -1, we never break (infinite)
                if (loops == -1) continue;
                // If we've finished the requested number of loops, break
                // Note: loops = 0 means it runs exactly once (the foreach above)
                if (completedLoops >= loops) break;
                completedLoops++;
            }
        }
        #endregion

        #region Stop Tweens
        // Helper to stop any existing tween on a target
        public static void Stop(object target, TweenType type)
        {
            var key = (target, type);
            if (ActiveTweens.TryGetValue(key, out Coroutine active))
            {
                if (active != null) Instance.StopCoroutine(active);
                ActiveTweens.Remove(key);
            }
        }

        public static void StopAll(object target)
        {
            List<(object, TweenType)> keysToRemove = new List<(object, TweenType)>();
            foreach (var key in ActiveTweens.Keys)
            {
                if (key.target == target) keysToRemove.Add(key);
            }

            foreach (var key in keysToRemove)
            {
                Instance.StopCoroutine(ActiveTweens[key]);
                ActiveTweens.Remove(key);
            }
        }
        #endregion

        #region Transform Shortcuts
        // --- Move Shortcuts ---
        public static Coroutine Move(this Transform t, Vector3 target, float duration, AnimationCurve curve = null)
            => t.Move(target, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine MoveArch(this Transform t, Vector3 end, float height, float duration, AnimationCurve curve = null)
            => t.MoveArch(end, height, new TweenParams().SetDuration(duration).SetEase(curve));

        // --- Scale & Rotate Shortcuts ---
        public static Coroutine Resize(this RectTransform t, Vector2 target, float duration, AnimationCurve curve = null)
            => t.Resize(target, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine MoveAnchorMin(this RectTransform t, Vector2 target, float duration, AnimationCurve curve = null)
            => t.MoveAnchorMin(target, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine MoveAnchorMax(this RectTransform t, Vector2 target, float duration, AnimationCurve curve = null)
            => t.MoveAnchorMax(target, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine MoveOffsetMin(this RectTransform t, Vector2 target, float duration, AnimationCurve curve = null)
            => t.MoveOffsetMin(target, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine MoveOffsetMax(this RectTransform t, Vector2 target, float duration, AnimationCurve curve = null)
            => t.MoveOffsetMax(target, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine Scale(this Transform t, Vector3 to, float duration, AnimationCurve curve = null)
            => t.Scale(to, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine Rotate(this Transform t, Quaternion to, float duration, AnimationCurve curve = null)
            => t.Rotate(to, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine Spin(this Transform t, Vector3 axis, float duration, AnimationCurve curve = null)
            => t.Spin(axis, new TweenParams().SetDuration(duration).SetEase(curve));
        #endregion

        #region Other Shortcuts
        public static Coroutine Fade(this CanvasGroup group, float to, float duration, AnimationCurve curve = null)
            => group.Fade(to, new TweenParams().SetDuration(duration).SetEase(curve).SetUnscaled(true));

        public static Coroutine Volume(this AudioSource source, float to, float duration, AnimationCurve curve = null)
            => source.Volume(to, new TweenParams().SetDuration(duration).SetEase(curve));

        public static Coroutine Colour(this Graphic graphic, Color to, float duration, AnimationCurve curve = null)
            => graphic.Colour(to, new TweenParams().SetDuration(duration).SetEase(curve));
        #endregion


        #region Transform with Params
        public static Coroutine MoveAnchorMin(this RectTransform target, Vector2 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector2 from = target.anchorMin;
            return Run(target, TweenType.Move, t =>
            {
                Vector2 newAnchor = Vector2.Lerp(from, to, t);
                target.anchorMin = newAnchor;
            }, p);
        }

        public static Coroutine MoveAnchorMax(this RectTransform target, Vector2 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector2 from = target.anchorMax;
            return Run(target, TweenType.Move, t =>
            {
                Vector2 newAnchor = Vector2.Lerp(from, to, t);
                target.anchorMax = newAnchor;
            }, p);
        }

        public static Coroutine MoveOffsetMin(this RectTransform target, Vector2 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector2 from = target.offsetMin;
            return Run(target, TweenType.OffsetMin, t =>
            {
                Vector2 newOffset = Vector2.Lerp(from, to, t);
                target.offsetMin = newOffset;
            }, p);
        }

        public static Coroutine MoveOffsetMax(this RectTransform target, Vector2 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector2 from = target.offsetMax;
            return Run(target, TweenType.OffsetMax, t =>
            {
                Vector2 newOffset = Vector2.Lerp(from, to, t);
                target.offsetMax = newOffset;
            }, p);
        }

        public static Coroutine Colour(this Graphic graphic, Color to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Color from = graphic.color;
            return Run(graphic, TweenType.Colour, t => graphic.color = Color.Lerp(from, to, t), p);
        }

        public static Coroutine Resize(this RectTransform target, Vector2 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector2 from = target.sizeDelta;
            return Run(target, TweenType.Resize, t =>
            {
                target.sizeDelta = Vector2.Lerp(from, to, t);
            }, p);
        }


        public static Coroutine Move(this Transform target, Vector3 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector3 start = target.GetPosition(p.movementMode);
            return Run(target, TweenType.Move, (tValue) =>
            {
                Vector3 position = Vector3.Lerp(start, to, tValue);
                target.SetPosition(position, p.movementMode);
            }, p);
        }

        public static Coroutine MoveCurve(this Transform target, Vector3 end, Vector3 control, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector3 start = target.GetPosition(p.movementMode);
            return Run(target, TweenType.Move, (tValue) =>
            {
                float invT = 1f - tValue;
                Vector3 position = invT * invT * start + 2f * invT * tValue * control + tValue * tValue * end;
                target.SetPosition(position, p.movementMode);
            }, p);
        }

        public static Coroutine MoveArch(this Transform target, Vector3 end, float archHeight, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector3 start = target.GetPosition(p.movementMode);
            Vector3 midPoint = (start + end) / 2f;
            Vector3 controlPoint = midPoint + (Vector3.up * archHeight);
            return target.MoveCurve(end, controlPoint, p);
        }

        public static Coroutine Scale(this Transform target, Vector3 to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector3 from = target.localScale;
            return Run(target, TweenType.Scale, t => target.localScale = Vector3.Lerp(from, to, t), p);
        }

        public static Coroutine Rotate(this Transform target, Vector3 toEuler, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Quaternion from = target.localRotation;
            Quaternion to = Quaternion.Euler(toEuler);
            return Run(target, TweenType.Rotate, t => target.localRotation = Quaternion.Slerp(from, to, t), p);
        }

        public static Coroutine Rotate(this Transform target, Quaternion to, TweenParams p = null)
        {
            p ??= TweenParams.Default;

            // Shortest Path (Default)
            if (p.rotationMode == RotationMode.Shortest)
            {
                Quaternion from = p.useLocal ? target.localRotation : target.rotation;
                return Run(target, TweenType.Rotate, t =>
                {
                    if (p.useLocal) target.localRotation = Quaternion.Slerp(from, to, t);
                    else target.rotation = Quaternion.Slerp(from, to, t);
                }, p);
            }

            // Long Path / Specific Path (Euler)
            Vector3 startEuler = p.useLocal ? target.localEulerAngles : target.eulerAngles;
            Vector3 endEuler = to.eulerAngles;

            return Run(target, TweenType.Rotate, t =>
            {
                Vector3 current = Vector3.Lerp(startEuler, endEuler, t);
                if (p.useLocal) target.localEulerAngles = current;
                else target.eulerAngles = current;
            }, p);
        }

        public static Coroutine RotateDirectional(this Transform target, Vector3 toEuler, bool clockwise, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector3 startEuler = target.localEulerAngles;
            Vector3 finalEuler = toEuler;

            if (startEuler == finalEuler) return null; // No rotation needed

            // Force the rotation to continue in the direction of the spin
            for (int i = 0; i < 3; i++) // Check X, Y, and Z axes
            {
                float start = startEuler[i];
                float end = finalEuler[i];

                if (clockwise)
                {
                    // If we want to go clockwise, end must be lower than start
                    while (end > start) end -= 360f;
                }
                else
                {
                    // If we want to go counter-clockwise (your Spin axis), end must be higher
                    while (end < start) end += 360f;
                }
                finalEuler[i] = end;
            }

            return Run(target, TweenType.Rotate, t =>
            {
                target.localEulerAngles = Vector3.Lerp(startEuler, finalEuler, t);
            }, p);
        }

        public static Coroutine Spin(this Transform target, Vector3 axis, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            Vector3 startRotation = target.localEulerAngles;

            return Run(target, TweenType.Rotate, t =>
            {
                target.localEulerAngles = startRotation + (axis * (t * 360f));
            }, p);
        }
        #endregion
        #region UI & Audio with Params
        public static Coroutine Fade(this CanvasGroup group, float to, TweenParams p = null)
        {
            // Default UI to unscaled time if not explicitly set
            p ??= new TweenParams().SetUnscaled(true);
            float from = group.alpha;
            return Run(group, TweenType.Fade, t => group.alpha = Mathf.Lerp(from, to, t), p);
        }

        public static Coroutine Volume(this AudioSource source, float to, TweenParams p = null)
        {
            p ??= TweenParams.Default;
            float from = source.volume;
            return Run(source, TweenType.Volume, t => source.volume = Mathf.Lerp(from, to, t), p);
        }
        #endregion
    }
}