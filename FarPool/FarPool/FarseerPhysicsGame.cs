using System;
using System.Windows.Controls;
using FarseerPhysics.DemoBaseSilverlight;
using FarseerPhysics.DemoBaseSilverlight.Components;
using FarseerPhysics.DemoBaseSilverlight.ScreenSystem;

namespace FarseerPhysics.SimpleSamplesSilverlight
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class FarseerPhysicsGame : Game
    {
        public FarseerPhysicsGame(UserControl userControl, Canvas drawingCanvas, Canvas debugCanvas)
            : base(userControl, drawingCanvas, debugCanvas)
        {
            TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 16);
            IsFixedTimeStep = true;

            //new-up components and add to Game.Components
            ScreenManager = new ScreenManager(this);
            Components.Add(ScreenManager);

            
            Demo4Screen demo4 = new Demo4Screen();
          
           
            ScreenManager.MainMenuScreen.AddMainMenuItem(demo4.GetTitle(), demo4);
            

            ScreenManager.GoToMainMenu();
            ScreenManager.MainMenuScreen.SelectEntry(0);
        }

        public ScreenManager ScreenManager { get; set; }
    }
}