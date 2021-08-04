using Sandbox;

namespace GodRealm
{
	[Library( "godrealm", Title = "Ray Marched 'God Realm' Demo" )]
	public partial class GodRealmGame : Sandbox.Game
	{
		public override void ClientJoined( Client client )
		{
			base.ClientJoined( client );

			var player = new Player();
			client.Pawn = player;
			player.Respawn();
			
			RpcSpawnPostProcessEntity( To.Single( client ) );
		}

		[ClientRpc]
		private void RpcSpawnPostProcessEntity()
		{
			PostProcessEntity.SpawnRaymarchPlane();
		}
	}
}
