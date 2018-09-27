using System.Collections;
using System.Collections.Generic;
using Unity.Entities;
using Unity.Rendering;
using UnityEngine;
using UnityEngine.Rendering;
using Unity.Transforms;
using UnityTK;
using UnityTK.ECS;

[CreateAssetMenu(fileName = "Example Prototype Renderer", menuName = "UnityTK.ECS/Examples/Prototypes/Example Renderer Component")]
public class ExampleRendererComponent : ExamplePrototyeComponent
{
    public Mesh mesh;
    public Material material;
    public int subMesh;
    public ShadowCastingMode castShadows;
    public bool receiveShadows;

    private MeshInstanceRenderer _sharedRenderer;

    private void OnEnable()
    {
        this._sharedRenderer = new MeshInstanceRenderer()
        {
            castShadows = this.castShadows,
            material = this.material,
            mesh = this.mesh,
            receiveShadows = this.receiveShadows,
            subMesh = this.subMesh
        };
    }

    public override void Construct(EntityManager entityManager, Entity entity, ExampleConstructData constructData)
    {
        entityManager.SetSharedComponentData(entity, _sharedRenderer);
        entityManager.SetComponentData(entity, new Position()
        {
            Value = constructData.position
        });
        entityManager.SetComponentData(entity, new Rotation()
        {
            Value = constructData.rotation
        });
        entityManager.SetComponentData(entity, new Scale()
        {
            Value = constructData.scale
        });
    }

    public override void GetComponentTypes(List<ComponentType> list)
    {
        list.Add(new ComponentType(typeof(MeshInstanceRenderer)));
        list.Add(new ComponentType(typeof(Position)));
        list.Add(new ComponentType(typeof(Rotation)));
        list.Add(new ComponentType(typeof(Scale)));
        list.Add(new ComponentType(typeof(LocalToWorld)));
    }
}
