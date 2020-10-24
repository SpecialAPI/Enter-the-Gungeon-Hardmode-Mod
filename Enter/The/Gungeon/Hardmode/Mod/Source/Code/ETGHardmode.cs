using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Brave.BulletScript;
using MonoMod.RuntimeDetour;

namespace EnterTheGungeonHardmode
{
    public class ETGHardmode : ETGModule
    {
        public override void Init()
        {
        }

        public override void Start()
        {
            Hook hook = new Hook(
                typeof(Bullet).GetMethod("Fire", new Type[] { typeof(Offset), typeof(Direction), typeof(Speed), typeof(Bullet) }),
                typeof(ETGHardmode).GetMethod("FireHook")
            );
        }

        public static void FireHook(Action<Bullet, Offset, Direction, Speed, Bullet> orig, Bullet self, Offset offset, Direction direction, Speed speed, Bullet bullet)
        {
            Bullet resultBullet = bullet;
            if(resultBullet == null)
            {
                resultBullet = new HardmodeBigBullet();
            }
            else
            {
                if(!(resultBullet is BigBulletSpawnedBullet))
                {
                    resultBullet = new HardmodeBigBullet(bullet.SuppressVfx, bullet.FirstBulletOfAttack, bullet.ForceBlackBullet);
                }
            }
            if(resultBullet is HardmodeBigBullet && self.BulletBank != null && self.BulletBank.GetBullet("bigBullet") == null)
            {
                self.BulletBank.Bullets.Add(EnemyDatabase.GetOrLoadByGuid("ec6b674e0acd4553b47ee94493d66422").bulletBank.GetBullet("bigBullet"));
            }
            orig(self, offset, direction, speed, resultBullet);
        }

        public override void Exit()
        {
        }

        public delegate void Action<T, T2, T3, T4, T5>(T arg1, T2 arg2, T3 arg3, T4 arg4, T5 arg5);
    }

    public class BigBulletSpawnedBullet : Bullet
    {
        public BigBulletSpawnedBullet(string bankName = null, bool suppressVfx = false, bool firstBulletOfAttack = false, bool forceBlackBullet = false) : base(bankName, suppressVfx, firstBulletOfAttack, forceBlackBullet)
        {
        }
    }

    public class HardmodeBigBullet : Bullet
    {
        public HardmodeBigBullet() : base("bigBullet", false, false, false)
        {
        }

        public HardmodeBigBullet(bool suppressVfx = false, bool firstBulletOfAttack = false, bool forceBlackBullet = false) : base("bigBullet", suppressVfx, firstBulletOfAttack, forceBlackBullet)
        {
        }

        public override void OnBulletDestruction(DestroyType destroyType, SpeculativeRigidbody hitRigidbody, bool preventSpawningProjectiles)
        {
            if (preventSpawningProjectiles)
            {
                return;
            }
            float num = base.RandomAngle();
            float num2 = 11.25f;
            for (int i = 0; i < 32; i++)
            {
                base.Fire(new Direction(num + num2 * (float)i, DirectionType.Absolute, -1f), new Speed(10f, SpeedType.Absolute), new BigBulletSpawnedBullet(null, false, false, false));
            }
        }
    }
}
