using System.Text;
using FarseerPhysics.DemoBaseSilverlight;
using FarseerPhysics.DemoBaseSilverlight.DemoShare;
using FarseerPhysics.DemoBaseSilverlight.ScreenSystem;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;

namespace FarseerPhysics.SimpleSamplesSilverlight
{
    internal class Demo4Screen : GameScreen
    {
        private Agent _agent;
        private Objects _circles;
        private Objects _gears;
        private Objects _rectangles;
        private Objects _stars;

        #region IDemoScreen Members

        public string GetTitle()
        {
            return "Demo5: Collision Categories";
        }

       

        #endregion

        public override void Initialize()
        {
            World = new World(Vector2.Zero);

            base.Initialize();
        }

        public override void LoadContent()
        {
            //Cat1=Circles, Cat2=Rectangles, Cat3=Gears, Cat4=Stars
            _agent = new Agent(World, Vector2.Zero);

            //Collide with all but stars
            _agent.CollisionCategories = Category.All & ~Category.Cat4;
            _agent.CollidesWith = Category.All & ~Category.Cat4;

            Vector2 startPosition = new Vector2(-20, 16);
            Vector2 endPosition = new Vector2(20, 16);
            _circles = new Objects(World, startPosition, endPosition, 15, 1, ObjectType.Circle);

            //Collide with itself only
            _circles.CollisionCategories = Category.Cat1;
            _circles.CollidesWith = Category.Cat1;

            startPosition = new Vector2(-20, -16);
            endPosition = new Vector2(20, -16);
            _rectangles = new Objects(World, startPosition, endPosition, 15, 2, ObjectType.Rectangle);

            //Collides with itself only
            _rectangles.CollisionCategories = Category.Cat2;
            _rectangles.CollidesWith = Category.Cat2;

            startPosition = new Vector2(-20, -10);
            endPosition = new Vector2(-20, 10);
            _gears = new Objects(World, startPosition, endPosition, 5, 1, ObjectType.Gear);

            //Collides with stars
            _gears.CollisionCategories = Category.Cat3;
            _gears.CollidesWith = Category.Cat3 | Category.Cat4;

            startPosition = new Vector2(20, -10);
            endPosition = new Vector2(20, 10);
            _stars = new Objects(World, startPosition, endPosition, 5, 1, ObjectType.Star);

            //Collides with gears
            _stars.CollisionCategories = Category.Cat4;
            _stars.CollidesWith = Category.Cat3 | Category.Cat4;

            base.LoadContent();
        }
    }
}