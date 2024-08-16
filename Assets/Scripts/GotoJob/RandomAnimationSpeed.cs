using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAnimationSpeed : MonoBehaviour
{
    Animator _anim;
    public Vector2 rangeSpeed = new Vector2(0.5f, 1.5f);
    void Start()
    {
        _anim = GetComponent<Animator>();
        _anim.speed = Random.Range(rangeSpeed.x, rangeSpeed.y);
    }
}
