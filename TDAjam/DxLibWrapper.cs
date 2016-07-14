using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using System.Drawing;

namespace TDAjam
{
    [Serializable]
    internal class DGparams
    {
        /// <summary>
        /// 0=DrawGraph,1=DrawRotaGraph3,2=DrawRectRotaGraph3
        /// </summary>
        public byte method = 0;
        /// <summary>
        /// Image handle
        /// </summary>
        public int handle = 0;
        /// <summary>
        /// Z-place
        /// </summary>
        public float z = 0f;
        /// <summary>
        /// destnation
        /// </summary>
        public int destX = 0, destY = 0;
        /// <summary>
        /// source
        /// </summary>
        public int srcX = 0, srcY = 0, srcW = 0, srcH = 0;
        /// <summary>
        /// center
        /// </summary>
        public int centX = 0, centY = 0;
        /// <summary>
        /// scale
        /// </summary>
        public double scaleX = 1, scaleY = 1;
        /// <summary>
        /// angle
        /// </summary>
        public double angle = 0;
        public DGparams(int handle, float z,
            int x, int y)
        {
            method = 0;
            this.handle = handle;
            this.z = z;
            destX = x;
            destY = y;
        }
        public DGparams(
            int handle, float z, int x, int y,
            int centerX, int centerY, double scaleX, double scaleY, double angle)
        {
            method = 1;
            this.handle = handle;
            this.z = z;
            destX = x;
            destY = y;
            centX = centerX;
            centY = centerY;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.angle = angle;
        }
        public DGparams(
            int handle, float z,
            int x, int y,
            int srcX, int srcY, int srcW, int srcH,
            int centerX, int centerY,
            double scaleX, double scaleY, double angle)
        {
            method = 2;
            this.handle = handle;
            this.z = z;
            destX = x;
            destY = y;
            centX = centerX;
            centY = centerY;
            this.srcX = srcX;
            this.srcY = srcY;
            this.srcW = srcW;
            this.srcH = srcH;
            this.scaleX = scaleX;
            this.scaleY = scaleY;
            this.angle = angle;
        }
    }

    /// <summary>
    /// Single wrapper for image handle.
    /// </summary>
    [Serializable]
    internal class DxImage
    {
        public int handle;
        public int width, height;
        public string uri;

