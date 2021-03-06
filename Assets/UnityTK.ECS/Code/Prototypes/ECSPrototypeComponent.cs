﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.Threading.Tasks;
using Unity.Entities;

namespace UnityTK.ECS
{
    /// <summary>
    /// Components for <see cref="ECSPrototype{TDataType, TConstructData}"/>.
    /// They are defining a set of unity entity data they will add to the entity and initialize its data based upon either pre-defined values or TConstructData.
    /// 
    /// An example for a unity ecs prototype component (for unity's ecs rendering system) could be a mesh renderer which adds:
    /// - TransformMatrix
    /// - MeshInstancedRenderer
    /// 
    /// Using a construction data type defining mesh to be rendered, shadow casting, ... this can be used to very effectively and flexibly group ecs data logically together in prototype components.
    /// Note that grouping is not required, prototype components can add any arbitrary amount of entity data.
    /// </summary>
    /// <typeparam name="TConstructData"><see cref="ECSPrototype{TDataType, TConstructData}"/></typeparam>
    public abstract class ECSPrototypeComponent<TConstructData> : ScriptableObject
    {
        /// <summary>
        /// This method decides whether or not this prototype component will override the other component.
        /// The other component is retrieved from <see cref="ECSPrototype{TDataType, TConstructData, TPrototypeType}.ancestor"/>.
        /// 
        /// This method is called on every component on the prototype for every component on the ancestor.
        /// This method can be overridden for custom behaviour, standard implementation does a type check and overrides other if the type is the same as this component.
        /// </summary>
        /// <param name="other">The component to check overriding for.</param>
        /// <returns>Whether or not this component would replace the specified other component.</returns>
        public virtual bool DoesOverride(ECSPrototypeComponent<TConstructData> other)
        {
            return this.GetType() == other.GetType();
        }

        /// <summary>
        /// Gets the component types this prototype data would add to a unity ecs entity.
        /// Adds the component types to the list.
        /// </summary>
        /// <param name="list">The list to add the component types to.</param>
        public abstract void GetComponentTypes(List<ComponentType> list);

        /// <summary>
        /// <see cref="ECSPrototype{TDataType, TConstructData}.Construct(EntityManager, Entity, TConstructData)"/>, called for every <see cref="ECSPrototype{TDataType, TConstructData}.data"/>
        /// </summary>
        public abstract void Construct(EntityManager entityManager, Entity entity, TConstructData constructData);
    }
}