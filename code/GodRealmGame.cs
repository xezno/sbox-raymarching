using Sandbox;

namespace GodRealm
{
	[Library( "godrealm" )]
	public partial class GodRealmGame : Sandbox.Game
	{
		public GodRealmGame() { }

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
