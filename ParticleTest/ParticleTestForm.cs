﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Threading;
using System.Windows.Forms;

namespace ParticleTest
{
    public partial class ParticleTestForm : Form
    {
        private const float AngleDefault = 270f;

        private bool _isEmitting;
        private readonly ParticleManager _manager;

        private readonly Thread _particlethread;
        private Thread _emitterThread;

        // Gui members
        private float _minPosX;
        private float _minPosY;

        private float _maxPosX;
        private float _maxPosY;

        private float _speed;
        private float _angle;

        private float _wind;
        private float _gravity;

        private int _particleAmount;
        private int _life;

        private int _minRed;
        private int _maxRed;

        private int _minGreen;
        private int _maxGreen;

        private int _minBlue;
        private int _maxBlue;

        private int _cycle;
        private int _interval;

        /// <summary>
        /// Form initialization
        /// </summary>
        public ParticleTestForm()
        {
            _manager = new ParticleManager();
            InitializeComponents();
            init_Gui();

            _particlethread = new Thread(ParticleLifeCycle);
            _particlethread.Start();
            _isEmitting = false;

            //DrawPanel.Paint += new PaintEventHandler(DrawPanelPaint);
        }

        /// <summary>
        /// Updates particles and cleans up particle array
        /// </summary>
        public void ParticleLifeCycle()
        {
            try
            {
                for (;;)
                {
                    Thread.Sleep(20);
                    _manager.UpdateParticles();
                    Invoke((MethodInvoker) (() =>
                    {
                        DrawBox.Invalidate();
                        DrawBox.Update();
                    }));
                }
            }
            catch (ThreadAbortException ex)
            {
                Debug.Print("Particle display Thread aborted.");
            }
        }

        /// <summary>
        /// 'Emitter' for particles.
        /// </summary>
        private void EmitParticles()
        {
            try
            {
                Random randomizer = new Random();

                for (int i = 0; i < _cycle; i++)
                {
                    Thread.Sleep(_interval);
                    GenerateParticles(randomizer);
                }
            }
            catch (ThreadAbortException ex)
            {
                Debug.Print("Particle generaton thread has stopped.");
            }

            _isEmitting = false;
        }

        private void GenerateParticles(Random randomizer)
        {
                float xCoordinate = ((float) (randomizer.NextDouble() * (double) (_maxPosX - _minPosX)) + _minPosX);
                float yCoordinate = ((float) (randomizer.NextDouble() * (double) (_maxPosY - _minPosY)) + _minPosY);

                Color particleColor = Color.FromArgb(calulateRandomColor(randomizer, _maxRed, _minRed),
                    calulateRandomColor(randomizer, _maxGreen, _minGreen),
                    calulateRandomColor(randomizer, _maxBlue, _minBlue));

                for (int n = 0; n < _particleAmount; n++)
                {
                    float angle = calculateRandomAngle(randomizer, _angle);
                    _manager.AddParticle(new Particle(xCoordinate, yCoordinate, _wind, _gravity, _life, angle,
                        (float) randomizer.NextDouble() * _speed, particleColor,
                        true));
                }
        }
        
        private static int calulateRandomColor(Random randomizer, int maxValue, int minValue)
        {
            // use float cast to int to account for 0.x rounding errors
            return (int) (randomizer.NextDouble() * (maxValue - minValue)) + minValue;
        }

        private static float calculateRandomAngle(Random randomizer, float angleRange)
        {
            return (float) (randomizer.NextDouble() * angleRange) + (AngleDefault - (angleRange / 2));
        }

        


        /// <summary>
        /// Draw Particles on DrawBox PaintEvent
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawBox_Paint(object sender, PaintEventArgs e)
        {
            _manager.DrawParticles(e.Graphics);
        }

