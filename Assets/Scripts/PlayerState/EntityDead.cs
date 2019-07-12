﻿using UnityEngine;
using System.Collections;

namespace State
{
    [CreateAssetMenu(fileName = "Dead", menuName = "EntityState/Dead")]
    public class EntityDead : AnimatedState
    {
        public override bool OnExit(GameEntity entity, EntityState nextState, EntityStateMachine fsm)
        {
            return false;
        }
    }

}