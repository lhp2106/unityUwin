using Spine.Unity;
using UnityEngine;

public class VKSkeletonReload : MonoBehaviour {

    public SkeletonGraphic skeleton;

	// Use this for initialization
	void OnEnable () {
        skeleton.AnimationState.ClearTrack(0);
        skeleton.AnimationState.SetAnimation(0, skeleton.startingAnimation, false);
	}
	
	// Update is called once per frame
    void OnDisable () {
	}
}