        /// <summary>
        /// Initialize all gui components 
        /// </summary>
        private void init_Gui()
        {
            // region Initialize TabControl

            // region TabPage Option

            // Position components

            PosScrBarMinPosX.Minimum = 1;
            PosScrBarMinPosX.Maximum = DrawBox.Width - 1;

            PosScrBarMinPosY.Minimum = 1;
            PosScrBarMinPosY.Maximum = DrawBox.Height - 1;

            PosScrBarMaxPosX.Minimum = 1;
            PosScrBarMaxPosX.Maximum = DrawBox.Width - 1;

            PosScrBarMaxPosY.Minimum = 1;
            PosScrBarMaxPosY.Maximum = DrawBox.Height - 1;

            // Velocity components

            OptScrBarSpeed.Minimum = 1;
            OptScrBarSpeed.Maximum = 500;

            OptScrBarAgl.Minimum = 1;
            OptScrBarAgl.Maximum = 360;

            // Physic components

            OptScrBarWind.Minimum = -250;
            OptScrBarWind.Maximum = 250;

            OptScrBarGrvy.Minimum = -250;
            OptScrBarGrvy.Maximum = 250;

            // Particle components

            OptScrBarNumPart.Minimum = 1;
            OptScrBarNumPart.Maximum = 1000;

            OptScrBarLife.Minimum = 1;
            OptScrBarLife.Maximum = 15;

            // Emission components

            OptScrBarCyle.Minimum = 1;
            OptScrBarCyle.Maximum = 500;

            OptScrBarInter.Minimum = 25;
            OptScrBarInter.Maximum = 500;

            // end region TabPage Option

            // region TabPage Color

            // Color min components

            ColScrBarMinR.Minimum = 0;
            ColScrBarMinR.Maximum = 255;
            ColScrBarMinR.Value = 0;

            ColLblMinRVal.Text = ColScrBarMinR.Value.ToString();
            ColLblMinR.BackColor = Color.Red;

            ColScrBarMinG.Minimum = 0;
            ColScrBarMinG.Maximum = 255;
            ColScrBarMinG.Value = 0;

            ColLblMinGVal.Text = ColScrBarMinG.Value.ToString();
            ColLblMinG.BackColor = Color.Green;

            ColScrBarMinB.Minimum = 0;
            ColScrBarMinB.Maximum = 255;
            ColScrBarMinB.Value = 0;

            ColLblMinBVal.Text = ColScrBarMinB.Value.ToString();
            ColLblMinB.BackColor = Color.Blue;

            // Color max components

            ColScrBarMaxR.Minimum = 0;
            ColScrBarMaxR.Maximum = 255;
            ColScrBarMaxR.Value = 255;

            ColLblMaxRVal.Text = ColScrBarMaxR.Value.ToString();
            ColLblMaxR.BackColor = Color.Red;

            ColScrBarMaxG.Minimum = 0;
            ColScrBarMaxG.Maximum = 255;
            ColScrBarMaxG.Value = 255;

            ColLblMaxGVal.Text = ColScrBarMaxG.Value.ToString();
            ColLblMaxG.BackColor = Color.Green;

            ColScrBarMaxB.Minimum = 0;
            ColScrBarMaxB.Maximum = 255;
            ColScrBarMaxB.Value = 255;

            ColLblMaxBVal.Text = ColScrBarMaxB.Value.ToString();
            ColLblMaxB.BackColor = Color.Blue;

            // end region TabPage Color

            // TabPage System

            string[] sItems = {"Firework", "Firework - Single", "Fountain", "Fire or Meteor", "Snow", "..."};
            SysCbb.DataSource = sItems;

            // end region Initialize TabControl

            DEFAULT_FIREWORK_VALUES();
        }

