using System;

namespace CPI311.GameEngine
{
    public class GameConstants
    {
        //Player constants
        public const float PlayerSpeed = 2000.0f;
        public const float PlayerRotateSpeed = 3.0f;

        //Object constants
        public const int NumAsteroids = 2;
        public const int NumBullets = NumAsteroids * 2;

        public const float AsteroidMinSpeed = 1;
        public const float AsteroidMaxSpeed = 100;

        //Other stuff
        public const int BulletSpeedAdjustment = 3;
        public const int ShotPenalty = 0;
        public const int KillBonus = 5;

        //Playfield
        public const int PlayfieldSizeX = 3000;
        public const int PlayfieldSizeY = 2500;
    }
}
