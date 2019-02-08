﻿using Unity.Jobs;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;

namespace Deform.Masking
{
	[Deformer (Name = "Mask State", Description = "Stores the current state of the mesh for masks to interpolate towards.", Type = typeof (MaskState), Category = Category.Mask)]
	public class MaskState : Deformer
	{
		public override int BatchCount => 256;
		public override DataFlags DataFlags => DataFlags.Vertices | DataFlags.MaskVertices;

		public override JobHandle Process (MeshData data, JobHandle dependency = default)
		{
			return new MaskStateJob
			{
				currentVertices = data.DynamicNative.VertexBuffer,
				maskVertices = data.DynamicNative.MaskVertexBuffer
			}.Schedule (data.Length, BatchCount, dependency);
		}

		[BurstCompile (CompileSynchronously = COMPILE_SYNCHRONOUSLY)]
		private struct MaskStateJob : IJobParallelFor
		{
			[ReadOnly]
			public NativeArray<float3> currentVertices;
			[WriteOnly]
			public NativeArray<float3> maskVertices;

			public void Execute (int index)
			{
				maskVertices[index] = currentVertices[index];
			}
		}
	}
}