        /// <summary>
        /// Constructor of DxImage.
        /// </summary>
        /// <param name="path">Related path of a image</param>
        public DxImage(string path)
        {
            uri = path;
            handle = DX.LoadGraph(path);
            if (handle == -1)
            {
                System.Windows.Forms.MessageBox.Show("DX.LoadGraph() Failed.\nPath = " + path);
                width = -1;
                height = -1;
                return;
            }
            else
            {
                DXcs.AddGraphHandle(handle);
                DX.SetDeviceLostDeleteGraphFlag(handle, 0);
                DX.GetGraphSize(handle, out width, out height);
            }
        }
        public DxImage(DxImage dxi)
        {
            handle = dxi.handle;
            width = dxi.width;
            height = dxi.height;
            uri = dxi.uri;
        }
        public DxImage(int handle)
        {
            this.handle = handle;
            if (handle == -1)
            {
                System.Windows.Forms.MessageBox.Show("Copy DxImage Failed.");
                width = -1;
                height = -1;
                return;
            }
            else
            {
                DX.SetDeviceLostDeleteGraphFlag(handle, 0);
                DX.GetGraphSize(handle, out width, out height);
            }
        }
        /// <summary>
        /// When the whole program shuts and the collection of DxImage disposes, call this function.
        /// </summary>
        public void UnloadImage()
        {
            if (isLoaded)
            {
                DX.DeleteGraph(handle, 1);
            }
        }
        public void LoadImage(string path)
        {
            int h = DX.LoadGraph(path);
            if (h == -1)
            {
                System.Windows.Forms.MessageBox.Show("Dx LoadGraph Failed\nPath=" + path + "\nGraph discards the change.");
                return;
            }
            handle = h;
            DXcs.AddGraphHandle(handle);
            DX.GetGraphSize(handle, out width, out height);
        }
        public bool isLoaded
        {
            get { return handle != -1; }
        }
        public void DrawImage(int x, int y, int transFlag = 1)
        {
            if (!DxLayer.LayerOpened)
                DX.DrawGraph(x, y, handle, transFlag);
            else
                DxLayer.list.Add(new DGparams(handle, DxLayer.Zplace, x, y));
        }
        public void DrawImageAdv(int x, int y, int centerX, int centerY, double scaleX, double scaleY, double angle)
        {
            if (!DxLayer.LayerOpened)
                DX.DrawRotaGraph3(x, y, centerX, centerY, scaleX, scaleY, angle, handle, 1);
            else
                DxLayer.list.Add(new DGparams(handle, DxLayer.Zplace, x, y, centerX, centerY, scaleX, scaleY, angle));
        }
        public void DrawImageClipAdv(int x, int y, int srcX, int srcY, int srcW, int srcH, int centerX, int centerY, double scaleX, double scaleY, double angle)
        {
            if (!DxLayer.LayerOpened)
                DX.DrawRectRotaGraph3(x, y, srcX, srcY, srcW, srcH, centerX, centerY, scaleX, scaleY, angle, handle, 1, 0);
            else
                DxLayer.list.Add(new DGparams(handle, DxLayer.Zplace, x, y, srcX, srcY, srcW, srcH, centerX, centerY, scaleX, scaleY, angle));
        }

    }
    /// <summary>
    /// Sprite, combined advanced 2D draw methods of a single image.
    /// Only contains changable drawing arguments.
    /// Use SingleAnimation or Animation class for procedural changle usage.
    /// </summary>
    [Serializable]
    internal class DxSprite
    {
        public DxImage image;
        /// <summary>
        /// Shows the slice count of the image.Init when constructed.
        /// </summary>
        public int sliceCountX { get; private set; }
        /// <summary>
        /// Shows the slice count of the image.Init when constructed.
        /// </summary>
        public int sliceCountY { get; private set; }
        /// <summary>
        /// Shows the width and height of a single cell after sliced.
        /// </summary>
        public float cellW { get; private set; }
        /// <summary>
        /// Shows the width and height of a single cell after sliced.
        /// </summary>
        public float cellH { get; private set; }
        /// <summary>
        /// Shows the Scale.Default is 1.
        /// </summary>
        public float scaleX { get; set; }
        /// <summary>
        /// Shows the Scale.Default is 1.
        /// </summary>
        public float scaleY { get; set; }
        /// <summary>
        /// Rotation angle.
        /// </summary>
        public float angle { get; set; }
        /// <summary>
        /// Drawing center offset, offset the destX destY.
        /// </summary>
        public int centerX { get; set; }
        /// <summary>
        /// Drawing center offset, offset the destX destY.
        /// </summary>
        public int centerY { get; set; }
        public PointF scale
        {
            get { return new PointF(scaleX, scaleY); }
        }
        public PointF slice
        {
            get { return new PointF(sliceCountX, sliceCountY); }
        }
        public Point center
        {
            get { return new Point(centerX, centerY); }
        }
        public Point cellPosition
        {
            get { return new Point(cellX, cellY); }
        }
        public SizeF cellSize
        {
            get { return new SizeF(cellW, cellH); }
        }
        public int cellCount
        {
            get { return sliceCountX * sliceCountY; }
        }
        public int cellIndex { get; private set; }
        /// <summary>
        /// Return the x pos of the current indexed cell.
        /// </summary>
        public int cellX
        {
            get { return (int)(cellW * (cellIndex % sliceCountX)); }
        }
        /// <summary>
        /// Return the y pos of the current indexed cell.
        /// </summary>
        public int cellY
        {
            get { return (int)(cellH * (cellIndex / sliceCountX)); }
        }
        /// <summary>
        /// Constructor of DxSprite
        /// </summary>
        /// <param name="img">DxImage Object</param>
        /// <param name="slicex">x slice count</param>
        /// <param name="slicey">y slice count</param>
        /// <param name="scalex">x scale, default is 1</param>
        /// <param name="scaley">y scale, default is 1</param>
        /// <param name="angle">rotation angle, default is 0</param>
        public DxSprite(DxImage img, int slicex = 1, int slicey = 1, float scalex = 1f, float scaley = 1f, float angle = 0f, bool centered = true)
        {
            image = img;
            SetSlice(slicex, slicey);
            SetScale(scalex, scaley);
            SetAngle(angle);
            CalcCellSize();
            if (centered) SetCenter2CellCenter();
            else SetCenter(0, 0);
        }
        private void CalcCellSize()
        {
            cellW = (float)image.width / sliceCountX;
            cellH = (float)image.height / sliceCountY;
        }
        public void SetScale(float sX, float sY)
        {
            scaleX = sX;
            scaleY = sY;
        }
        public void SetSlice(int sX, int sY)
        {
            sliceCountX = sX;
            sliceCountY = sY;
        }
        public void SetAngle(float ang)
        {
            angle = ang;
        }
        public void SetCenter(int cX, int cY)
        {
            centerX = cX;
            centerY = cY;
        }
        /// <summary>
        /// Set center to cellpos / 2.
        /// </summary>
        public void SetCenter2CellCenter()
        {
            SetCenter((int)(cellW / 2), (int)(cellH / 2));
        }
        /// <summary>
        /// Set the index of the sliced image.
        /// </summary>
        /// <param name="idx">cell index</param>
        /// <param name="forceset">Set index to (idx % cellCount) if set to true.</param>
        /// <returns>false if idx > cellCount</returns>
        public bool SetIndex(int idx, bool forceset = true)
        {
            if (forceset)
            {
                cellIndex = idx % cellCount;
                return true;
            }
            if (idx < cellCount)
            {
                cellIndex = idx;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Normal drawing with scale and rotation.
        /// </summary>
        /// <param name="destX">destination x</param>
        /// <param name="destY">destination y</param>
        public void DrawSprite(int destX, int destY)
        {
            image.DrawImageAdv(destX, destY, centerX, centerY, scaleX, scaleY, angle);
        }
        /// <summary>
        /// Adv drawing with slice and normal drawing arguments.
        /// </summary>
        /// <param name="destX">destination x</param>
        /// <param name="destY">destination y</param>
        public void DrawCellSprite(int destX, int destY)
        {
            image.DrawImageClipAdv(
                destX, destY,
                cellX, cellY,
                (int)cellW, (int)cellH,
                centerX, centerY,
                scaleX, scaleY,
                angle);
        }

    }
    /// <summary>
    /// Arrangement of arguments in DxSprite.
    /// </summary>
    [Serializable]
    internal class DxSingleAnimation
    {
        /// <summary>
        /// 变换信息类
        /// </summary>
        public class TransformInfo
        {
            /// <summary>
            /// 插值枚举型
            /// </summary>
            public enum InterpolationMethod
            {
                Nearest,
                Linear,
                Sin,
                Cos,
                Quadratic,
                Cubic
            };
            public float value, start, end;
            /// <summary>
            /// from 0 to 1
            /// </summary>
            public double currentPos
            {
                get { return curpos; }
                set
                {
                    ApplyTransformAbsolute(value);
                    curpos = value;
                }
            }
            private double curpos;
            public InterpolationMethod interpolationMethod { get; set; }
            public string tag { get; set; }

            public TransformInfo(float value, InterpolationMethod method = InterpolationMethod.Linear, string Tag = "")
            {
                this.value = value;
                this.start = value;
                this.end = value;
                CalcCurrentPos();
                this.interpolationMethod = method;
                this.tag = Tag;
            }
            public TransformInfo(float value, float start, float end, InterpolationMethod method, string Tag = "")
            {
                this.value = value;
                this.start = start;
                this.end = end;
                CalcCurrentPos();
                this.interpolationMethod = method;
                this.tag = Tag;
            }
            /// <summary>
            /// 计算当前变换过程的完成度，构造的时候调用
            /// </summary>
            private void CalcCurrentPos()
            {
                curpos = DXcs.Scale(value, start, end, 0, 1);
            }
            /// <summary>
            /// 应用变换
            /// </summary>
            /// <param name="percentage">变换完成度增加量</param>
            /// <returns></returns>
            public float ApplyTransform(double percentage)
            {
                curpos += percentage;
                if (curpos > 1) curpos -= (int)curpos;
                switch (interpolationMethod)
                {
                    case InterpolationMethod.Nearest:
                        if (curpos < 0.5) value = start;
                        else value = end;
                        break;
                    case InterpolationMethod.Linear:
                        value = (float)DXcs.Scale(curpos, 0, 1, start, end);
                        break;
                    case InterpolationMethod.Quadratic:
                        value = (float)DXcs.Scale(curpos * curpos, 0, 1, start, end);
                        break;
                    case InterpolationMethod.Cubic:
                        value = (float)DXcs.Scale(curpos * curpos * curpos, 0, 1, start, end);
                        break;
                    case InterpolationMethod.Sin:
                        value = (float)((DXcs.SinD(curpos - 0.5, 2, 1) + 0.5) * (end - start) + start);
                        break;
                    case InterpolationMethod.Cos:
                        value = (float)((DXcs.CosD(curpos, 2, 1) + 0.5) * (end - start) + start);
                        break;
                }
                return value;
            }
            /// <summary>
            /// 应用变换
            /// </summary>
            /// <param name="percentage">变换完成度</param>
            /// <returns></returns>
            public float ApplyTransformAbsolute(double percentage)
            {
                curpos = percentage;
                if (curpos > 1)
                    curpos -= (int)curpos;
                switch (interpolationMethod)
                {
                    case InterpolationMethod.Nearest:
                        if (curpos < 0.5) value = start;
                        else value = end;
                        break;
                    case InterpolationMethod.Linear:
                        value = (float)DXcs.Scale(curpos, 0, 1, start, end);
                        break;
                    case InterpolationMethod.Quadratic:
                        value = (float)DXcs.Scale(curpos * curpos, 0, 1, start, end);
                        break;
                    case InterpolationMethod.Cubic:
                        value = (float)DXcs.Scale(curpos * curpos * curpos, 0, 1, start, end);
                        break;
                    case InterpolationMethod.Sin:
                        value = (float)((DXcs.SinD(curpos - 0.5, 2, 1) + 0.5) * (end - start) + start);
                        break;
                    case InterpolationMethod.Cos:
                        value = (float)((DXcs.CosD(curpos, 2, 1) + 0.5) * (end - start) + start);
                        break;
                }
                return value;
            }
        }
        /// <summary>
        /// 变换对象枚举型
        /// </summary>
        public enum TransformItem
        {
            scaleX, scaleY, angle, centerX, centerY, posX, posY
        }

        /// <summary>
        /// 精灵
        /// </summary>
        public DxSprite sprite;
        /// <summary>
        /// 是否启用索引映射
        /// </summary>
        public bool useIndexMapping { get; set; }
        /// <summary>
        /// 索引映射表，数组下表对应Sprite的cellIndex
        /// </summary>
        public ushort[] indexMap { get; set; }
        /// <summary>
        /// 当前索引
        /// </summary>
        public ushort index { get; set; }
        public uint frameTime { get; set; }
        public uint animationTime { get; set; }
        public long startTime { get; private set; }
        public bool usePositionControl { get; set; }
        public PointF position { get; set; }
        public PointF positionStart { get; set; }
        public PointF positionEnd { get; set; }
        public TransformInfo posXTransform, posYTransform;
        public TransformInfo[] transform { get; private set; }
        public bool[] useTransform { get; set; }

        /// <summary>
        /// Constructor of DxSingleAnimation.
        /// </summary>
        /// <param name="dxs"></param>
        /// <param name="animationTime"></param>
        /// <param name="indexMapping"></param>
        /// <param name="positionControl"></param>
        public DxSingleAnimation(ref DxSprite dxs, float animationTime, bool indexMapping = false, bool positionControl = false)
        {
            sprite = dxs;
            transform = new TransformInfo[5]
            {
                new TransformInfo (sprite.scaleX),
                new TransformInfo(sprite.scaleY),
                new TransformInfo(sprite.angle),
                new TransformInfo(sprite.centerX),
                new TransformInfo (sprite.centerY)
            };
            useTransform = new bool[5] { false, false, false, false, false };
            this.useIndexMapping = indexMapping;
            this.usePositionControl = positionControl;
            this.index = 0;
            this.indexMap = new ushort[0];
            this.animationTime = (uint)(1000 * animationTime);
            SetStartTime();
        }
        public void SetStartTime()
        {
            this.startTime = DateTime.Now.Ticks;
        }
        public void SetIndexMap(ushort[] map)
        {
            this.indexMap = map;
        }
        public void SetPosTransform(PointF startP, PointF endP, TransformInfo.InterpolationMethod method)
        {
            usePositionControl = true;
            positionStart = startP;
            positionEnd = endP;
            posXTransform = new TransformInfo(positionStart.X, positionStart.X, positionEnd.X, method);
            posYTransform = new TransformInfo(positionStart.Y, positionStart.Y, positionEnd.Y, method);
        }
        public void ApplyTransform()
        {
            long timeNow = DXcs.nowTime;
            for (int i = 0; i < 5; i++)
            {
                if (useTransform[i])
                {
                    transform[i].ApplyTransformAbsolute((double)(timeNow - startTime) / (animationTime * 10000));
                }
            }
            if (useTransform[0])
                sprite.scaleX = transform[0].value;
            if (useTransform[1])
                sprite.scaleY = transform[1].value;
            if (useTransform[2])
                sprite.angle = transform[2].value;
            if (useTransform[3])
                sprite.centerX = (int)transform[3].value;
            if (useTransform[4])
                sprite.centerY = (int)transform[4].value;
            if (usePositionControl)
            {
                position = new PointF(
                    posXTransform.ApplyTransformAbsolute((double)(timeNow - startTime) / (animationTime * 10000)),
                    posYTransform.ApplyTransformAbsolute((double)(timeNow - startTime) / (animationTime * 10000))
                    );
            }
            if (useIndexMapping)
            {
                sprite.SetIndex(indexMap[((timeNow - startTime) / (frameTime * 10000)) % (uint)indexMap.Length]);
            }
        }
        public void SetTransform(TransformItem item, float start, float end, TransformInfo.InterpolationMethod method)
        {
            if (item == TransformItem.posX)
            {
                System.Windows.Forms.MessageBox.Show("请使用SetPosTransform");
                return;
            }
            if (item == TransformItem.posY)
            {
                System.Windows.Forms.MessageBox.Show("请使用SetPosTransform");
                return;
            }
            useTransform[(byte)item] = true;
            transform[(byte)item].start = start;
            transform[(byte)item].end = end;
            transform[(byte)item].interpolationMethod = method;
        }
        public void DrawAnimationFrame()
        {
            DrawAnimationFrame((int)position.X, (int)position.Y);
        }
        public void DrawAnimationFrame(int destX, int destY)
        {
            if (sprite.cellCount > 1)
                sprite.DrawCellSprite(destX, destY);
            else
                sprite.DrawSprite(destX, destY);
        }
    }
    /// <summary>
    /// DXcsharp wrapper class.
    /// </summary>
    internal static class DXcs
    {
        private const bool DEBUGMODE = true;
        private const int use3Dmode = 0;
        public const float fpsLimit = 120f;
        public static Random rnd;
        public static int frmWidth, frmHeight;
        public static int resWidth, resHeight;
        public static long nowTime;
        public static long deltaTime;
        public static Size[] frmSize;
        public static int defaultSizeIndex = 6;
        public static int centerX
        {
            get { return resWidth / 2; }
        }
        public static int centerY
        {
            get { return resHeight / 2; }
        }
        public static Point center
        {
            get { return new Point(centerX, centerY); }
        }
        public static int fontHeight { get; private set; }
        public static int fontWidth { get; private set; }
        private static int _debugDrawIndex = 0;
        private static long _oneFrameCalcTimeBuf;
        public static long oneFrameCalcTime { get; private set; }

        private static readonly HashSet<int> graphHandleSet;

        static DXcs()
        {
            rnd = new Random();
            frmSize = new Size[]
            {
                new Size(640,360),  //nHD
                new Size(720,405),
                new Size(800,450),
                new Size(848,480),
                new Size(960,540),  //qHD
                new Size(1024,576),
                new Size(1280,720), //HD
                new Size(1366,768),
                new Size(1600,900), //HD+
                new Size(1920,1080),//Full HD
                new Size(2048,1152),
                new Size(2560,1440),//QHD
                new Size(3840,2160) //4K UHDTV
            };
            graphHandleSet = new HashSet<int>();
            musicHandleDic = new Dictionary<string, int>();
            KeysHolding = new List<int>();
            keyState = new bool[256];
            keyState.Initialize();
            DX.InitKeyInput();
        }
        #region DrawRelated
        /// <summary>
        /// 单个绘制周期的开始
        /// </summary>
        public static void FrameBegin()
        {
            System.Windows.Forms.Application.DoEvents();
            DXcs.UpdateKeyStatus();
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 255);
        }
        /// <summary>
        /// 单个绘制周期的结束
        /// </summary>
        public static void FrameEnd(bool clearScreen = true)
        {
            DXcs.UpdateKeyStatus2();
            DXcs.Present(clearScreen);
            DXcs.WaitFrameTime();
        }

        public static int Present(bool clearScreen = true)
        {
            //国外论坛上找到的
            DX.RefreshDxLibDirect3DSetting();
            //Present和周期时间计算
            _debugDrawIndex = 0;
            oneFrameCalcTime = DateTime.Now.Ticks - _oneFrameCalcTimeBuf;
            int result = DX.ScreenFlip();
            if (clearScreen)
            {
                DX.ClearDrawScreen();
                DX.ClearDrawScreenZBuffer();
            }
            return result;
        }
        public static void DrawLine(PointF p1,PointF p2,Color col)
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, col.A);
            DX.DrawLine((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, (uint)col.ToArgb());
        }
        public static void DrawLine(int x1, int y1, int x2, int y2, Color col)
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, col.A);
            DX.DrawLine(x1, y1, x2, y2, (uint)col.ToArgb());
        }
        public static void DrawBox(PointF p1, PointF p2, Color col, int fillflag = 0)
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, col.A);
            DX.DrawBox((int)p1.X, (int)p1.Y, (int)p2.X, (int)p2.Y, (uint)col.ToArgb(), fillflag);
        }
        public static void DrawBox(int x1, int y1, int x2, int y2, Color col, int fillflag = 0)
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, col.A);
            DX.DrawBox(x1, y1, x2, y2, (uint)col.ToArgb(), fillflag);
        }
        public static void DrawText(int x, int y, string str, Color col)
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, col.A);
            DX.DrawString(x, y, str, (uint)col.ToArgb());
        }
        public static void DrawText<T>(int x, int y, T infoToDraw, Color col)
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, col.A);
            DX.DrawString(x, y, infoToDraw.ToString(), (uint)col.ToArgb());
        }
        public static void DrawDebug<T>(T infoToDraw)
        {
            DrawText<T>(0, _debugDrawIndex++ * fontHeight, infoToDraw, Color.White);
        }

        #endregion

        #region MathRelated
        public static double Scale(double value, double srcStart, double srcEnd, double destStart, double destEnd)
        {
            value -= srcStart;
            value *= (destEnd - destStart) / (srcEnd - srcStart);
            value += destStart;
            return value;
        }
        public static double SinD(double value, double period, double scale, double offset = 0d)
        {
            return Math.Sin(value / period * Math.PI * 2) * scale + offset;
        }
        public static double CosD(double value, double period, double scale, double offset = 0d)
        {
            return Math.Cos(value / period * Math.PI * 2) * scale + offset;
        }
        public static double TanD(double value, double period, double scale, double offset = 0d)
        {
            return Math.Tan(value / period * Math.PI * 2) * scale + offset;
        }
        public static int Sin(double value, double period, double scale, double offset = 0d)
        {
            return (int)SinD(value, period, scale, offset);
        }
        public static int Cos(double value, double period, double scale, double offset = 0d)
        {
            return (int)CosD(value, period, scale, offset);
        }
        public static int Tan(double value, double period, double scale, double offset = 0d)
        {
            return (int)Tan(value, period, scale, offset);
        }
        public static float Pow2(float a) { return a * a; }
        public static float GetDistance(float x,float y)
        {
            return (float)Math.Sqrt(Pow2(x) + Pow2(y));
        }
        /// <summary>
        /// 返回-PI到PI的角度值
        /// </summary>
        /// <param name="x1"></param>
        /// <param name="y1"></param>
        /// <param name="x2"></param>
        /// <param name="y2"></param>
        /// <returns></returns>
        public static float GetTowards(float x1,float y1,float x2,float y2)
        {
            return (float)Math.Atan2(y2 - y1, x2 - x1);
        }
        /// <summary>
        /// 返回-PI到PI的角度值
        /// </summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        public static float GetTowards(PointF p1,PointF p2)
        {
            return (float)Math.Atan2(p2.Y - p1.Y, p2.X - p1.X);
        }
        /// <summary>
        /// 将角度调整到0~2PI区间
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float AdjustAngle2Basic(float angle)
        {
            while (angle > PI2f)
                angle -= PI2f;
            while (angle < 0)
                angle += PI2f;
            return angle;
        }
        /// <summary>
        /// 将角度调整到-PI~PI区间
        /// </summary>
        /// <param name="angle">角度</param>
        /// <returns></returns>
        public static float AdjustAngle2HalfB(float angle)
        {
            while (angle > PIf)
                angle -= PI2f;
            while (angle < -PIf)
                angle += PI2f;
            return angle;
        }
        /// <summary>
        /// 返回角度是否在所给范围内
        /// </summary>
        /// <param name="angle">已经调整至-PI~PI的角度</param>
        /// <param name="aleft">角度左边界（较小）</param>
        /// <param name="aright">角度右边界（较大）</param>
        /// <returns></returns>
        public static bool AngleInRange(float angle,float aleft,float aright)
        {
            aleft = AdjustAngle2HalfB(aleft);
            while (aright - aleft > PI2f)
                aright -= PI2f;
            while (aright - aleft < 0)
                aright += PI2f;
            while (angle - aleft > PI2f)
                angle -= PI2f;
            while (angle - aleft < 0)
                angle += PI2f;
            return aleft < angle && aright > angle;
        }
        public const double PI2 = Math.PI * 2;
        public const float PI2f = (float)(Math.PI * 2);
        public const double PI = Math.PI;
        public const float PIf = (float)(Math.PI);

        #endregion

        #region basic
        public static void SetWindowSize(Size s)
        {
            SetWindowSize(s.Width, s.Height);
        }
        public static void SetWindowSize(int width, int height)
        {
            frmWidth = width;
            frmHeight = height;
            DX.SetWindowSize(width, height);
        }
        public static void SetResolution(Size s, int colorBitsDepth = 32)
        {
            SetResolution(s.Width, s.Height, colorBitsDepth);
        }
        public static void SetResolution(int width, int height, int colorBitsDepth = 32)
        {
            resWidth = width;
            resHeight = height;
            DX.SetGraphMode(width, height, colorBitsDepth);
        }
        public static void InitGameSettings()
        {
            //Load preferences
        }
        public static int InitForm(string title, bool bWindowed)
        {
            System.IO.StreamWriter log = new System.IO.StreamWriter("Logme.txt");
            log.WriteLine($"{DateTime.Now.ToString()}");
            DX.SetOutApplicationLogValidFlag(0);
            if (bWindowed)
            {
                //This sets the window position on my PC with double monitor.
                DX.SetWindowPosition(0, 100);
                DX.ChangeWindowMode(1);
                DX.SetWindowStyleMode(0);
                DX.SetWaitVSyncFlag(0);
                SetWindowSize(frmSize[defaultSizeIndex]);
            }
            else
            {
                DX.SetWindowPosition(0, 0);
                DX.ChangeWindowMode(1);
                DX.SetWindowStyleMode(2);
                DX.SetWaitVSyncFlag(0);
                SetWindowSize(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            }
            DX.SetWindowIconHandle(TDAjam.Properties.Resources.ml.Handle);
            DX.SetWindowText(title);
            DX.ChangeFont("Microsoft YaHei");
            DX.SetFontSize(20);
            DX.SetAlwaysRunFlag(1);
            fontWidth = 20 / 2;
            fontHeight = (int)(20 * 1.2);

            SetResolution(frmSize[0]);
            log.WriteLine($"{DX.SetFullSceneAntiAliasingMode(0, 0)}");
            log.WriteLine($"{DX.SetCreateDrawValidGraphMultiSample(0, 0)}");
            log.WriteLine($"{DX.SetUse3DFlag(use3Dmode)}");

            if (DX.DxLib_Init() == -1)
            {
                System.Windows.Forms.MessageBox.Show("failed");
                return 0;
            }
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);
            //log.WriteLine($"{ DX.SetZBufferSize(ResWidth, ResHeight)}");
            //log.WriteLine($"{ DX.SetZBufferBitDepth(32)}");
            //log.WriteLine($"{ DX.SetUseZBuffer3D(1)}");
            //log.WriteLine($"{ DX.SetUseZBufferFlag(1)}");
            //log.WriteLine($"{ DX.SetWriteZBuffer3D(1)}");
            //log.WriteLine($"{ DX.SetWriteZBufferFlag(1)}");

            log.Close();
            nowTime = DateTime.Now.Ticks;
            return 1;
        }
        public static long WaitFrameTime()
        {
            //Not work in Vsync-mode
            if (DX.GetWaitVSyncFlag() == 0)
            {
                long deltaTime;
                while ((deltaTime = DateTime.Now.Ticks - nowTime) / 10000d < 1000f / fpsLimit)
                {
                    DX.WaitTimer(1);
                }
                DXcs.nowTime = DateTime.Now.Ticks;
                DXcs.deltaTime = deltaTime;
                _oneFrameCalcTimeBuf = DateTime.Now.Ticks;
                return deltaTime;
            }
            else
                return 0;
        }
        public static void ClearGraphMemory()
        {
            if (graphHandleSet.Count == 0) return;
#if DEBUG
            int i = 0;
            int count = graphHandleSet.Count;
            string handleNames = "";
            foreach (var item in graphHandleSet)
            {
                DX.DeleteGraph(item);
                handleNames += item.ToString() + " ";
                if (++i != 6) continue;
                handleNames += "\n";
                i = 0;
            }
            graphHandleSet.Clear();
            System.Windows.Forms.MessageBox.Show($"Handles deleted:{count}\n" + handleNames);
            return;
#else
            foreach (var item in graphHandleSet)
            {
                DX.DeleteGraph(item);
            }
            graphHandleSet.Clear();
            return;
#endif
        }
        public static void AddGraphHandle(int handle)
        {
            if (!graphHandleSet.Contains(handle))
            {
                graphHandleSet.Add(handle);
            }
        }
        public static void DisposeAll()
        {
            ClearGraphMemory();
        }
        public static bool IsWindowOpen()
        {
            //DxLib自带处理窗体消息的检测，如果消息阻塞，则ProcessMessage返回-1
            return DX.ProcessMessage() == 0;
        }
        public static bool IsWindowActive()
        {
            return DX.GetWindowActiveFlag() == 1;
        }


