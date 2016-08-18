using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DxLibDLL;
using System.Runtime.InteropServices;

namespace TDAjam
{
    #region Basic
    /// <summary>
    /// 位置坐标类
    /// </summary>
    [Serializable]
    internal class Position
    {
        /// <summary>
        /// 位置的X坐标
        /// </summary>
        public float posX { get; set; } = 0f;
        /// <summary>
        /// 位置的Y坐标
        /// </summary>
        public float posY { get; set; } = 0f;
        /// <summary>
        /// 父坐标对象
        /// </summary>
        public Position parentPosition = null;
        public Position(float _x, float _y, Position _parentPosition = null)
        {
            posX = _x;
            posY = _y;
            parentPosition = _parentPosition;
        }
        private static float pow2(float a)
        {
            return a * a;
        }
        /// <summary>
        /// 两个Position类相加
        /// </summary>
        /// <param name="p1">Position1，保留parent</param>
        /// <param name="p2">Position2</param>
        /// <returns>相加后的Position，parent保留p1的</returns>
        public static Position operator +(Position p1, Position p2)
        {
            return new Position(p1.posX + p2.posX, p1.posY + p2.posY, p1.parentPosition);
        }
        /// <summary>
        /// 两个Position类相加
        /// </summary>
        /// <param name="p1">Position1，保留parent</param>
        /// <param name="pf2">Pointf类的Position2</param>
        /// <returns></returns>
        public static Position operator +(Position p1, PointF pf2)
        {
            return new Position(p1.posX + pf2.X, p1.posY + pf2.Y, p1.parentPosition);
        }
        /// <summary>
        /// 获取相对位置
        /// </summary>
        /// <returns>PointF类型的位置</returns>
        public PointF toPointF()
        {
            return new PointF(posX, posY);
        }
        /// <summary>
        /// 获取绝对位置
        /// </summary>
        /// <returns>PointF类型的位置</returns>
        public PointF toPointFAbs()
        {
            if (parentPosition == null)
                return new PointF(posX, posY);
            else
            {
                PointF p2 = parentPosition.toPointFAbs();
                return new PointF(posX + p2.X, posY + p2.Y);
            }
        }
        /// <summary>
        /// 获取相对于父对象的距离
        /// </summary>
        /// <param name="p2">位置2</param>
        /// <returns>距离</returns>
        public float getDistance(Position p2)
        {
            return (float)Math.Sqrt(pow2(posX - p2.posX) + pow2(posY - p2.posY));
        }
        /// <summary>
        /// 获取绝对距离
        /// </summary>
        /// <param name="p">位置2</param>
        /// <returns>绝对距离</returns>
        public float getDistanceAbs(Position p)
        {
            PointF p1 = toPointFAbs(), p2 = p.toPointFAbs();
            return (float)Math.Sqrt(pow2(p1.X - p2.X) + pow2(p1.Y - p2.Y));
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void Move(float x, float y)
        {
            posX += x;
            posY += y;
        }
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="p"></param>
        public void Move(PointF p)
        {
            posX += p.X;
            posY += p.Y;
        }
        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveTo(float x, float y)
        {
            posX = x;
            posY = y;
        }
        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="p"></param>
        public void MoveTo(PointF p)
        {
            posX = p.X;
            posY = p.Y;
        }
        /// <summary>
        /// 移动到绝对位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveToAbs(float x, float y)
        {
            PointF p = toPointFAbs();
            Move(x - p.X, y - p.Y);
        }
        /// <summary>
        /// 移动到绝对位置
        /// </summary>
        /// <param name="to"></param>
        public void MoveToAbs(PointF to)
        {
            PointF p = toPointFAbs();
            Move(to.X - p.X, to.Y - p.Y);
        }
        /// <summary>
        /// 转换为字符串
        /// </summary>
        /// <returns>转换后的字符串</returns>
        public override string ToString()
        {
            string s = $"{{{posX},{posY}}}";
            if (parentPosition != null)
                s += $"->{parentPosition.ToString()}";
            return s;
        }
    }