        /// <summary>
        /// update all members with values from gui
        /// </summary>
        private void SetdefaultfromGui()
        {
            _minPosX = PosScrBarMinPosX.Value;
            _maxPosX = PosScrBarMaxPosX.Value;
            _minPosY = PosScrBarMinPosY.Value;
            _maxPosY = PosScrBarMaxPosY.Value;
            _speed = (OptScrBarSpeed.Value / 100f);
            _angle = OptScrBarAgl.Value;
            _wind = (OptScrBarWind.Value / 100f);
            _gravity = (OptScrBarGrvy.Value / 100f);
            _particleAmount = OptScrBarNumPart.Value;
            _life = OptScrBarLife.Value;
            _cycle = OptScrBarCyle.Value;
            _interval = OptScrBarInter.Value;
            _minRed = ColScrBarMinR.Value;
            _minGreen = ColScrBarMinG.Value;
            _minBlue = ColScrBarMinB.Value;
            _maxRed = ColScrBarMaxR.Value;
            _maxGreen = ColScrBarMaxG.Value;
            _maxBlue = ColScrBarMaxB.Value;

            UpdateLabels();
        }

        /// <summary>
        /// update all labels with values
        /// </summary>
        private void UpdateLabels()
        {
            PosLblMinPosXVal.Text = PosScrBarMinPosX.Value.ToString();
            PosLblMinPosYVal.Text = PosScrBarMinPosY.Value.ToString();
            PosLblMaxPosXVal.Text = PosScrBarMaxPosX.Value.ToString();
            PosLblMaxPosYVal.Text = PosScrBarMaxPosY.Value.ToString();
            OptLblVeloSpdVal.Text = (OptScrBarSpeed.Value / 100f).ToString();
            OptLblVeloAglVal.Text = OptScrBarAgl.Value.ToString();
            OptLblWindVal.Text = (OptScrBarWind.Value / 100f).ToString();
            OptLblGrvyVal.Text = (OptScrBarGrvy.Value / 100f).ToString();
            OptLblNumPartVal.Text = OptScrBarNumPart.Value.ToString();
            OptLblLifeVal.Text = OptScrBarLife.Value.ToString();
            OptLblCycleVal.Text = OptScrBarCyle.Value.ToString();
            OptLblCycleVal.Text = OptScrBarInter.Value.ToString();
        }

