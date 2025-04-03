using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : MonoBehaviour
{
    private Animator _Animator;

    private void Start()
    {
        _Animator = GetComponent<Animator>();
    }

    public void SetAnimationIdle()
    {
        _Animator.SetInteger("Animation", 0);
    }

    public void SetAnimationWalk()
    {
        _Animator.SetInteger("Animation", 1);
    }

    public void SetAnimationRun()
    {
        _Animator.SetInteger("Animation", 2);
    }

    public void SetAnimationFire()
    {
        _Animator.SetTrigger("Fire");
    }
    public void SetAnimationReload()
    {
        _Animator.SetTrigger("Reload");
    }
}