    public enum VectorType
    {
        Vector,
        PolarVector
    }
    /// <summary>
    /// 向量接口
    /// </summary>
    interface IVector
    {
        float X { get; set; }
        float Y { get; set; }
        float Angle { get; set; }
        float Length { get; set; }
        float this[int v] { get; set; }
        VectorType vectorType { get; }
    }
    /// <summary>
    /// 2D向量，用作速度等其他类包含的基础类型
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal class Vector2D : IVector
    {
        private float x = 0f;
        private float y = 0f;
        public float X
        {
            get { return x; }
            set { x = value; }
        }
        public float Y
        {
            get { return y; }
            set { y = value; }
        }
        public float this[int v]
        {
            set
            {
                if (v == 0)
                    X = v;
                else
                    Y = v;
            }
            get
            {
                if (v == 0)
                    return X;
                return Y;
            }
        }
        public float Length
        {
            get
            {
                return (float)Math.Sqrt(X * X + Y * Y);
            }
            set
            {
                float a = Angle;
                X = (float)Math.Cos(a) * value;
                Y = (float)Math.Sin(a) * value;
            }
        }
        public float Angle
        {
            get
            {
                return (float)Math.Atan2(Y, X);
            }
            set
            {
                float l = Length;
                X = (float)Math.Cos(value) * l;
                Y = (float)Math.Sin(value) * l;
            }
        }
        public VectorType vectorType
        {
            get
            {
                return VectorType.Vector;
            }
        }

        public Vector2D() { }
        public Vector2D(float x, float y)
        {
            this.x = x;
            this.y = y;
        }
        public Vector2D(double x, double y)
        {
            this.x = (float)x;
            this.y = (float)y;
        }
        public PolarVector2D ToPolarVector2D()
            => new PolarVector2D(Angle, Length);
        public static Vector2D operator +(Vector2D v1, Vector2D v2)
            => new Vector2D(v1.X + v2.X, v1.Y + v2.Y);
        public static Vector2D operator -(Vector2D v1, Vector2D v2)
            => new Vector2D(v1.X - v2.X, v1.Y - v2.Y);
        public override string ToString()
            => $"{{{X},{Y}}}";
    }
    /// <summary>
    /// 2D向量，极坐标系下
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    internal class PolarVector2D : IVector
    {
        private float length = 0f;
        private float angle = 0f;
        public float X
        {
            get { return (float)Math.Cos(angle) * length; }
            set
            {
                float temp = Y;
                length = (float)Math.Sqrt(value * value + temp * temp);
                angle = (float)Math.Atan2(temp, value);
            }
        }
        public float Y
        {
            get { return (float)Math.Sin(angle) * length; }
            set
            {
                float temp = X;
                length = (float)Math.Sqrt(value * value + temp * temp);
                angle = (float)Math.Atan2(value, temp);
            }
        }
        public float this[int v]
        {
            get
            {
                if (v == 0)
                    return X;
                return Y;
            }
            set
            {
                if (v == 0)
                    X = value;
                else
                    Y = value;
            }
        }
        public float Length
        {
            get { return length; }
            set { length = value; }
        }
        public float Angle
        {
            get { return angle; }
            set { angle = value; }
        }
        public VectorType vectorType
        {
            get
            {
                return VectorType.PolarVector;
            }
        }

        public PolarVector2D() { }
        public PolarVector2D(float angle, float length)
        {
            this.angle = angle;
            this.length = length;
        }
        public Vector2D ToVector2D()
            => new Vector2D(X, Y);
        public static PolarVector2D operator +(PolarVector2D v1, PolarVector2D v2)
            => new PolarVector2D(v1.angle + v2.angle, v1.length + v2.length);
        public static PolarVector2D operator -(PolarVector2D v1, PolarVector2D v2)
            => new PolarVector2D(v1.angle - v2.angle, v1.length - v2.length);
        public override string ToString()
            => $"{{{X},{Y}}}";
    }