        /// <summary>
        /// change the system
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysCbb_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (SysCbb.SelectedItem.ToString())
            {
                // to firework
                case "Firework":
                    DEFAULT_FIREWORK_VALUES();
                    break;
                // to fountain
                case "Fountain":
                    DEFAULT_FONTAIN_VALUES();
                    break;
                // to single firework
                case "Firework - Single":
                    DEFAULT_FIREWORKSINGLE_VALUES();
                    break;
                case "Fire or Meteor":
                    DEFAULT_METEOR_VALUES();
                    break;
                case "Snow":
                    DEFAULT_SNOW_VALUES();
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// set default values for fireword-effect
        /// </summary>
        private void DEFAULT_FIREWORK_VALUES()
        {
            // set min pos x & y
            PosScrBarMinPosX.Value = 1;
            PosScrBarMinPosY.Value = 1;

            // set max pos x & y
            PosScrBarMaxPosX.Value = (DrawBox.Width - 1);
            PosScrBarMaxPosY.Value = (DrawBox.Height - 1);

            // set speed
            OptScrBarSpeed.Value = 100;
            // set angle
            OptScrBarAgl.Value = 360;
            // set wind
            OptScrBarWind.Value = 0;
            // set gravity
            OptScrBarGrvy.Value = 0;
            // set number of particles
            OptScrBarNumPart.Value = 500;
            // set life
            OptScrBarLife.Value = 2;
            // set cycle
            OptScrBarCyle.Value = 50;
            // set interval
            OptScrBarInter.Value = 125;

            // set max rgb
            ColScrBarMaxR.Value = 255;
            ColScrBarMaxG.Value = 255;
            ColScrBarMaxB.Value = 255;

            // set min rgb
            ColScrBarMinR.Value = 0;
            ColScrBarMinG.Value = 0;
            ColScrBarMinB.Value = 0;

            SetdefaultfromGui();
        }

        private void DEFAULT_METEOR_VALUES()
        {
            // set min pos x & y
            PosScrBarMinPosX.Value = (DrawBox.Width / 2);
            PosScrBarMinPosY.Value = (DrawBox.Height / 2);

            // set max pos x & y
            PosScrBarMaxPosX.Value = (DrawBox.Width / 2);
            PosScrBarMaxPosY.Value = (DrawBox.Height / 2);

            // set speed
            OptScrBarSpeed.Value = 29;
            // set angle
            OptScrBarAgl.Value = 360;
            // set wind
            OptScrBarWind.Value = 2;
            // set gravity
            OptScrBarGrvy.Value = -5;
            // set number of particles
            OptScrBarNumPart.Value = 60;
            // set life
            OptScrBarLife.Value = 4;
            // set cycle
            OptScrBarCyle.Value = 100;
            // set interval
            OptScrBarInter.Value = 70;

            // set max rgb
            ColScrBarMaxR.Value = 255;
            ColScrBarMaxG.Value = 0;
            ColScrBarMaxB.Value = 0;

            // set min rgb
            ColScrBarMinR.Value = 200;
            ColScrBarMinG.Value = 0;
            ColScrBarMinB.Value = 0;

            SetdefaultfromGui();
        }

        private void DEFAULT_SNOW_VALUES()
        {
            // set min pos x & y
            PosScrBarMinPosX.Value = 1;
            PosScrBarMinPosY.Value = 1;

            // set max pos x & y
            PosScrBarMaxPosX.Value = (DrawBox.Width - 1);
            PosScrBarMaxPosY.Value = 1;

            // set speed
            OptScrBarSpeed.Value = 15;
            // set angle
            OptScrBarAgl.Value = 180;
            // set wind
            OptScrBarWind.Value = 0;
            // set gravity
            OptScrBarGrvy.Value = 2;
            // set number of particles
            OptScrBarNumPart.Value = 1;
            // set life
            OptScrBarLife.Value = 6;
            // set cycle
            OptScrBarCyle.Value = 500;
            // set interval
            OptScrBarInter.Value = 25;

            // set max rgb
            ColScrBarMaxR.Value = 255;
            ColScrBarMaxG.Value = 255;
            ColScrBarMaxB.Value = 255;

            // set min rgb
            ColScrBarMinR.Value = 255;
            ColScrBarMinG.Value = 255;
            ColScrBarMinB.Value = 255;

            SetdefaultfromGui();
        }

        private void DEFAULT_FIREWORKSINGLE_VALUES()
        {
            // set min pos x & y
            PosScrBarMinPosX.Value = (DrawBox.Width / 2);
            PosScrBarMinPosY.Value = (DrawBox.Height / 2);

            // set max pos x & y
            PosScrBarMaxPosX.Value = (DrawBox.Width / 2);
            PosScrBarMaxPosY.Value = (DrawBox.Height / 2);

            // set speed
            OptScrBarSpeed.Value = 100;
            // set angle
            OptScrBarAgl.Value = 360;
            // set wind
            OptScrBarWind.Value = 0;
            // set gravity
            OptScrBarGrvy.Value = 0;
            // set number of particles
            OptScrBarNumPart.Value = 1000;
            // set life
            OptScrBarLife.Value = 4;
            // set cycle
            OptScrBarCyle.Value = 1;
            // set interval
            OptScrBarInter.Value = 50;

            // set max rgb
            ColScrBarMaxR.Value = 255;
            ColScrBarMaxG.Value = 255;
            ColScrBarMaxB.Value = 255;

            // set min rgb
            ColScrBarMinR.Value = 0;
            ColScrBarMinG.Value = 0;
            ColScrBarMinB.Value = 0;

            SetdefaultfromGui();
        }

        /// <summary>
        /// set the default values for a fountain-effect
        /// </summary>
        private void DEFAULT_FONTAIN_VALUES()
        {
            // set min pos x & y
            PosScrBarMinPosX.Value = (DrawBox.Width - 1) / 2;
            PosScrBarMinPosY.Value = (DrawBox.Height - 1) / 2;

            // set max pos x & y
            PosScrBarMaxPosX.Value = (DrawBox.Width - 1) / 2;
            PosScrBarMaxPosY.Value = (DrawBox.Height - 1) / 2;

            // set speed
            OptScrBarSpeed.Value = 350;
            // set angle
            OptScrBarAgl.Value = 22;
            // set wind
            OptScrBarWind.Value = 0;
            // set gravity
            OptScrBarGrvy.Value = 5;
            // set number of particles
            OptScrBarNumPart.Value = 25;
            // set life
            OptScrBarLife.Value = 3;
            // set cycle
            OptScrBarCyle.Value = 200;
            // set interval
            OptScrBarInter.Value = 100;

            // set max rgb
            ColScrBarMaxR.Value = 25;
            ColScrBarMaxG.Value = 75;
            ColScrBarMaxB.Value = 255;

            // set min rgb
            ColScrBarMinR.Value = 0;
            ColScrBarMinG.Value = 0;
            ColScrBarMinB.Value = 0;

            SetdefaultfromGui();
        }


        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            //depricated
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            _particlethread.Abort();
        }


