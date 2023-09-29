using System;

namespace InfiniteOdyssey.Behaviors.Camera;

[Flags]
public enum CameraMode
{
    Bounded = 0x01,
    FollowActor = 0x02,
    AllowNudge = 0x04,

    LockX = 0x1000,
    LockY = 0x2000
}