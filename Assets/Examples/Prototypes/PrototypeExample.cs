using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Entities;

public class PrototypeExample : MonoBehaviour
{
    public ExampleConstructData constructData;
    public ExamplePrototye prototype;

    public void Awake()
    {
        var em = World.Active.GetOrCreateManager<EntityManager>();
        var entity = this.prototype.Construct(em, this.constructData);
    }
}