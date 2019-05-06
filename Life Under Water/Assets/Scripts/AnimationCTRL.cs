using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationCTRL : Singleton<AnimationCTRL>
{
    public PlayableDirector playableDirector;

    private void Start()
    {
        PlayAnimation();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) == true)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation () 
    {
        playableDirector.Play();
    }

}