#endregion

#region InputRelated
        /// <summary>
        /// 按键状态，外部访问通过函数，每帧通过UpdateKeyStatus更新
        /// </summary>
        private static bool[] keyState;
        /// <summary>
        /// 按下键的列表
        /// </summary>
        private static List<int> KeysHolding;
        /// <summary>
        /// 查询对应按键是否按下
        /// </summary>
        /// <param name="KEY_INPUT_">DxLib中指定键的const值</param>
        /// <returns></returns>
        public static bool IsKeyDown(int KEY_INPUT_)
        {
            return keyState[KEY_INPUT_];
        }
        /// <summary>
        /// 查询KeysHolding表和KeyState数组
        /// </summary>
        /// <param name="KEY_INPUT_">DxLib中指定键的const值</param>
        /// <returns></returns>
        public static bool IsKeyDownOnce(int KEY_INPUT_)
        {
            return keyState[KEY_INPUT_] && !KeysHolding.Contains(KEY_INPUT_);
        }
        /// <summary>
        /// 每帧最开始位置使用，直接获取所有按键状态，之后再调用IsKeyDown等都是查表
        /// </summary>
        public static void UpdateKeyStatus()
        {
            for (int i = 0; i < 256; i++)
            {
                keyState[i] = DX.CheckHitKey(i) > 0;
                if (!keyState[i])
                    if (KeysHolding.Contains(i))
                        KeysHolding.Remove(i);
            }
        }
        /// <summary>
        /// 每帧最后的位置使用，把所有获取的按键中按下的放进KeysHolding里边
        /// </summary>
        public static void UpdateKeyStatus2()
        {
            for (int i = 0; i < 256; i++)
            {
                if (keyState[i])
                    if (!KeysHolding.Contains(i))
                        KeysHolding.Add(i);
            }
        }

