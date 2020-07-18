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
    private readonly FeelerNodeSetJob job;
    private readonly CubeMarchedChunk chunk;
    private readonly SDF sdf;

    public CubeMarchPriorGenTask(CubeMarchedChunk chunk, SDF sdf)
    {
        int edgesPerEdge = 1 << chunk.Quality;
        int verticesPerEdge = edgesPerEdge + 1;
        job = new FeelerNodeSetJob()
        {
            // Get a pointer to the distance function
            Function = new FunctionPointer<Equation.ExpressionDelegate>(Marshal.GetFunctionPointerForDelegate(sdf.Dist)),

            Resolution = verticesPerEdge,
            Delta = Chunk.SIZE / edgesPerEdge,
            Origin = float3.zero,
            SamplingOffset = chunk.transform.position,

            Target = new NativeArray<FeelerNode>(verticesPerEdge * verticesPerEdge * verticesPerEdge, Allocator.TempJob)
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
        chunk.GenerateMesh(feelerNodeSet, sdf.Grad);

        // TODO: Other nearby chunk transition regions

        // Set chunk meshes
        chunk.MergeAndSetMeshes();
    }
}