        private void DrawBox_Click(object sender, EventArgs e)
        {
            //depricated
        }

        /// <summary>
        /// start the particle system through mouse-click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawBox_MouseClick(object sender, MouseEventArgs e)
        {
            if (!_isEmitting)
            {
                _emitterThread = new Thread(EmitParticles);
                _emitterThread.Start();
            }
        }


        // region ScrollBar Eventhandler

        /// <summary>
        /// define minimum X position based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarMinPosX_Scroll(object sender, ScrollEventArgs e)
        {
            _minPosX = (float) e.NewValue;
            PosLblMinPosXVal.Text = e.NewValue.ToString();

            PosScrBarMaxPosX.Minimum = e.NewValue;
        }

        /// <summary>
        /// define minimum Y position for emission based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarMinPosY_Scroll(object sender, ScrollEventArgs e)
        {
            _minPosY = (float) e.NewValue;
            PosLblMinPosYVal.Text = e.NewValue.ToString();

            PosScrBarMaxPosY.Minimum = e.NewValue;
        }

        /// <summary>
        /// define maximum X position for emission based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PosScrBarMaxPosX_Scroll(object sender, ScrollEventArgs e)
        {
            _maxPosX = (float) e.NewValue;
            PosLblMaxPosXVal.Text = e.NewValue.ToString();

            PosScrBarMinPosX.Maximum = e.NewValue;
        }

        /// <summary>
        ///    define maximum Y position for emission based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PosScrBarMaxPosY_Scroll(object sender, ScrollEventArgs e)
        {
            _maxPosY = (float) e.NewValue;
            PosLblMaxPosYVal.Text = e.NewValue.ToString();

            PosScrBarMinPosY.Maximum = e.NewValue;
        }

        /// <summary>
        /// change value of _speed to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarSpeed_Scroll(object sender, ScrollEventArgs e)
        {
            _speed = (float) (e.NewValue / 100f);
            OptLblVeloSpdVal.Text = _speed.ToString();
        }