    #endregion
    #region  Entity
    /// <summary>
    /// 实体类
    /// </summary>
    [Serializable]
    internal class Entity
    {
        /// <summary>
        /// 实体的位置
        /// </summary>
        public Position position { get; set; }
        /// <summary>
        /// 图片中心相对实体中心的偏移
        /// </summary>
        public Point picCenter { get; set; }
        /// <summary>
        /// 实体形状枚举型
        /// </summary>
        public enum EntityShape
        {
            round = 0,
            rect = 1,
            fan = 2,
            ellipse = 3
        }
        /// <summary>
        /// 实体的形状
        /// </summary>
        public EntityShape shape { get; set; } = EntityShape.round;
        /// <summary>
        /// 实体的半径。形状为rect时为正方形半边长。
        /// </summary>
        public float radius { get; set; } = 0f;
        private DxSprite _sprite;
        /// <summary>
        /// 精灵图
        /// </summary>
        public DxSprite sprite
        {
            get
            {
                return _sprite;
            }
            set
            {
                _sprite = value;
                picCenter = _sprite.center;
            }
        }
        private int _spriteIndex = 0;
        /// <summary>
        /// 精灵图索引
        /// </summary>
        public int spriteIndex
        {
            get
            {
                return _spriteIndex;
            }
            set
            {
                _spriteIndex = value;
            }
        }
        /// <summary>
        /// 这个实体本身是什么碰撞类型
        /// </summary>
        public CollisionTargetEnum entityType { get; set; } = CollisionTargetEnum.none;
        /// <summary>
        /// 这个实体要和其他的什么类型碰撞
        /// </summary>
        public CollisionTarget collisionType { get; set; } = new CollisionTarget(CollisionTargetEnum.none);

        public Entity(Position pos, float rad = 0f)
        {
            position = pos;
            picCenter = new Point(0, 0);
            rad = 0;
        }
        /// <summary>
        /// 默认绘制，使用Sprite的DrawCellSprite
        /// </summary>
        public virtual void Draw()
        {
            PointF pf = position.toPointFAbs();
            if (sprite != null)
            {
                sprite.SetCenter(picCenter.X, picCenter.Y);
                sprite.SetIndex(_spriteIndex);
                sprite.DrawCellSprite((int)pf.X, (int)pf.Y);
            }
            else
                DX.DrawPixel((int)pf.X, (int)pf.Y, (uint)Color.SkyBlue.ToArgb());
        }
    }
    [Serializable]
    internal class Particle : Entity
    {
        /// <summary>
        /// 速度
        /// </summary>
        public IVector velocity { get; set; }
        /// <summary>
        /// 加速度
        /// </summary>
        public IVector acceleration { get; set; }
        public float spriteAngle { get; set; } = 0f;
        /// <summary>
        /// 是否使用DxSingleAnimation类绘制
        /// </summary>
        public bool useAnimation { get; set; }
        /// <summary>
        /// animation对象
        /// </summary>
        public DxSingleAnimation animation { get; set; } = null;

        public Particle(IVector speed, Position pos, float rad = 0f) : base(pos, rad)
        {
            velocity = speed;
            entityType = CollisionTargetEnum.particle;
        }


