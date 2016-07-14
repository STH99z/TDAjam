using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;
using DxLibDLL;

namespace TDAjam
{
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
        private float pow2(float a)
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
        public Point picCenter { get; set; } = new Point(0, 0);
        /// <summary>
        /// 实体形状枚举型
        /// </summary>
        public enum EntityShape
        {
            round = 0,
            rect = 1
        }
        /// <summary>
        /// 实体的形状
        /// </summary>
        public EntityShape shape { get; set; } = EntityShape.round;
        /// <summary>
        /// 实体的半径。形状为rect时为正方形半边长。
        /// </summary>
        public short radius { get; set; } = 0;
        public DxSprite sprite { get; set; }
        public CollisionTargetType entityType { get; set; } = CollisionTargetType.none;
        public CollisionType collisionType { get; set; } = new CollisionType(CollisionTargetType.none);

    }
    [Serializable]
    internal class Particle : Entity
    {

    }
    [Serializable]
    internal class Bullet : Particle
    {

    }
    [Serializable]
    internal class Breakable : Particle
    {

    }
    [Serializable]
    internal class Creature : Entity
    {

    }
    [Serializable]
    internal class Player : Creature
    {

    }
    [Serializable]
    internal class Mob : Creature
    {

    }
    [Serializable]
    internal class Boss : Mob
    {

    }

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
        public void DrawArea()
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
        public void DrawArea()
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

    public enum CollisionTargetType
    {
        none = 0,
        particle = 1,
        bullet = 2,
        breakable = 4,
        player = 8,
        mob = 16,
        boss = 32
    }
    [Serializable]
    internal class CollisionType
    {
        public byte collisionWith = 0;
        public CollisionType (CollisionTargetType ctt_addable)
        {
            collisionWith = (byte)ctt_addable;
        }
        /// <summary>
        /// 获取或设置是否发生碰撞
        /// </summary>
        /// <param name="ctt">碰撞体类型</param>
        /// <returns></returns>
        public bool this[CollisionTargetType ctt]
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

}