#endregion

#region AudioRelated
        private static Dictionary<string, int> musicHandleDic;
        /// <summary>
        /// 载入MP3文件到内存
        /// </summary>
        /// <param name="fileName">文件名</param>
        /// <param name="musicName">音乐名，做标记</param>
        public static bool LoadMusic(string fileName, string musicName)
        {
            int handle = DX.LoadMusicMem(fileName);
            if (handle > 0)
            {
                if (musicHandleDic.Contains(new KeyValuePair<string, int>(musicName, handle)))
                {
                    DX.DeleteMusicMem(handle);
                    return false;
                }
                else
                {
                    musicHandleDic.Add(musicName, handle);
                    return true;
                }
            }
            else
                return false;
        }
        /// <summary>
        /// 播放制定音乐名的音乐，从内存
        /// </summary>
        /// <param name="musicName">音乐名</param>
        /// <param name="DX_PLAYTYPE_">播放方式</param>
        /// <returns></returns>
        public static bool PlayMusic(string musicName, int DX_PLAYTYPE_)
        {
            if (musicHandleDic.ContainsKey(musicName))
            {
                DX.PlayMusicMem(musicHandleDic[musicName], DX_PLAYTYPE_);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 停止指定音乐名的音乐
        /// </summary>
        /// <param name="musicName">音乐名</param>
        /// <returns></returns>
        public static bool StopMusic(string musicName)
        {
            if (musicHandleDic.ContainsKey(musicName))
            {
                DX.StopMusicMem(musicHandleDic[musicName]);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 释放指定音乐名所占用的内存
        /// </summary>
        /// <param name="musicName">音乐名</param>
        /// <returns></returns>
        public static bool DeleteMusic(string musicName)
        {
            if (musicHandleDic.ContainsKey(musicName))
            {
                DX.DeleteMusicMem(musicHandleDic[musicName]);
                musicHandleDic.Remove(musicName);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 设定指定音频名的音量
        /// </summary>
        /// <param name="musicName">音乐名</param>
        /// <param name="volume">音量</param>
        /// <returns></returns>
        public static bool SetMusicVolume(string musicName, int volume)
        {
            if (musicHandleDic.ContainsKey(musicName))
            {
                DX.SetVolumeMusicMem(volume, musicHandleDic[musicName]);
                return true;
            }
            else
            {
                return false;
            }
        }
        /// <summary>
        /// 清除所有音乐所占用的内存
        /// </summary>
        public static void ClearMusicMemory()
        {
            foreach (int item in musicHandleDic.Values)
            {
                DX.DeleteMusicMem(item);
            }
            musicHandleDic.Clear();
        }

#endregion
    }
    /// <summary>
    /// Simple sprite draw order control class
    /// </summary>
    internal static class DxLayer
    {
        static DxLayer ()
        {
            list = new List<DGparams>();
        }
        /// <summary>
        /// Get the state of layer. Affect image draw order when open.
        /// </summary>
        public static bool LayerOpened { get; private set; }
        internal static List<DGparams> list;
        internal class DGcomparer : IComparer<DGparams>
        {
            public int Compare(DGparams x, DGparams y)
            {
#if DEBUG
                compareTimes++;
#endif
                return (int)Math.Round(x.z - y.z);
                //throw new NotImplementedException();
            }
        }
        private static DGcomparer DGC = new DGcomparer();
        public static float Zplace { get; set; } = 0;
#if DEBUG
        public static int compareTimes=0;
        public static float sortingTime = 0f;
        public static float drawingTime = 0f;
#endif
        public static void SetZ(float Z)
        {
            Zplace = Z;
        }
        /// <summary>
        /// Open the layer to draw.
        /// </summary>
        /// <returns></returns>
        public static bool Open()
        {
            if (LayerOpened)
                return false;
            LayerOpened = true;
            return true;
        }
        /// <summary>
        /// Close the layer and draw layer.
        /// </summary>
        /// <returns></returns>
        public static bool Close()
        {
#if DEBUG
            if (!LayerOpened)
                return false;
            long tn = DateTime.Now.Ticks;
            SortLayer();
            sortingTime = (DateTime.Now.Ticks - tn) / 10000f;
            tn = DateTime.Now.Ticks;
            DrawLayer();
            drawingTime = (DateTime.Now.Ticks - tn) / 10000f;
            LayerOpened = false;
            return true;
#else
            if (!LayerOpened)
                return false;
            SortLayer();
            DrawLayer();
            LayerOpened = false;
            return true;
#endif
        }
        private static void SortLayer()
        {
#if DEBUG
            compareTimes = 0;
#endif
            list.Sort(DGC);
        }
        private static void DrawLayer()
        {
            DGparams DGtemp;
            for (int i = 0; i < list.Count; i++)
            {
                DGtemp = list[i];
                switch(list[i].method )
                {
                    case 0:
                        DX.DrawGraph(DGtemp.destX, DGtemp.destY, DGtemp.handle, 1);
                        break;
                    case 1:
                        DX.DrawRotaGraph3(DGtemp.destX, DGtemp.destY, DGtemp.centX, DGtemp.centY,
                            DGtemp.scaleX, DGtemp.scaleY, DGtemp.angle, DGtemp.handle, 1);
                        break;
                    case 2:
                        DX.DrawRectRotaGraph3(
                            DGtemp.destX, DGtemp.destY,
                            DGtemp.srcX, DGtemp.srcY, DGtemp.srcW, DGtemp.srcH,
                            DGtemp.centX, DGtemp.centY,
                            DGtemp.scaleX, DGtemp.scaleY,
                            DGtemp.angle, DGtemp.handle, 1, 0);
                        break;
                }
            }
            list.Clear();
        }
    }
}
