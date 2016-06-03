﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using DxLibDLL;

namespace TDAjam
{
    static class Program
    {
        public static bool bStop = false;

        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            DXcs.InitGameSettings();
            DXcs.InitForm("TouhouDungeonAdventure_Jam", true);

            //TestingUnit.TestSprite();
            //TestingUnit.TestSingleAnimation();
            //TestingUnit.TestingInput();
            //TestingUnit.TestingClocking();
            //little change.
            //let's see if the git works fine.
            TestingUnit.TestingAudio();

            DXcs.DisposeAll();
            DX.DxLib_End();
        }
    }
    static class TestingUnit
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
                s.DrawCellSprite(DXcs.CenterX, DXcs.CenterY);
                DX.DrawLine(
                    0,
                    DXcs.ResHeight,
                    (int)((float)iFrame * DXcs.ResWidth / 1000),
                    DXcs.ResHeight,
                    (uint)Color.White.ToArgb(), 20);
                DXcs.DrawText(0, 0, "Running \"TestingUnit.TestSprite()\"", Color.White);
                DXcs.DrawText(0, DXcs.FontHeight, typeof(DxSprite).FullName, Color.White);
                DXcs.DrawText(
                    DXcs.CenterX + 50, DXcs.CenterY + 50,
                    s.cellIndex.ToString() + "/" + s.cellCount.ToString(), Color.White);
                DXcs.DrawText(
                    DXcs.CenterX + 50, DXcs.CenterY + DXcs.FontHeight + 50,
                    s.cellPosition, Color.White);

                DXcs.DrawText(
                    (int)((float)iFrame * DXcs.ResWidth / 1000) - 35,
                    DXcs.ResHeight - 30,
                    iFrame,
                    Color.Wheat);

                DXcs.FrameEnd();
            }
            DX.WaitKey();
            dxi.UnloadImage();
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
                dsa.DrawAnimationFrame(DXcs.CenterX, DXcs.CenterY);
                s.DrawSprite(20, 163);
                DXcs.DrawDebug("TestingUnit.TestSingleAnimation()");
                DXcs.DrawDebug(typeof(DxSingleAnimation));
                DXcs.DrawDebug("Ticks:" + ((ulong)DateTime.Now.Ticks - dsa.startTime));
                DXcs.DrawDebug("index:" + ((ulong)DateTime.Now.Ticks - dsa.startTime) / (dsa.frameTime * 10000));
                DXcs.DrawDebug("indexMap:{ 9, 10, 11, 10 }");
                DXcs.DrawDebug("Frame:" + iFrame);
                DX.DrawBox(
                    0,
                    DXcs.ResHeight - 10,
                    (int)DXcs.Scale(iFrame, 0, totalFrame, 0, DXcs.ResWidth),
                    DXcs.ResHeight,
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

                DXcs.DrawDebug("ticks:" + DXcs.oneFrameCalcTime);
                DXcs.DrawDebug("ms:" + (float)DXcs.oneFrameCalcTime / 10000);
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

    }
}