        /// <summary>
        /// 叠加速度。类似差分机和帧Based原理，改变当前粒子移速。
        /// </summary>
        /// <param name="deltaTime">距离上一次操作的时间差，单位tick</param>
        public void ApplyVelocity(long deltaTime)
        {
            position.Move(velocity.X * deltaTime / 1000000, velocity.Y * deltaTime / 1000000);
        }
        /// <summary>
        /// 叠加速度。类似差分机和帧Based原理，改变当前粒子移速。deltaTime默认为DXcs.deltaTime。
        /// </summary>
        public void ApplyVelocity()
        {
            position.Move(velocity.X * DXcs.deltaTime / 1000000, velocity.Y * DXcs.deltaTime / 1000000);
        }
        /// <summary>
        /// 叠加加速度，与叠加速度同理。
        /// </summary>
        /// <param name="deltaTime">距离上一次操作的时间差，单位tick</param>
        public void ApplyAcceleration(long deltaTime)
        {
            //判断是Vec还是PolVec
            if (acceleration.vectorType == VectorType.Vector)
            {
                velocity.X += acceleration.X * deltaTime / 1000000;
                velocity.Y += acceleration.Y * deltaTime / 1000000;
            }
            else
            {
                velocity.Angle += acceleration.Angle * deltaTime / 1000000;
                velocity.Length += acceleration.Length * deltaTime / 1000000;
            }
        }
        /// <summary>
        /// 叠加加速度，与叠加速度同理。deltaTime默认为DXcs.deltaTime。
        /// </summary>
        public void ApplyAcceleration()
        {
            //判断是Vec还是PolVec
            if (acceleration.vectorType == VectorType.Vector)
            {
                velocity.X += acceleration.X * DXcs.deltaTime / 1000000;
                velocity.Y += acceleration.Y * DXcs.deltaTime / 1000000;
            }
            else
            {
                velocity.Angle += acceleration.Angle * DXcs.deltaTime / 1000000;
                velocity.Length += acceleration.Length * DXcs.deltaTime / 1000000;
            }
        }
        /// <summary>
        /// 绘制
        /// </summary>
        public override void Draw()
        {
            if (useAnimation)
                animation.DrawAnimationFrame((int)position.posX, (int)position.posY);
            //TODO
            else
                sprite?.SetAngle(spriteAngle + velocity.Angle);
            base.Draw();
        }
    }
    [Serializable]
    internal class Bullet : Particle, IDisposable
    {
        /// <summary>
        /// 伤害值，默认1f
        /// </summary>
        public float damage { get; set; } = 1f;
        /// <summary>
        /// 目标分组，默认敌方，2
        /// </summary>
        public int targetGroup { get; set; } = 2;
        /// <summary>
        /// 穿透次数限制，默认1
        /// </summary>
        public int pierceTimes { get; set; } = 1;
        /// <summary>
        /// 已经穿透的次数，默认0
        /// </summary>
        public int pierceCount { get; set; } = 0;
        /// <summary>
        /// 发射者
        /// </summary>
        public Creature launcher { get; }
        public enum BulletType
        {
            normal = 0
            //TODO
        }

        public Bullet(Creature launcher, IVector speed, Position pos, float rad = 0) : base(speed, pos, rad)
        {
            this.launcher = launcher;
            entityType = CollisionTargetEnum.bullet;
        }

        public bool CheckHit(Entity ent)
        {
            CollisionField cf;
            switch (shape)
            {
                case EntityShape.rect:
                    cf = new RectCollisionField(position, radius * 2, radius * 2);
                    break;
                case EntityShape.round:
                    cf = new RoundCollisionField(position, radius);
                    break;
                case EntityShape.fan:
                    cf = new FanCollisionField(position, radius, 0f, 0f);//TODO
                    break;
                case EntityShape.ellipse:
                    cf = new EllipseCollisionField(position);//TODO
                    break;
                default:
                    throw new Exception("not a valid entity shape!");
            }
            return cf.CollideWith(ent);
        }
        public void Hit(Creature _creature)
        {
            //default changes to crt and this
            //TODO

            //raise event
            HitEvent?.Invoke(_creature);

            //dispose
            Dispose();
        }
        public delegate void HitEventHandler(Creature _creature);
        public event HitEventHandler HitEvent;

        public void Dispose()
        {

        }
    }
    [Serializable]
    internal class Breakable : Particle
    {
        public Breakable(Position pos, float rad = 0) : base(new Vector2D(0, 0), pos, rad)
        {
            entityType = CollisionTargetEnum.breakable;
        }
    }
    [Serializable]
    internal class Creature : Entity
    {
        public Creature(Position pos, float rad = 0) : base(pos, rad)
        {
            entityType = CollisionTargetEnum.creature;
            group = 0;
        }

        /// <summary>
        /// 友好分组，同组为friendly，中立为0
        /// </summary>
        public int group { get; set; }
    }
    [Serializable]
    internal class Player : Creature
    {
        public Player(Position pos, float rad = 0) : base(pos, rad)
        {
            entityType = CollisionTargetEnum.player;
            group = 1;
        }
    }
    [Serializable]
    internal class Mob : Creature
    {
        public Mob(Position pos, float rad = 0) : base(pos, rad)
        {
            entityType = CollisionTargetEnum.mob;
            group = 2;
        }
    }
    [Serializable]
    internal class Boss : Mob
    {
        public Boss(Position pos, float rad = 0) : base(pos, rad)
        {
            entityType = CollisionTargetEnum.boss;
        }
    }


