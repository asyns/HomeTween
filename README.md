```markdown
# HomeTween

A high-performance, zero-boilerplate tweening engine for Unity. Designed for professional workflows: install via UPM and run instantly—no manual scene setup or manager placement required.

---

## 📦 Installation

Install via the **Unity Package Manager** (UPM) using the Git URL:

1. Open `Window > Package Manager` in Unity.
2. Click the **+** icon (top left) and select **Add package from git URL...**
3. Paste the following:
   `git@github.com:asyns/HomeTween.git`

---

## ⚡ Quick Start

HomeTween is completely self-managed. The `TweenManager` is handled internally by the package; you never need to manually add a component to your scene.

### Simple Animations
```csharp
using HomeTween;

// Move to a world position over 1.5 seconds
transform.Move(new Vector3(10, 0, 5), 1.5f);

// Fade a UI CanvasGroup (Defaults to unscaled time)
myCanvasGroup.Fade(0f, 0.5f);

// 3D Arch movement (jumps 5 units high)
transform.MoveArch(targetPos, 5f, 2f);
```

### Advanced Control with `Tween.Params`
No need to instantiate `new TweenParams()`. Use the static `Tween.Params` entry point to chain your settings cleanly:

```csharp
transform.Scale(Vector3.one * 2f, Tween.Params
    .SetDuration(0.8f)
    .SetEase(myAnimationCurve)
    .SetLoop(3, Tween.LoopType.PingPong)
    .SetUnscaled(true)
    .SetLocal(true));
```

---

## ⛓ Sequences

The `Sequence` API allows you to chain animations linearly or run them in parallel with zero complexity.

### Basic Sequence
```csharp
Tween.CreateSequence()
    .Append(() => transform.Move(Vector3.up, 1f))
    .AppendInterval(0.5f) // Wait half a second
    .Append(() => transform.Spin(Vector3.forward, 0.5f))
    .AppendCallback(() => Debug.Log("Sequence Complete!"))
    .Play();
```

### Parallel Execution (Join)
Use `.Join()` to play a tween at the same time as the previous `.Append()`. The sequence will wait for **both** to finish before moving to the next step.
```csharp
Tween.CreateSequence()
    .Append(() => transform.Move(Vector3.right, 2f)) 
    .Join(() => otherTransform.Fade(0f, 0.5f)) // Starts same time as Move
    .Append(() => transform.Scale(Vector3.zero, 1f)) // Waits for Move AND Fade
    .Play();
```

### Looping Sequences
```csharp
Tween.CreateSequence()
    .Append(() => transform.Rotate(Vector3.up * 90, 0.5f))
    .SetLoops(-1) // Infinite loops
    .Play();
```

### Sequence Lifecycle
.Play() returns a Coroutine. You can capture this to manually stop the sequence later.
```csharp
Coroutine mySequence = Tween.CreateSequence()
    .Append(() => transform.Move(Vector3.up,   1f))
    .Append(() => transform.Move(Vector3.down, 1f))
    .SetLoops(-1)
    .Play();

// Later, stop the sequence manually
Tween.StopSequence(mySequence);
```

---

## 🛠 Feature Breakdown

### Transform Extensions
* **Move**: Interpolate position or localPosition.
* **MoveCurve / MoveArch**: Quadratic Bezier paths for organic, non-linear movement.
* **Rotate**: Choose between `ShortestPath` (Slerp) or `Euler` (Specific degree paths) via parameters.
* **Spin**: Rapid shorthand for a 360-degree rotation on a specific axis.
* **Scale**: Tween local scale values.

### UI & Audio
* **Fade**: Direct `CanvasGroup` alpha control (optimized for UI).
* **Volume**: Smoothly transition `AudioSource` volume.

### Conflict Resolution
HomeTween prevents "tween fighting." If you start a `Move` on a Transform that is already moving, the library automatically kills the previous coroutine before starting the new one.

---

## 🛑 Manual Termination

If you want to stop a tween manually:

```csharp
// Stop only the movement on this specific target
Tween.Stop(transform, Tween.TweenType.Move);

// Kill every active HomeTween operation on this target
Tween.StopAll(transform);
```

---

## ⚙️ TweenParams API
The `.Params` helper allows you to configure:
* `.SetDuration(float)`
* `.SetEase(AnimationCurve)`
* `.SetCallback(Action)`
* `.SetLoops(int count, LoopType type)`
* `.SetRotationMode(RotationMode)`: `Shortest` or `Euler`.
* `.SetUnscaled(bool)`: Independent of `Time.timeScale`.
* `.SetDelay(float)`
* `.SetLocal(bool)`: Toggle between World and Local space.
```