        /// <summary>
        /// change value of _angle to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarAgl_Scroll(object sender, ScrollEventArgs e)
        {
            _angle = (float) e.NewValue;
            OptLblVeloAglVal.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// change value of _wind to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarWind_Scroll(object sender, ScrollEventArgs e)
        {
            _wind = (float) (e.NewValue / 100f);
            OptLblWindVal.Text = _wind.ToString();
        }

        /// <summary>
        /// change value of _gravity to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarGrvy_Scroll(object sender, ScrollEventArgs e)
        {
            _gravity = (float) (e.NewValue / 100f);
            OptLblGrvyVal.Text = _gravity.ToString();
        }

        /// <summary>
        /// change value of _numPart to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarNumPart_Scroll(object sender, ScrollEventArgs e)
        {
            _particleAmount = e.NewValue;
            OptLblNumPartVal.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// change value of _life to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarLife_Scroll(object sender, ScrollEventArgs e)
        {
            _life = e.NewValue;
            OptLblLifeVal.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// change value of _cycle to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarCyle_Scroll(object sender, ScrollEventArgs e)
        {
            _cycle = e.NewValue;
            OptLblCycleVal.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// change value of _interval to a new value based on scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OptScrBarEmission_Scroll(object sender, ScrollEventArgs e)
        {
            _interval = e.NewValue;
            OptLblInterVal.Text = e.NewValue.ToString();
        }

        /// <summary>
        /// change value of _minR to a new value based on scrollbar
        /// and set a new minimum for MaxR-Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColScrBarMinR_Scroll(object sender, ScrollEventArgs e)
        {
            _minRed = e.NewValue;
            ColLblMinRVal.Text = e.NewValue.ToString();
            ColScrBarMaxR.Minimum = e.NewValue;
        }

        /// <summary>
        /// change value of _minG to a new value based on scrollbar
        /// and set a new minimum for MaxG-Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColScrBarMinG_Scroll(object sender, ScrollEventArgs e)
        {
            _minGreen = e.NewValue;
            ColLblMinGVal.Text = e.NewValue.ToString();
            ColScrBarMaxG.Minimum = e.NewValue;
        }

        /// <summary>
        /// change value of _minB to a new value based on scrollbar
        /// and set a new minimum for MaxB-Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColScrBarMinB_Scroll(object sender, ScrollEventArgs e)
        {
            _minBlue = e.NewValue;
            ColLblMinBVal.Text = e.NewValue.ToString();
            ColScrBarMaxB.Minimum = e.NewValue;
        }

        /// <summary>
        /// change value of _maxR to a new value based on scrollbar
        /// and set a new maximum for minR-Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColScrBarMaxR_Scroll(object sender, ScrollEventArgs e)
        {
            _maxRed = e.NewValue;
            ColLblMaxRVal.Text = e.NewValue.ToString();
            ColScrBarMinR.Maximum = e.NewValue;
        }

        /// <summary>
        /// change value of _maxg to a new value based on scrollbar
        /// and set a new maximum for minG-Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColScrBarMaxG_Scroll(object sender, ScrollEventArgs e)
        {
            _maxGreen = e.NewValue;
            ColLblMaxGVal.Text = e.NewValue.ToString();
            ColScrBarMinG.Maximum = e.NewValue;
        }

        /// <summary>
        /// change value of _maxB to a new value based on scrollbar
        /// and set a new maximum for minB-Scrollbar
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ColScrBarMaxB_Scroll(object sender, ScrollEventArgs e)
        {
            _maxBlue = e.NewValue;
            ColLblMaxBVal.Text = e.NewValue.ToString();
            ColScrBarMinB.Maximum = e.NewValue;
        }

        // end region ScrollBar Eventhandler


        /// <summary>
        /// start the system 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SysBtnStart_Click(object sender, EventArgs e)
        {
            if (!_isEmitting)
            {
                _emitterThread = new Thread(EmitParticles);
                _emitterThread.Start();
            }
        }

        /// <summary>
        /// adjust the values of position-scrollbars, if the size of drawbox changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DrawBox_SizeChanged(object sender, EventArgs e)
        {
            //change x minimum value to fit box size
            if (DrawBox.Width <= PosScrBarMinPosX.Value)
                PosScrBarMinPosX.Value = DrawBox.Width - 1;

            //change X maximum value to fit box size
            if (DrawBox.Width <= PosScrBarMaxPosX.Value)
                PosScrBarMaxPosX.Value = DrawBox.Width - 1;
            //change x maximum slider max value to fit box size
            PosScrBarMaxPosX.Maximum = DrawBox.Width - 1;


            if (DrawBox.Height <= PosScrBarMinPosY.Value)
                PosScrBarMinPosY.Value = DrawBox.Height - 1;

            if (DrawBox.Height <= PosScrBarMaxPosY.Value)
                PosScrBarMaxPosY.Value = DrawBox.Height - 1;

            PosScrBarMaxPosY.Maximum = DrawBox.Height - 1;
        }
    }
}