    #endregion
    #region Collison
    /// <summary>
    /// 判定域类
    /// </summary>
    [Serializable]
    internal class CollisionField
    {
        /// <summary>
        /// 访问和判定域绑定的位置
        /// </summary>
        public Position position { get; set; }
        /// <summary>
        /// 判定域形状枚举类
        /// </summary>
        public enum FieldShape
        {
            round = 0,
            rect = 1,
            fan = 2,
            ellipse = 3
        }
        /// <summary>
        /// 判定域形状
        /// </summary>
        public FieldShape shape { get; protected set; }
        /// <summary>
        /// 判断是否发生碰撞
        /// </summary>
        /// <param name="ent">需要判断的实体</param>
        /// <returns></returns>
        public virtual bool CollideWith(Entity ent)
        {
            return false;
        }
        internal protected CollisionField(Position pos)
        {
            position = pos;
        }
        ~CollisionField() { }
#if DEBUG
        public virtual void DrawArea() { }
#endif

        /// <summary>
        /// 静态检测判定域和实体是否碰撞的方法
        /// </summary>
        /// <param name="cf">判定域</param>
        /// <param name="ent">实体</param>
        /// <returns></returns>
        public static bool CollideWith(CollisionField cf, Entity ent)
        {
            return cf.CollideWith(ent);
        }
    }
    /// <summary>
    /// 矩形判定域类
    /// </summary>
    [Serializable]
    internal class RectCollisionField : CollisionField
    {
        /// <summary>
        /// 长
        /// </summary>
        public float width
        {
            get { return _width; }
            set
            {
                this._width = value;
            }
        }
        /// <summary>
        /// 高
        /// </summary>
        public float height
        {
            get { return _height; }
            set
            {
                this._height = value;
            }
        }
        /// <summary>
        /// 半长
        /// </summary>
        public float halfWidth
        {
            get { return width / 2; }
            set { width = value * 2; }
        }
        /// <summary>
        /// 半高
        /// </summary>
        public float halfHeight
        {
            get { return height / 2; }
            set { height = value * 2; }
        }
        private float _width, _height;
        /// <summary>
        /// 构造矩形判定域
        /// </summary>
        /// <param name="pos">坐标</param>
        /// <param name="width">长</param>
        /// <param name="height">高</param>
        public RectCollisionField(Position pos, float width, float height) : base(pos)
        {
            shape = FieldShape.round;
            this.width = width;
            this.height = height;
        }
        /// <summary>
        /// 检测是否和实体发生碰撞
        /// </summary>
        /// <param name="ent">实体</param>
        /// <returns>是否碰撞</returns>
        public override bool CollideWith(Entity ent)
        {
            getXY();
            PointF p = ent.position.toPointFAbs();
            return p.X > x1 && p.X < x2 && p.Y > y1 && p.Y < y2;
        }
        private void getXY()
        {
            PointF p = position.toPointFAbs();
            float x1, x2, y1, y2;
            x1 = p.X - halfWidth;
            x2 = x1 + _width;
            y1 = p.Y - halfHeight;
            y2 = y1 + _height;
        }
        private float x1, x2, y1, y2;
#if DEBUG
        public override void DrawArea()
        {
            int a;
            int m = DX.DX_BLENDMODE_ALPHA;
            getXY();
            DX.GetDrawBlendMode(out m, out a);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 128);
            DXcs.DrawBox((int)x1, (int)y1, (int)x2, (int)y2, Color.White, 1);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, a);
        }
#endif
    }
    /// <summary>
    /// 圆形判定域类
    /// </summary>
    [Serializable]
    internal class RoundCollisionField : CollisionField
    {
        /// <summary>
        /// 半径
        /// </summary>
        public float radius { get; set; }
        /// <summary>
        /// 直径
        /// </summary>
        public float diameter
        {
            get { return radius * 2; }
            set { radius = value / 2; }
        }
        public RoundCollisionField(Position pos, float radius) : base(pos)
        {
            this.radius = radius;
        }
        public override bool CollideWith(Entity ent)
        {
            PointF p = ent.position.toPointFAbs();
            PointF c = position.toPointFAbs();
            if (p.X > c.X - radius && p.X < c.X + radius && p.Y > c.Y - radius && p.Y < c.Y + radius)
                if (DXcs.GetDistance(p.X - c.X, p.Y - c.Y) < radius)
                    return true;
            return false;
        }
#if DEBUG
        public override void DrawArea()
        {
            int a;
            int m = DX.DX_BLENDMODE_ALPHA;
            PointF p = position.toPointFAbs();
            DX.GetDrawBlendMode(out m, out a);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 128);
            DX.DrawCircle((int)p.X, (int)p.Y, (int)radius, 0xffffffff);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, a);
        }
