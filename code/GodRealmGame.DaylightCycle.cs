using System;
using System.Linq;
using Sandbox;

/*
 * Day/night cycle
 * Same cycle rate as Minecraft (approx 20 minutes)
 */
namespace GodRealm
{
	public partial class GodRealmGame
	{
		[Event.Tick.Server]
		public void OnDaylightCycleTick()
		{
			if ( !TimeEnabled )
				return;
			
			float gameSecondsPerRtSecond = 72;

			float gameSecondsPerTick = gameSecondsPerRtSecond / Global.TickRate;
			CurrentTime += TimeSpan.FromSeconds( gameSecondsPerTick );
		}

		private Rotation CurrentSunRotation { get; set; } = Rotation.Identity;

		private TimeSpan currentTime = TimeSpan.FromHours( 8 );
		
		[Net] public string CurrentTimeString { get; set; }

		public TimeSpan CurrentTime
		{
			get { return currentTime; }
			set
			{
				currentTime = value;

				// 
				// Set the sun's rotation accordingly
				//
				{
					var sunEntity = Entity.All.First( e => e.EngineEntityName == "light_environment" );
					float seconds = (float)currentTime.TotalSeconds;
					float degPerSec = 0.0041666667f;

					seconds -= (6 * 60 * 60); // Sunrise/sunet at 6am/6pm

					float calcDeg = seconds * degPerSec;
					CurrentSunRotation = Rotation.FromPitch( calcDeg ) * Rotation.FromYaw( 45 ) * Rotation.FromRoll( 45 );
					sunEntity.Transform = new Transform( sunEntity.Transform.Position, CurrentSunRotation );
				}

				// 
				// Set time string so that the client can see it on the other side of the network
				//
				{
					int hours = CurrentTime.Hours;
					int minutes = CurrentTime.Minutes;
					int seconds = CurrentTime.Seconds;

					CurrentTimeString = $"{hours:D2}:{minutes:D2}:{seconds:D2}";
				}
			}
		}

		[Net] public bool TimeEnabled { get; set; } = true;

		[ServerCmd( "time_enabled" )]
		public static void SetEnabledCmd( bool enabled )
		{
			var instance = (Game.Current as GodRealmGame);
			instance.TimeEnabled = true;
		}

		[ServerCmd( "time_set" )]
		public static void SetTimeCmd( string time )
		{
			var instance = (Game.Current as GodRealmGame);

			TimeSpan targetTime = TimeSpan.Zero;
			if ( time.Equals( "noon", StringComparison.CurrentCultureIgnoreCase ) || 
			     time.Equals( "day", StringComparison.CurrentCultureIgnoreCase ))
			{
				targetTime = new TimeSpan( 12, 00, 00 );
			}
			else if ( time.Equals( "night", StringComparison.CurrentCultureIgnoreCase ) || 
			          time.Equals( "midnight", StringComparison.CurrentCultureIgnoreCase ))
			{
				targetTime = new TimeSpan( 0, 00, 00 );
			}
			else if ( time.Equals( "sunrise", StringComparison.CurrentCultureIgnoreCase ) )
			{
				targetTime = new TimeSpan( 6, 00, 00 );
			}
			else if ( time.Equals( "sunset", StringComparison.CurrentCultureIgnoreCase ) )
			{
				targetTime = new TimeSpan( 18, 00, 00 );
			}
			else
			{
				if ( !TimeSpan.TryParse( time, out targetTime ) )
				{
					Log.Error( $"Couldn't set time {time}" );
					return;
				}
			}

			instance.CurrentTime = targetTime;
		}
	}
}
