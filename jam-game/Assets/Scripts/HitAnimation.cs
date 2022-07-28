using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitAnimation : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    private Material defMaterial;
    [SerializeField]
    private Material whiteMaterial;

    [SerializeField] 
    private float duration = 1.0f;
    
    [SerializeField]
    private AnimationCurve hitAnimation;

    private bool playing = false;
    
    [SerializeField]
    private bool playEditor = false;
    
    private float playStartTimestamp = -1.0f;

    public void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        defMaterial = spriteRenderer.material;
    }

    public void Play()
    {
        playing = true;
        playStartTimestamp = Time.time;
    }

    private void Update()
    {
        if (playEditor)
        {
            Play();
            playEditor = false;
        }
        
        if (playing)
        {
            float currentTime = Mathf.Clamp((Time.time - playStartTimestamp) / duration, 0.0f, 1.0f);
            
            float progress = hitAnimation.Evaluate(currentTime);
            bool showWhite = Mathf.RoundToInt(progress) == 1;
            
            spriteRenderer.material = showWhite ? whiteMaterial : defMaterial;
            if (currentTime == 1.0f)
            {
                playing = false;
            }
        }
    }
}
