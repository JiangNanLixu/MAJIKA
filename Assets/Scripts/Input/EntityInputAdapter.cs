﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(EntityController))]
public class EntityInputAdapter : EntityBehaviour<GameEntity>
{

    // Update is called once per frame
    void Update()
    {
        var controller = GetComponent<EntityController>();
        controller.Movement = InputManager.Instance.Movement;
        controller.Jumped = InputManager.Instance.Jumped;
        controller.Climbed = InputManager.Instance.Climbed;
        controller.SkillIndex = InputManager.Instance.GetSkillIndex();
    }
}