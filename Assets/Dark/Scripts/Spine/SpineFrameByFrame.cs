using UnityEngine;
using Spine.Unity;
using Spine; // for Physics enum

[RequireComponent(typeof(SkeletonAnimation))]
public class SpineFrameByFrame : MonoBehaviour {
    public int targetFps = 12;       // desired stepping FPS
    private float frameInterval;
    private float accumulator;

    private SkeletonAnimation skeletonAnim;

    void Awake() {
        skeletonAnim = GetComponent<SkeletonAnimation>();

        // turn off automatic updating so we can drive it manually
        skeletonAnim.UpdateMode = UpdateMode.Nothing;

        frameInterval = 1f / targetFps;
    }

    void Update() {
        accumulator += Time.deltaTime;
        bool stepped = false;

        while (accumulator >= frameInterval) {
            // step animation
            skeletonAnim.AnimationState.Update(frameInterval);
            accumulator -= frameInterval;
            stepped = true;
        }

        if (stepped) {
            // apply animation state to skeleton
            skeletonAnim.AnimationState.Apply(skeletonAnim.Skeleton);

            // update world transforms with new runtime API
            skeletonAnim.Skeleton.UpdateWorldTransform(Skeleton.Physics.None);

            // rebuild the mesh for rendering
            skeletonAnim.LateUpdate();
        }
    }
}