using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Drawing;

namespace TDAjam
{
    /// <summary>
    /// 位置坐标类
    /// </summary>
    [Serializable]
    class Position
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
        public void MoveTo(float x,float y)
        {
            posX = x;
            posY = y;
        }
        /// <summary>
        /// 移动到
        /// </summary>
        /// <param name="p"></param>
        public void MoveTo (PointF p)
        {
            posX = p.X;
            posY = p.Y;
        }
        /// <summary>
        /// 移动到绝对位置
        /// </summary>
        /// <param name="x"></param>
        /// <param name="y"></param>
        public void MoveToAbs(float x,float y)
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
    class Entity
    {
        public Position position { get; set; }
        public Point center { get; set; } = new Point(0, 0);
    }
    [Serializable]
    class Particle : Entity
    {

    }
    [Serializable]
    class Bullet : Particle
    {

    }
    [Serializable]
    class Breakable : Particle
    {

    }
    [Serializable]
    class Creature : Entity
    {

    }
    [Serializable]
    class Player : Creature
    {

    }
    [Serializable]
    class Mob : Creature
    {

    }
    [Serializable]
    class Boss : Mob
    {

    }

    [Serializable]
    class CollisionField { }
    [Serializable]
    class RectCollisionField : CollisionField { }
    [Serializable]
    class RoundCollisionField : CollisionField { }
    [Serializable]
    class FanCollitionField : CollisionField { }
    [Serializable]
    class EllipseCollitionField : CollisionField { }

}
