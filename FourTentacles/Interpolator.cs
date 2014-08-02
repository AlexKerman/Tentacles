using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace FourTentacles
{
	public class Interpolator
	{
		private DateTime lastUpdated;
		private int timeLeftMs = 0;
		private const int AnimationTime = 300;

		protected float Interpolation;

		public bool Active { get { return timeLeftMs > 0; } }

		protected void StartAnimation()
		{
			lastUpdated = DateTime.Now;
			timeLeftMs = AnimationTime;
		}

		protected void CheckPosition()
		{
			var now = DateTime.Now;
			int interval = (now - lastUpdated).Milliseconds;
			lastUpdated = now;
			if (interval >= timeLeftMs)
			{
				Interpolation = 1.0f;
				timeLeftMs = 0;
				return;
			}
			timeLeftMs -= interval;
			Interpolation = 1.0f - (timeLeftMs / (float)AnimationTime);
		}
	}

	public class RollInterpolator : Interpolator
	{
		private float distanceToTarget;
		private float deltaDistance;

		public void Roll(Vector3 camPos, Vector3 targetPos, float factor)
		{
			float leftDistance = deltaDistance * (1.0f - Interpolation);
			distanceToTarget = (camPos - targetPos).Length;

			deltaDistance = (distanceToTarget + leftDistance) * factor + leftDistance;
			StartAnimation();
		}

		public Vector3 GetCameraPos(Vector3 pos, Vector3 target)
		{
			CheckPosition();
			Vector3 direction = pos - target;
			direction.Normalize();
			return target + direction * (distanceToTarget + deltaDistance * Interpolation);
		}
	}
}
