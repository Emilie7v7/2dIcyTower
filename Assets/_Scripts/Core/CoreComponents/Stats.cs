using System;
using _Scripts.CoreSystem.StatSystem;
using UnityEngine;

namespace _Scripts.CoreSystem
{
    public class Stats : CoreComponent
    {
        [field: SerializeField] public Stat Health { get; private set; }

        protected override void Awake()
        {
            base.Awake();
            
            Health.Initialize();
        }
    }
}
