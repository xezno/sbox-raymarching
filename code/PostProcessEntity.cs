﻿using Sandbox;

namespace GodRealm
{
	internal sealed class PostProcessEntity : RenderEntity
	{
		private Material material = Material.Load( "materials/post_process.vmat" );
		private VertexBuffer vertexBuffer;

		public PostProcessEntity( Vector3 position )
		{
			Position = position;
			vertexBuffer = new();
			vertexBuffer.Init( true );
			Transmit = TransmitType.Always; 

			Vertex[] vertex =
			{
				 new( new Vector3(-1, 1, 1), Vector3.Up, Vector3.Right, new Vector2(0,0) ),
				 new( new Vector3(1, 1, 1), Vector3.Up, Vector3.Right, new Vector2(1,0) ),
				 new( new Vector3(1, -1, 1), Vector3.Up, Vector3.Right, new Vector2(1,1) ),
				 new( new Vector3(-1, -1, 1), Vector3.Up, Vector3.Right, new Vector2(0,1) ),
			};

			vertexBuffer.AddQuad( vertex[3], vertex[2], vertex[1], vertex[0] );
		}

		[Event.Tick]
		public void OnTick()
		{
			// TODO: Is there a better way of doing this?
			Vector3 a = Vector3.One * 16384;
			RenderBounds = new BBox( Local.Pawn.Position - a, Local.Pawn.Position + a );
		}

		public override void DoRender( SceneObject obj )
		{
			Render.CopyFrameBuffer();
			obj.Flags.IsOpaque = false;
			// obj.Flags.IsTranslucent = false;
			obj.Flags.IsDecal = false;
			obj.Flags.OverlayLayer = true;
			obj.Flags.BloomLayer = false;
			obj.Flags.ViewModelLayer = false;
			obj.Flags.SkyBoxLayer = false;
			obj.Flags.NeedsLightProbe = false;
			
			vertexBuffer.Draw( material );
		}
		
		[ClientCmd( "spawn_raymarch_plane" )]
		public static void SpawnRaymarchPlane()
		{
			TraceResult tr = Trace.Ray( Local.Pawn.EyePos, Local.Pawn.EyePos + Local.Pawn.EyeRot.Forward * 512 ).Ignore( Local.Pawn ).Run();
			_ = new PostProcessEntity( tr.EndPos + tr.Normal * 4.0f );
		}
	}
}
