using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using UnityEngine;

namespace UnityTK.ECS
{
    /// <summary>
    /// Prototype base class for Unity ECS Entities.
    /// Prototypes can be seen as a substitute for unity gameobject prefabs.
    /// 
    /// Prototypes consist of multiple <see cref="ECSPrototypeComponent{TSpawnData}"/> components.
    /// They support inheritance, inherited components will be overridden by components of the same type higher in the hierarchy.
    /// </summary>
    /// <typeparam name="TDataType">The base type of the components that can be added to this prototype.</typeparam>
    /// <typeparam name="TConstructData">A data object or structure to be passed to all prototype datas <see cref="ECSPrototypeComponent{TSpawnData}"/> in order to pass in information to spawn the prototype.</typeparam>
    public class ECSPrototype<TDataType, TConstructData> : ScriptableObject where TDataType : ECSPrototypeComponent<TConstructData>
    {
        /// <summary>
        /// The datas of this entity prototype.
        /// </summary>
        public TDataType[] data;

        /// <summary>
        /// The archetypes dictionary for every entity manager it ever has been evaluated.
        /// This is simply a cache.
        /// </summary>
        private Dictionary<EntityManager, EntityArchetype> archetypes = new Dictionary<EntityManager, EntityArchetype>();

        /// <summary>
        /// Returns the archetype for this entity prototype.
        /// </summary>
        /// <param name="entityManager">The entity manager to retrieve the archetype from.</param>
        /// <returns>The archetype</returns>
        public EntityArchetype GetArchetype(EntityManager entityManager)
        {
            // Try to get a cached archetype
            EntityArchetype archetype;
            if (this.archetypes.TryGetValue(entityManager, out archetype))
                return archetype;

            // Compose component types
            List<ComponentType> list = ListPool<ComponentType>.Get();
            foreach (var d in this.data)
            {
                d.GetComponentTypes(list);
            }
            ListPool<ComponentType>.Return(list);

            // Create and store archetype in cache
            archetype = entityManager.CreateArchetype(list.ToArray());
            this.archetypes.Add(entityManager, archetype);
            return archetype;
        }

        /// <summary>
        /// Constructs this prototype on the specified entity.
        /// </summary>
        /// <param name="entityManager">The entity manager to use to set data to the specified entity.</param>
        /// <param name="entity">The entity where to add data to on the entity manager.</param>
        /// <param name="constructData">The spawn data to be used to construct the entity.</param>
        public void Construct(EntityManager entityManager, Entity entity, TConstructData constructData = default(TConstructData))
        {
            foreach (var d in this.data)
            {
                d.Construct(entityManager, entity, constructData);
            }
        }
    }
}