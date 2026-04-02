using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace HomeTween
{
    public class Sequence
    {
        private readonly List<Func<IEnumerator>> _steps = new List<Func<IEnumerator>>();
        private int _loopCount = 0; // 0 means play once, -1 means infinite

        public Coroutine Play() => Tween.RunSequence(_steps, _loopCount);

        public Sequence SetLoops(int loops)
        {
            _loopCount = loops;
            return this;
        }

        public Sequence Append(Func<Coroutine> tweenLauncher)
        {
            _steps.Add(() => Wait(tweenLauncher()));
            return this;
        }

        public Sequence Join(Func<Coroutine> tweenLauncher)
        {
            if (_steps.Count == 0) return Append(tweenLauncher);

            var lastStep = _steps[_steps.Count - 1];
            _steps[_steps.Count - 1] = () => RunParallel(lastStep(), Wait(tweenLauncher()));
            return this;
        }
        public Sequence AppendInterval(float seconds)
        {
            _steps.Add(() => WaitSeconds(seconds));
            return this;
        }

        public Sequence AppendCallback(Action callback)
        {
            _steps.Add(() => ExecuteCallback(callback));
            return this;
        }

        private IEnumerator Wait(Coroutine c) { yield return c; }
        private IEnumerator WaitSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }
        private IEnumerator ExecuteCallback(Action cb)
        {
            cb?.Invoke();
            yield break; // Finish immediately and move to next step
        }

        private IEnumerator RunParallel(IEnumerator a, IEnumerator b)
        {
            Coroutine startA = Tween.StartInternalCoroutine(a);
            Coroutine startB = Tween.StartInternalCoroutine(b);

            // Wait for both to finish
            yield return startA;
            yield return startB;
        }
    }
}