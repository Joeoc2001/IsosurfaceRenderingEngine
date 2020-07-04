using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using UnityEngine;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Algebra;
using System.Runtime.InteropServices;

public class CubeMarchPriorGenTask : PriorGenTask
{
    private struct FeelerNodeSetJob : IJob
    {
        public FunctionPointer<Equation.ExpressionDelegate> Function;

        public int Resolution;
        public float Delta;
        public float3 Origin;

        [WriteOnly]
        public NativeArray<FeelerNode> Target;

        public void Execute()
        {
            //Equation.ExpressionDelegate expression = Function.Invoke;
            for (int x = 0; x < Resolution; x++)
            {
                for (int y = 0; y < Resolution; y++)
                {
                    for (int z = 0; z < Resolution; z++)
                    {
                        // Construct position
                        float3 position = Origin + (Delta * new float3(x, y, z));

                        // Get value
                        VariableSet variableSet = new VariableSet(position);
                        float value = Function.Invoke(variableSet);

                        // Guard for invalid values
                        if (float.IsNaN(value))
                        {
                            value = 0;
                        }

                        // Place in target array
                        int index = (x * Resolution + y) * Resolution + z;
                        Target[index] = new FeelerNode(position, value);
                    }
                }
            }
        }
    }

    private readonly FeelerNodeSetJob job;
    private readonly CubeMarchedChunk chunk;
    private readonly SDF sdf;

    public CubeMarchPriorGenTask(CubeMarchedChunk chunk, SDF sdf)
    {
        int resolution = (1 << chunk.quality) + 1;
        job = new FeelerNodeSetJob()
        {
            // Get a pointer to the distance function
            Function = new FunctionPointer<Equation.ExpressionDelegate>(Marshal.GetFunctionPointerForDelegate(sdf.Dist)),

            Resolution = resolution,
            Delta = Chunk.SIZE / (1 << chunk.quality),
            Origin = chunk.transform.position,

            Target = new NativeArray<FeelerNode>(resolution * resolution * resolution, Allocator.TempJob)
        };

        this.chunk = chunk;
        this.sdf = sdf;
    }

    public override IPriorGenTaskHandle Schedule()
    {
        JobHandle handle = job.Schedule();
        return new PriorGenTaskJobHandle(handle);
    }

    public override void AfterFinished()
    {
        // Extract completed datas
        FeelerNode[] feelerNodes = new FeelerNode[job.Target.Length];
        job.Target.CopyTo(feelerNodes);
        int resolution = job.Resolution;
        FeelerNodeSet feelerNodeSet = new FeelerNodeSet(resolution, feelerNodes);

        // Clean up
        job.Target.Dispose();

        // Update chunk's mesh
        chunk.GenerateMeshCore(feelerNodeSet, sdf.Grad);

        // TODO: Other nearby chunk transition regions

        // Set chunk meshes
        chunk.MergeAndSetMeshes();
    }
}