#endif
    }
    /// <summary>
    /// 扇形判定域类
    /// </summary>
    [Serializable]
    internal class FanCollisionField : CollisionField
    {
        /// <summary>
        /// 扇形中心指向角度
        /// </summary>
        public float towards
        {
            get { return _towards; }
            set
            {
                _towards = value;
                aleft = _towards - _spread;
                aright = _towards + _spread;
            }
        }
        /// <summary>
        /// 从toward向左右打开的角度
        /// </summary>
        public float spread
        {
            get { return _spread; }
            set
            {
                _spread = value;
                aleft = _towards - _spread;
                aright = _towards + _spread;
            }
        }
        /// <summary>
        /// 半径
        /// </summary>
        public float radius { get; set; }
        private float _towards = 0f, _spread = 0f;
        private float aleft, aright;
        public FanCollisionField(Position pos, float radius, float towards, float spread) : base(pos)
        {
            this.radius = radius;
            this.towards = towards;
            this.spread = spread;
        }
        public override bool CollideWith(Entity ent)
        {
            PointF p = ent.position.toPointFAbs();
            PointF c = position.toPointFAbs();
            float dist = DXcs.GetDistance(p.X - c.X, p.Y - c.Y);
            float ang = DXcs.GetTowards(c, p);
            return DXcs.AngleInRange(ang, aleft, aright) && dist < radius;
        }
    }
    /// <summary>
    /// 椭圆形判定域类
    /// </summary>
    [Serializable]
    internal class EllipseCollisionField : CollisionField
    {
        //WIP
        public EllipseCollisionField(Position pos) : base(pos)
        {
        }
    }

    public enum CollisionTargetEnum
    {
        none = 0,
        particle = 1,
        bullet = 2,
        breakable = 4,
        player = 8,
        mob = 16,
        boss = 32,
        creature = 64
    }
    [Serializable]
    internal class CollisionTarget
    {
        public byte collisionWith = 0;
        /// <summary>
        /// 构造一个碰撞目标
        /// </summary>
        /// <param name="ctt_addable">将要判定碰撞的对象种类，可用|连接多个</param>
        public CollisionTarget(CollisionTargetEnum ctt_addable)
        {
            collisionWith = (byte)ctt_addable;
            if (collisionWith > 127)
                throw new Exception($"构造碰撞目标出错，当前collisionWith值为{collisionWith}。");
        }
        /// <summary>
        /// 获取或设置是否发生碰撞
        /// </summary>
        /// <param name="ctt">碰撞体类型</param>
        /// <returns></returns>
        public bool this[CollisionTargetEnum ctt]
        {
            set
            {
                byte t = (byte)~(byte)ctt;
                collisionWith &= t;
                if (value)
                    collisionWith += (byte)ctt;
            }
            get
            {
                return (collisionWith & (byte)ctt) > 0;
            }
        }
    }

    #endregion
    #region Map
    /// <summary>
    /// 地图类
    /// </summary>
    [Serializable]
    internal class Map
    {
        public TileSets tileSets { get; set; } = null;
        public Size mapSize { get; set; } = new Size(1, 1);
        public int mapWidth => mapSize.Width;
        public int mapHeight => mapSize.Height;
        public int layerCount => layers.Count;
        public List<MapLayer> layers { get; private set; } = null;

        public Map(TileSets tileSets, Size mapSize)
        {
            this.tileSets = tileSets;
            this.mapSize = mapSize;
        }

        public bool addLayer(MapLayer layer)
        {
            try
            {
                layers.Add(layer);
            }
            catch(Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message);
                return false;
            }
            return true;
        }
    }
    /// <summary>
    /// 地图层
    /// </summary>
    [Serializable]
    internal class MapLayer
    {
        public Map mapRef { get; } = null;
        public int layerID { get; private set; } = 0;
        public List<Tile> tileData { get; set; } = new List<Tile>();

        public MapLayer(ref Map mapRef)
        {
            this.mapRef = mapRef;
            this.layerID = mapRef.layerCount;
            mapRef.addLayer(this);
        }
    }
    /// <summary>
    /// 地图块
    /// </summary>
    [Serializable]
    internal class Tile
    {
        public enum TileShape
        {
            rectangle = 0,
            circle,
            outerCircleLT,
            outerCircleRT,
            outerCircleLB,
            outerCircleRB,
            innerCircleLT,
            innerCircleRT,
            innerCircleLB,
            innerCircleRB,
            slopeLT,
            slopeRT,
            slopeLB,
            slopeRB
        }
        /// <summary>
        /// 形状
        /// </summary>
        public TileShape shape { get; set; } = TileShape.rectangle;
        /// <summary>
        /// 碰撞对象，默认是墙体，所以全碰撞
        /// </summary>
        public CollisionTarget collisionTarget { get; set; } = new CollisionTarget(
            CollisionTargetEnum.particle | CollisionTargetEnum.bullet |
            CollisionTargetEnum.creature | CollisionTargetEnum.player |
            CollisionTargetEnum.mob | CollisionTargetEnum.boss);
        /// <summary>
        /// 组内ID
        /// </summary>
        public int IDinSets { get; private set; } = 0;
        public TileSets setsRef { get; set; } = null;

        public Tile(int IDinSets, TileSets setsRef)
        {
            this.IDinSets = IDinSets;
            this.setsRef = setsRef;
        }
        public void Draw(int x, int y)
        {
            setsRef.setsSprite.SetIndex(IDinSets);
            setsRef.setsSprite.DrawCellSprite(x, y);
        }
#if DEBUG
        public void DrawCollisionTarget(int x, int y)
        {
            DXcs.DrawText(x, y, collisionTarget.collisionWith.ToString(), Color.White);
        }
#endif

        public static bool isTileShapeRegular(TileShape shape)
        {
            return (byte)shape < 2;
        }
        public static bool isTileShapeQuarterCircle(TileShape shape)
        {
            return (byte)shape > 1;
        }
        public static bool isTileShapeInnerCircle(TileShape shape)
        {
            return (byte)shape > 1 && (byte)shape < 6;
        }
        public static bool isTileShapeOuterCircle(TileShape shape)
        {
            return (byte)shape > 5 && (byte)shape < 10;
        }
        public static bool isTileShapeSlope(TileShape shape)
        {
            return (byte)shape > 9;
        }

    }
    /// <summary>
    /// 地图块元数据组
    /// </summary>
    [Serializable]
    internal class TileSets
    {
        public DxSprite setsSprite { get; private set; } = null;
        public int setsRow => setsSprite.sliceCountY;
        public int setsColumm => setsSprite.sliceCountX;
        public List<Tile> tiles { get; set; } = new List<Tile>();

        public TileSets(DxSprite setsSprite)
        {
            this.setsSprite = setsSprite;
            tiles.Clear();
            for (int i = 0; i < setsSprite.cellCount; i++)
            {
                Tile t = new Tile(i, this);
                tiles.Add(t);
            }
            if (tiles.Count != setsSprite.cellCount)
                throw new Exception("tileSets初始化错误");
        }
    }
    #endregion
}
