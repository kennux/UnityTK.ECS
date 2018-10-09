using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
    /// <see cref="ECSPrototypeComponent{TConstructData}.DoesOverride(ECSPrototypeComponent{TConstructData})"/>
    /// </summary>
    /// <typeparam name="TComponentType">The base type of the components that can be added to this prototype.</typeparam>
    /// <typeparam name="TConstructData">A data object or structure to be passed to all prototype datas <see cref="ECSPrototypeComponent{TSpawnData}"/> in order to pass in information to spawn the prototype.</typeparam>
    public class ECSPrototype<TComponentType, TConstructData, TPrototypeType> : ScriptableObject where TComponentType : ECSPrototypeComponent<TConstructData> where TPrototypeType : ECSPrototype<TComponentType, TConstructData, TPrototypeType>
    {
        /// <summary>
        /// The ancestor of this prototype.
        /// May be null.
        /// </summary>
        [Header("Data and inheritance")]
        [SerializeField]
        private TPrototypeType ancestor;

        /// <summary>
        /// The datas of this entity prototype.
        /// </summary>
        [SerializeField]
        private List<TComponentType> components;

        /// <summary>
        /// The runtime data evaluated by <see cref="components"/> and <see cref="ancestor"/>
        /// </summary>
        private List<TComponentType> initializedComponents;

        /// <summary>
        /// Whether or not this prototype was initialized yet.
        /// </summary>
        private bool wasInitialized = false;

        /// <summary>
        /// The archetypes dictionary for every entity manager it ever has been evaluated.
        /// This is simply a cache.
        /// </summary>
        private Dictionary<EntityManager, EntityArchetype> archetypes = new Dictionary<EntityManager, EntityArchetype>();

        /// <summary>
        /// Sets <see cref="ancestor"/> and <see cref="components"/>.
        /// Resets <see cref="wasInitialized"/>.
        /// 
        /// This can be used to create prototypes at runtime.
        /// </summary>
        /// <param name="components">The components to set to this prototype instance. The list reference is kept internally!</param>
        /// <param name="ancestor">The ancestor of this prototype, may be null</param>
        public void Setup(List<TComponentType> components, TPrototypeType ancestor)
        {
            this.components = components;
            this.ancestor = ancestor;
        }

        /// <summary>
        /// Resets state.
        /// </summary>
        private void OnEnable()
        {
            this.initializedComponents = null;
            this.wasInitialized = false;
        }

        /// <summary>
        /// Sets up <see cref="initializedComponents"/>.
        /// Will run warm up of this prototype, will be automatically called if not warmed up yet on every prototype method.
        /// </summary>
        public void Initialize()
        {
            // Ancestor set?
            if (Essentials.UnityIsNull(this.ancestor))
            {
                this.initializedComponents = this.components;
                this.wasInitialized = true;
                return;
            }

            // Warmup ancestor
            this.ancestor.Initialize();

            // Do inheritance magic :>
            List<TComponentType> datas = new List<TComponentType>();

            // Iterate over every ancestor data
            foreach (var c in this.ancestor.initializedComponents)
            {
                bool overridden = false;
                foreach (var component in this.components)
                {
                    if (component.DoesOverride(c))
                    {
                        // Override
                        datas.Add(component);
                        overridden = true;
                        break;
                    }
                }

                if (!overridden)
                {
                    datas.Add(c); // Ancestor component is used
                }
            }

            // Write state
            this.initializedComponents = datas;
            this.wasInitialized = true;
        }

        /// <summary>
        /// Calls <see cref="Initialize"/>, only when <see cref="wasInitialized"/> is false.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void _Initialize()
        {
            if (this.wasInitialized)
                return;

            this.Initialize();
        }

        /// <summary>
        /// Returns the archetype for this entity prototype.
        /// </summary>
        /// <param name="entityManager">The entity manager to retrieve the archetype from.</param>
        /// <returns>The archetype</returns>
        public EntityArchetype GetArchetype(EntityManager entityManager)
        {
            _Initialize();

            // Try to get a cached archetype
            EntityArchetype archetype;
            if (this.archetypes.TryGetValue(entityManager, out archetype))
                return archetype;

            // Compose component types
            List<ComponentType> list = ListPool<ComponentType>.Get();
            foreach (var d in this.initializedComponents)
            {
                d.GetComponentTypes(list);
            }

            // Create and store archetype in cache
            archetype = entityManager.CreateArchetype(list.ToArray());
            ListPool<ComponentType>.Return(list);
            this.archetypes.Add(entityManager, archetype);
            return archetype;
        }

        /// <summary>
        /// Tries getting a component of the specified type.
        /// </summary>
        /// <typeparam name="T">The type of the component to look for.</typeparam>
        /// <param name="component">The component, default(T) if not existing.</param>
        /// <returns>Whether or not the component was found. If this is true, component parameter is set.</returns>
        public bool TryGetComponent<T>(out T component) where T : TComponentType
        {
            foreach (var comp in this.components)
                if (comp is T)
                {
                    component = comp as T;
                    return true;
                }

            component = default(T);
            return false;
        }

        /// <summary>
        /// Constructs this prototype on the specified entity.
        /// </summary>
        /// <param name="entityManager">The entity manager to use to set data to the specified entity.</param>
        /// <param name="entity">The entity where to add data to on the entity manager.</param>
        /// <param name="constructData">The spawn data to be used to construct the entity.</param>
        public void Construct(EntityManager entityManager, Entity entity, TConstructData constructData = default(TConstructData))
        {
            _Initialize();

            foreach (var d in this.initializedComponents)
            {
                d.Construct(entityManager, entity, constructData);
            }
        }

        /// <summary>
        /// "Simple" construct method constructing the entity on itself.
        /// <see cref="Construct(EntityManager, Entity, TConstructData)"/>
        /// </summary>
        /// <param name="entityManager">The entity manager to create the entity on.</param>
        /// <returns>The created and constructed entity.</returns>
        public Entity Construct(EntityManager entityManager, TConstructData constructData = default(TConstructData))
        {
            var entity = entityManager.CreateEntity(GetArchetype(entityManager));
            Construct(entityManager, entity, constructData);

            return entity;
        }
    }
}