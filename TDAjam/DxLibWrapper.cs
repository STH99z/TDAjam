using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DxLibDLL;
using System.Drawing;

namespace TDAjam
{
    /// <summary>
    /// Single wrapper for image handle.
    /// </summary>
    [Serializable]
    class DxImage
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
            if (IsLoaded)
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
        public bool IsLoaded
        {
            get { return handle != -1; }
        }
        public void DrawImage(int x, int y, int transFlag = 1)
        {
            DX.DrawGraph(x, y, handle, transFlag);

        }
        public void DrawImageAdv(int x, int y, int centerX, int centerY, double scaleX, double scaleY, double angle)
        {
            DX.DrawRotaGraph3(x, y, centerX, centerY, scaleX, scaleY, angle, handle, 1);
        }
        public void DrawImageClipAdv(int x, int y, int srcX, int srcY, int srcW, int srcH, int centerX, int centerY, double scaleX, double scaleY, double angle, int flip)
        {
            DX.DrawRectRotaGraph3(x, y, srcX, srcY, srcW, srcH, centerX, centerY, scaleX, scaleY, angle, handle, 1, 0);
        }

    }
    /// <summary>
    /// Sprite, combined advanced 2D draw methods of a single image.
    /// Only contains changable drawing arguments.
    /// Use SingleAnimation or Animation class for procedural changle usage.
    /// </summary>
    class DxSprite
    {
        DxImage image;
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
            if (centered) SetCenter2CellCenter();
            else SetCenter(0, 0);
            SetSlice(slicex, slicey);
            SetScale(scalex, scaley);
            SetAngle(angle);
            CalcCellSize();

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
                angle, 0);
        }
    }
    /// <summary>
    /// Arrangement of arguments in DxSprite.
    /// </summary>
    class DxSingleAnimation
    {
        /// <summary>
        /// 变换信息类
        /// </summary>
        public class transformInfo
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

            public transformInfo(float value, InterpolationMethod method = InterpolationMethod.Linear, string Tag = "")
            {
                this.value = value;
                this.start = value;
                this.end = value;
                CalcCurrentPos();
                this.interpolationMethod = method;
                this.tag = Tag;
            }
            public transformInfo(float value, float start, float end, InterpolationMethod method, string Tag = "")
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
        public ulong startTime { get; private set; }
        public bool usePositionControl { get; set; }
        public PointF position { get; set; }
        public PointF positionStart { get; set; }
        public PointF positionEnd { get; set; }
        public transformInfo posXTransform, posYTransform;
        public transformInfo[] transform { get; private set; }
        public bool[] useTransform { get; set; }

        /// <summary>
        /// Constructor of DxSingleAnimation.
        /// </summary>
        /// <param name="dxs"></param>
        public DxSingleAnimation(ref DxSprite dxs, float animationTime, bool indexMapping = false, bool positionControl = false)
        {
            sprite = dxs;
            transform = new transformInfo[5]
            {
                new transformInfo (sprite.scaleX),
                new transformInfo(sprite.scaleY),
                new transformInfo(sprite.angle),
                new transformInfo(sprite.centerX),
                new transformInfo (sprite.centerY)
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
            this.startTime = (ulong)DateTime.Now.Ticks;
        }
        public void SetIndexMap(ushort[] map)
        {
            this.indexMap = map;
        }
        public void SetPosTransform(PointF startP, PointF endP, transformInfo.InterpolationMethod method)
        {
            usePositionControl = true;
            positionStart = startP;
            positionEnd = endP;
            posXTransform = new transformInfo(positionStart.X, positionStart.X, positionEnd.X, method);
            posYTransform = new transformInfo(positionStart.Y, positionStart.Y, positionEnd.Y, method);
        }
        public void ApplyTransform()
        {
            ulong timeNow = DXcs.nowTime;
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
        public void SetTransform(TransformItem item, float start, float end, transformInfo.InterpolationMethod method)
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
    static class DXcs
    {
        private const bool DEBUGMODE = true;
        private const int use3Dmode = 0;
        public static Random rnd;
        public static int FrmWidth, FrmHeight;
        public static int ResWidth, ResHeight;
        /// <summary>
        /// each frame start time. returns ticks.
        /// </summary>
        public static ulong nowTime;
        public static ulong deltaTime;
        public static Size[] FrmSize;
        public static int DefaultSizeIndex = 6;
        public static int CenterX
        {
            get { return ResWidth / 2; }
        }
        public static int CenterY
        {
            get { return ResHeight / 2; }
        }
        public static Point Center
        {
            get { return new Point(CenterX, CenterY); }
        }
        public static int FontHeight { get; private set; }
        public static int FontWidth { get; private set; }
        private static int debugDrawIndex = 0;
        public static ulong oneFrameCalcTime { get; private set; }
        private static ulong oneFrameCalcTimeBuf;

        private static HashSet<int> graphHandleSet;

        static DXcs()
        {
            rnd = new Random();
            FrmSize = new Size[]
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
        }
        /// <summary>
        /// 单个绘制周期的结束
        /// </summary>
        public static void FrameEnd()
        {
            DXcs.UpdateKeyStatus2();
            DXcs.Present();
            DXcs.WaitFrameTime();
        }

        public static int Present(bool clearScreen = true)
        {
            //Present和周期时间计算
            debugDrawIndex = 0;
            oneFrameCalcTime = (ulong)DateTime.Now.Ticks - oneFrameCalcTimeBuf;
            int result = DX.ScreenFlip();
            if (clearScreen)
                DX.ClearDrawScreen();
            return result;
        }
        public static void DrawText(int x, int y, string str, Color col)
        {
            DX.DrawString(x, y, str, (uint)col.ToArgb());
        }
        public static void DrawText<T>(int x, int y, T infoToDraw, Color col)
        {
            DX.DrawString(x, y, infoToDraw.ToString(), (uint)col.ToArgb());
        }
        public static void DrawDebug<T>(T infoToDraw)
        {
            DrawText<T>(0, debugDrawIndex++ * FontHeight, infoToDraw, Color.White);
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

        #endregion

        #region basic
        public static void SetWindowSize(Size s)
        {
            SetWindowSize(s.Width, s.Height);
        }
        public static void SetWindowSize(int width, int height)
        {
            FrmWidth = width;
            FrmHeight = height;
            DX.SetWindowSize(width, height);
        }
        public static void SetResolution(Size s, int colorBitsDepth = 32)
        {
            SetResolution(s.Width, s.Height, colorBitsDepth);
        }
        public static void SetResolution(int width, int height, int colorBitsDepth = 32)
        {
            ResWidth = width;
            ResHeight = height;
            DX.SetGraphMode(width, height, colorBitsDepth);
        }
        public static void InitGameSettings()
        {
            //Load preferences
        }
        public static int InitForm(string title, bool bWindowed)
        {
            DX.SetOutApplicationLogValidFlag(0);
            if (bWindowed)
            {
                //This sets the window position on my PC with double monitor.
                DX.SetWindowPosition(0, 100);
                DX.ChangeWindowMode(1);
                DX.SetWindowStyleMode(0);
                SetWindowSize(FrmSize[DefaultSizeIndex]);
            }
            else
            {
                DX.SetWindowPosition(0, 0);
                DX.ChangeWindowMode(1);
                DX.SetWindowStyleMode(2);
                SetWindowSize(System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width, System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height);
            }
            DX.SetWindowIconHandle(TDAjam.Properties.Resources.ml.Handle);

            SetResolution(FrmSize[0]);
            DX.SetUse3DFlag(use3Dmode);//Set to 0 to disable multi-sampling.
            DX.SetAlwaysRunFlag(1);
            DX.SetFullSceneAntiAliasingMode(0, 0); //NOTICE: Only in 3D Scene
            DX.SetCreateDrawValidGraphMultiSample(0, 0);

            DX.SetWindowText(title);
            DX.ChangeFont("Microsoft YaHei");
            DX.SetFontSize(20);
            FontWidth = 20 / 2;
            FontHeight = (int)(20 * 1.2);
            if (DX.DxLib_Init() == -1)
            {
                System.Windows.Forms.MessageBox.Show("failed");
                return 0;
            }
            DX.SetDrawScreen(DX.DX_SCREEN_BACK);
            nowTime = (ulong)DateTime.Now.Ticks;
            return 1;
        }
        public static ulong WaitFrameTime()
        {
            //Not work in Vsync-mode
            ulong deltaTime;
            while ((deltaTime = (ulong)DateTime.Now.Ticks - nowTime) < 1000f / 60)
            {
                DX.WaitTimer(1);
            }
            DXcs.nowTime = (ulong)DateTime.Now.Ticks;
            DXcs.deltaTime = deltaTime;
            oneFrameCalcTimeBuf = (ulong)DateTime.Now.Ticks;
            return deltaTime;
        }
        public static void ClearGraphMemory()
        {
            if (graphHandleSet.Count == 0) return;
            if (!DEBUGMODE)
            {
                foreach (var item in graphHandleSet)
                {
                    DX.DeleteGraph(item);
                }
                graphHandleSet.Clear();
                return;
            }
            else
            {
                string handleNames = "";
                foreach (var item in graphHandleSet)
                {
                    DX.DeleteGraph(item);
                    handleNames += item + '\n';
                }
                graphHandleSet.Clear();
                System.Windows.Forms.MessageBox.Show("Handles deleted:\n" + handleNames);
                return;
            }

        }
        public static void AddGraphHandle(int handle)
        {
            if (!graphHandleSet.Contains(handle)) graphHandleSet.Add(handle);
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
}
