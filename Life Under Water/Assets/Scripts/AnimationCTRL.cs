using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;

public class AnimationCTRL : MonoBehaviour
{
    public PlayableDirector playableDirector;

    private void Update()
    {
        if (Input.GetKey(KeyCode.Space) == true)
        {
            PlayAnimation();
        }
    }

    public void PlayAnimation () 
    {
        playableDirector.Play();
    }
}
