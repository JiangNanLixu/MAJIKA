﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ContactEffectTrigger : MonoBehaviour, IEffectorTrigger
{
    [MAJIKA.Utils.StatusEffect]
    public List<EffectInstance> Effects = new List<EffectInstance>();
    List<GameEntity> contactList = new List<GameEntity>();
    // Use this for initialization
    void Start()
    {

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        var entity = collision.rigidbody.GetComponent<GameEntity>();
        if (entity && !contactList.Contains(entity))
            contactList.Add(entity);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        var entity = collision.rigidbody.GetComponent<GameEntity>();
        if (entity)
            contactList.Remove(entity);
    }

    // Update is called once per frame
    void Update()
    {
        Effects.ForEach(effect =>
            contactList.ForEach(entity =>
                entity.GetComponent<EntityEffector>()
                    .AddEffect(effect.Clone(), this)));
    }
}
