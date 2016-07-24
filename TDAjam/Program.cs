using System;
using System.Collections.Generic;
using System.Linq;
using System.Drawing;
using DxLibDLL;

namespace TDAjam
{
    internal static class Program
    {
        public static bool bStop = false;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        private static void Main()
        {
            DXcs.InitGameSettings();
            DXcs.InitForm("TouhouDungeonAdventure_Jam", true);

#if DEBUG
            //TestingUnit.TestSprite();
            //TestingUnit.TestSpriteZ();
            //TestingUnit.TestSingleAnimation();
            //TestingUnit.TestingInput();
            //TestingUnit.TestingClocking();
            //TestingUnit.TestingAudio();
            //TestingUnit.Tt_Position();
            TestingUnit.Tt_Math();
#endif

            DXcs.DisposeAll();
            DX.DxLib_End();
        }
    }
#if DEBUG
    internal static class TestingUnit
    {
        public static void TestSprite()
        {
            ulong totalFrame = 1000;
            DxImage dxi = new DxImage(@"RES\chara_07.png");
            DxSprite s = new DxSprite(dxi, 3, 5);
            s.SetCenter2CellCenter();
            for (uint iFrame = 0; iFrame <= totalFrame && DXcs.IsWindowOpen(); iFrame++)
            {
                DXcs.FrameBegin();

                if (iFrame % 18 == 0)
                {
                    s.SetIndex(s.cellIndex + 1);
                }
                s.DrawCellSprite(DXcs.centerX, DXcs.centerY);
                DX.DrawLine(
                    0,
                    DXcs.resHeight,
                    (int)((float)iFrame * DXcs.resWidth / 1000),
                    DXcs.resHeight,
                    (uint)Color.White.ToArgb(), 20);
                DXcs.DrawText(0, 0, "Running \"TestingUnit.TestSprite()\"", Color.White);
                DXcs.DrawText(0, DXcs.fontHeight, typeof(DxSprite).FullName, Color.White);
                DXcs.DrawText(
                    DXcs.centerX + 50, DXcs.centerY + 50,
                    s.cellIndex.ToString() + "/" + s.cellCount.ToString(), Color.White);
                DXcs.DrawText(
                    DXcs.centerX + 50, DXcs.centerY + DXcs.fontHeight + 50,
                    s.cellPosition, Color.White);

                DXcs.DrawText(
                    (int)((float)iFrame * DXcs.resWidth / 1000) - 35,
                    DXcs.resHeight - 30,
                    iFrame,
                    Color.Wheat);

                DXcs.FrameEnd();
            }
            DX.WaitKey();
            dxi.UnloadImage();
        }
        public static void TestSpriteZ()
        {
            GC.Collect();
            DxImage[] charalist = new DxImage[36];
            for (int i = 0; i < 36; i++)
            {
                charalist[i] = new DxImage(@"RES\chara\chara_" + string.Format("{0:D2}", i + 1) + ".png");
            }
            DxSprite sp1 = new DxSprite(charalist[0], 3, 5);    
            int[] face = { 7, 10, 13, 10, 7, 4, 1, 4 };
            int count = 1, speed = 4000;
            double rollang = Math.PI;
            long start = DateTime.Now.Ticks;
            //DX.SetBackgroundColor(0, 128, 0);
            while (DXcs.IsWindowOpen() && !DXcs.IsKeyDown(DX.KEY_INPUT_ESCAPE))
            {
                DXcs.FrameBegin();
                if (!DXcs.IsKeyDown(DX.KEY_INPUT_SPACE))
                {
                    DxLayer.Open();
                    for (int i = 0; i < count; i++)
                    {
                        double a, r;
                        float x, y, z;
                        for (int j = 0; j < 8; j++)
                        {
                            r = 100;
                            a = DXcs.Scale((DXcs.nowTime - start) / 10000 % speed, 0, speed, 0, Math.PI * 2) +
                                rollang / count * i + j * 6.28f / 8;
                            if (a > Math.PI * 2)
                                a -= Math.PI * 2;
                            z = DXcs.Sin(a, Math.PI * 2, r, 0);
                            x = DXcs.Cos(a, Math.PI * 2, r, DXcs.centerX);
                            y = DXcs.centerY - (count / 2 - i) * 200 / count + z / 2;
                            DxLayer.SetZ(z);
                            sp1.SetIndex(face[(int)((a + Math.PI / 8) / (Math.PI / 4)) % 8]);
                            sp1.image = charalist[j];
                            if (a >= Math.PI / 2 && a < Math.PI * 1.5f)
                                sp1.SetScale(-1f, 1f);
                            else
                                sp1.SetScale(1f, 1f);
                            sp1.DrawCellSprite((int)x, (int)y);
                        }
                    }
                    DxLayer.Close();
                    if(false)
                    {
                        DXcs.DrawDebug("AntiAliasing Test");
                        DXcs.DrawDebug("Z test");
                        DXcs.DrawDebug("c=" + DxLayer.compareTimes + " times");
                        DXcs.DrawDebug("s=" + DxLayer.sortingTime + "ms");
                        DXcs.DrawDebug("d=" + DxLayer.drawingTime + "ms");
                        DXcs.DrawDebug("f=" + DXcs.deltaTime / 10000f + "ms");
                    }
                }
                DXcs.FrameEnd();
            }
        }
        public static void TestSingleAnimation()
        {
            uint totalFrame = 1000;
            DxImage dxi = new DxImage(@"RES\chara_07.png");
            DxSprite s = new DxSprite(dxi, 3, 5);
            s.SetCenter2CellCenter();
            DxSingleAnimation dsa = new DxSingleAnimation(ref s, 2.0f, true);
            dsa.SetIndexMap(new ushort[] { 9, 10, 11, 10 });
            dsa.frameTime = 200;
            for (uint iFrame = 0; iFrame < totalFrame && DXcs.IsWindowOpen(); iFrame++)
            {
                DXcs.FrameBegin();
                dsa.ApplyTransform();
                dsa.DrawAnimationFrame(DXcs.centerX, DXcs.centerY);
                s.DrawSprite(20, 163);
                DXcs.DrawDebug("TestingUnit.TestSingleAnimation()");
                DXcs.DrawDebug(typeof(DxSingleAnimation));
                DXcs.DrawDebug("Ticks:" + (DXcs.nowTime - dsa.startTime));
                DXcs.DrawDebug("index:" + (DXcs.nowTime - dsa.startTime) / (dsa.frameTime * 10000));
                DXcs.DrawDebug("indexMap:{ 9, 10, 11, 10 }");
                DXcs.DrawDebug("Frame:" + iFrame);
                DX.DrawBox(
                    0,
                    DXcs.resHeight - 10,
                    (int)DXcs.Scale(iFrame, 0, totalFrame, 0, DXcs.resWidth),
                    DXcs.resHeight,
                    (uint)Color.White.ToArgb(), 1);

                DXcs.FrameEnd();
            }
        }
        public static void TestingInput()
        {
            uint pressFrame = 0, i = 0;
            uint pressFrameO = 0;
            bool kdo = false;
            while (pressFrame < 200u && !DXcs.IsKeyDown(DX.KEY_INPUT_ESCAPE) && DXcs.IsWindowOpen())
            {
                DXcs.FrameBegin();

                kdo = DXcs.IsKeyDownOnce(DX.KEY_INPUT_A);
                if (kdo) pressFrameO++;
                DXcs.DrawDebug(">Press A, test stop when pressFrame reach 200.");
                DXcs.DrawDebug("Frames=" + i++);
                DXcs.DrawDebug("IsKeyDownOnce=" + kdo);
                DXcs.DrawDebug("IsKeyDown=" + DXcs.IsKeyDown(DX.KEY_INPUT_A));
                DXcs.DrawDebug("pressFrame=" + pressFrame);
                DXcs.DrawDebug("pressFrameO=" + pressFrameO);
                if (DXcs.IsKeyDown(DX.KEY_INPUT_A)) pressFrame++;

                DXcs.FrameEnd();
            }
        }
        public static void TestingClocking()
        {
            while (!DXcs.IsKeyDown(DX.KEY_INPUT_ESCAPE) && DXcs.IsWindowOpen())
            {
                DXcs.FrameBegin();

                DXcs.DrawDebug($"ticks:{DXcs.oneFrameCalcTime}");
                DXcs.DrawDebug($"ms:{(float)DXcs.oneFrameCalcTime / 10000}");
                DXcs.DrawDebug(1000f / 60);

                DXcs.FrameEnd();
            }
        }
        public static void TestingAudio()
        {
            DXcs.LoadMusic("Trixie Megalovania.mp3", "tm");
            DXcs.PlayMusic("tm", DX.DX_PLAYTYPE_LOOP);
            while (!DXcs.IsKeyDown(DX.KEY_INPUT_ESCAPE) && DXcs.IsWindowOpen())
            {
                DXcs.FrameBegin();

                DXcs.DrawDebug("Press {A,S,D,F} to adjust music volume.");
                DXcs.DrawDebug("Need a mp3 file named \"Trixie Megalovania.mp3\".");
                if (DXcs.IsKeyDown(DX.KEY_INPUT_A))
                {
                    DXcs.DrawDebug("A");
                    DXcs.SetMusicVolume("tm", 64);
                }
                if (DXcs.IsKeyDown(DX.KEY_INPUT_S))
                {
                    DXcs.DrawDebug("S");
                    DXcs.SetMusicVolume("tm", 128);
                }
                if (DXcs.IsKeyDown(DX.KEY_INPUT_D))
                {
                    DXcs.DrawDebug("D");
                    DXcs.SetMusicVolume("tm", 192);
                }
                if (DXcs.IsKeyDown(DX.KEY_INPUT_F))
                {
                    DXcs.DrawDebug("F");
                    DXcs.SetMusicVolume("tm", 255);
                }

                DXcs.FrameEnd();
            }
        }
        public static void Tt_Position()
        {
            int len;
            int lenc = 100, lenv = 20;
            int count = 7;
            int tp = 180;
            float pi23 = (float)Math.PI * 2 / 3;
            float pi2 = (float)Math.PI * 2;
            Position p = new Position(DXcs.resWidth / 2, DXcs.resHeight / 2);
            List<Position> pl = new List<Position>();
            Random rnd = new Random();
            while (DXcs.IsWindowOpen() && !DXcs.IsKeyDown(DX.KEY_INPUT_ESCAPE))
            {
                if (tp < 180)
                    tp++;
                else
                {
                    tp = 0;
                    pl.Clear();
                    lenc = (int)(rnd.NextDouble() * 60) + 60;
                    lenv = (int)(rnd.NextDouble() * lenc);
                    double rs = rnd.NextDouble();
                    count = (int)(rs * 28) + 5;
                    for (float i = 0; i < pi2; i += pi2 / count)
                    {
                        pl.Add(new Position(0, 0, p));
                    }
                }
                DXcs.FrameBegin();
                DXcs.DrawDebug("Press ESC to exit.");
                DXcs.DrawDebug(pl.Count);
                DXcs.DrawDebug($"{(float)DXcs.deltaTime / 10000}ms");
                for (int i = 0; i < pl.Count; i++)
                {
                    float a = i * pi2 / pl.Count;
                    float at = (float)DXcs.Scale(DateTime.Now.Millisecond, 0, 1000, 0, pi23);
                    len = DXcs.Cos((at - a), pi23, lenv, lenc);
                    pl[i].MoveTo(len * (float)Math.Cos(a), len * (float)Math.Sin(a));
                    //DXcs.DrawLine(pl[i].toPointFAbs(), p.toPointFAbs(), Color.White);
                    if (i > 0)
                        DXcs.DrawLine(pl[i - 1].toPointFAbs(), pl[i].toPointFAbs(), Color.White);
                }
                DXcs.DrawLine(pl[pl.Count - 1].toPointFAbs(), pl[0].toPointFAbs(), Color.White);
                DXcs.DrawBox(0, 0, DXcs.resWidth, DXcs.resHeight, Color.FromArgb(16, Color.Black), 1);

                DXcs.FrameEnd(false);
            }
        }
        public static void Tt_Math()
        {
            DXcs.fpsLimit = 120f;
            while(DXcs.IsWindowOpen () && !DXcs.IsKeyDown (DX.KEY_INPUT_ESCAPE ))
            {
                DXcs.FrameBegin();
                int x, y;
                DX.GetMousePoint(out x, out y);
                DXcs.DrawDebug($"mousePos:({x},{y})");
                DXcs.DrawDebug($"FrmCenter:({DXcs.centerX },{DXcs.centerY })");
                DXcs.DrawDebug($"Math.Atan2:{Math.Atan2(y - DXcs.centerY, x - DXcs.centerX) }");
                DXcs.DrawDebug($"Dxcs.GetDistance:{DXcs.GetDistance(y - DXcs.centerY, x - DXcs.centerX)}");
                DXcs.DrawDebug(1000f / DXcs.fpsLimit);
                DXcs.DrawDebug(DXcs.deltaTime / 10000f);

                var fcf = new FanCollisionField(new Position(DXcs.centerX, DXcs.centerY), 100,
                    (float)DXcs.Scale(DXcs.nowTime / 10000 % 3000, 0, 3000, 0, DXcs.PI2),
                    (float)DXcs.Scale(DXcs.nowTime / 10000 % 5000, 0, 5000, 0, DXcs.PIf));
                DXcs.DrawLine(DXcs.centerX, DXcs.centerY,
                    DXcs.Cos(fcf.towards - fcf.spread, DXcs.PI2f, 100, DXcs.centerX),
                    DXcs.Sin(fcf.towards - fcf.spread, DXcs.PI2f, 100, DXcs.centerY),
                    Color.White);
                DXcs.DrawLine(DXcs.centerX, DXcs.centerY,
                    DXcs.Cos(fcf.towards + fcf.spread, DXcs.PI2f, 100, DXcs.centerX),
                    DXcs.Sin(fcf.towards + fcf.spread, DXcs.PI2f, 100, DXcs.centerY),
                    Color.White);
                float a = fcf.towards - fcf.spread;
                for (int i = 0; i < 36; i++)
                {
                    float a1, a2;
                    a1 = a + fcf.spread * 2 / 36 * i;
                    a2 = a + fcf.spread * 2 / 36 * (i + 1);
                    DXcs.DrawLine(
                        DXcs.Cos(a1, DXcs.PI2f, 100, DXcs.centerX),
                        DXcs.Sin(a1, DXcs.PI2f, 100, DXcs.centerY),
                        DXcs.Cos(a2, DXcs.PI2f, 100, DXcs.centerX),
                        DXcs.Sin(a2, DXcs.PI2f, 100, DXcs.centerY),
                        Color.White);
                }
                Entity ent = new Entity {position = new Position(x, y)};
                if(fcf.CollideWith (ent))
                    DXcs.DrawBox(x - 3, y - 3, x + 3, y + 3, Color.Blue, 1);
                else
                    DXcs.DrawBox(x - 3, y - 3, x + 3, y + 3, Color.Red, 1);

                DXcs.FrameEnd();
                if (DXcs.nowTime / 10000 % 3000 < 20)
                    GC.Collect();
            }
        }
    }
#endif
}