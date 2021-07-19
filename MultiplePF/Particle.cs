using System;
using System.Collections.Generic;

namespace MultiplePF
{
    public class Particle
    {

        // SETS TYPE OF MEMBER VARIABLE
        public double X;
        public double Y;
        public double Z;
        public double THETA;
        public double V;
        public int INITIAL_PARTICLE_RANGE;

        public double W;
        public Particle()
        {
            // Class Members
            this.INITIAL_PARTICLE_RANGE = 150;
            this.X = MyGlobals.random_num.Next(-INITIAL_PARTICLE_RANGE, INITIAL_PARTICLE_RANGE);
            this.Y = MyGlobals.random_num.Next(-INITIAL_PARTICLE_RANGE, INITIAL_PARTICLE_RANGE);
            //this.X = 45;
            //this.Y = 45;
            this.Z = MyGlobals.random_num.Next(-INITIAL_PARTICLE_RANGE, INITIAL_PARTICLE_RANGE);
            this.V = MyGlobals.random_num.Next(0, 5);
            this.THETA = MyGlobals.random_num.NextDouble() * (2 * Math.PI) + -Math.PI;
            //this.THETA = Math.PI/3;
            this.W = 0.01;
        }
        static public double angle_wrap(double ang)
        {
            if (-Math.PI <= ang & ang <= Math.PI)
            {
                return ang;
            }
            else if (ang > Math.PI)
            {
                ang -= 2 * Math.PI;
                return angle_wrap(ang);
            }
            else
            {
                ang += 2 * Math.PI;
                return angle_wrap(ang);
            }
        }
        static public double velocity_wrap(double vel)
        {
            if (vel <= 2)
            {
                return vel;
            }
            else
            {
                vel += -2;
                return velocity_wrap(vel);
            }
        }

        public void updateParticles()
        {
            /*updates the particles location with random v and theta
                input (dt) is the amount of time the particles are "moving" 
                    generally set to .1, but it should be whatever the "time.sleep" is set to in the main loop
            */
            // pull out
            double RANDOM_VELOCITY = 2;
            double RANDOM_THETA = Math.PI / 2;

            // updates velocity of particles

            this.V += MyGlobals.random_num.NextDouble() * RANDOM_VELOCITY;
            this.V = velocity_wrap(this.V);

            //change theta & pass throughMyGlobals.angle_wrap
            this.THETA += MyGlobals.random_num.NextDouble() * (2 * RANDOM_THETA) - RANDOM_THETA;
            this.THETA = angle_wrap(this.THETA);

            // change x & y coordinates to match
            this.X += this.V * Math.Cos(this.THETA);
            this.Y += this.V * Math.Sin(this.THETA);

        }


        /*
         * public double calc_particle_alpha(double x_auv, double y_auv, double theta_auv)
        {
            // calculates the alpha value of a particle
            double particleAlpha = MyGlobals.angle_wrap(Math.Atan2((this.Y - y_auv), (this.X - x_auv)) - theta_auv);
            return particleAlpha;
        }
        */

        public double calc_particle_range(Robot currentRobot)// msg
        {
            //calculates the range from the particle to the auv
            double particleRange = Math.Sqrt(Math.Pow((currentRobot.Y - this.Y), 2) + Math.Pow((currentRobot.X - this.X), 2));
            return particleRange;
        }

        public void weight(double real_range1, double particle_range1, double real_range2, double particle_range2) //real_range_list
        {
            /* calculates the weight according to alpha, then the weight according
             * they are multiplied together to get the final weight */

            double E = 2.71828;
            double SIGMA_RANGE = 10.0;
            double DENOMINATOR2 = (Math.Pow(SIGMA_RANGE, 2));
            double dRange = Math.Pow(particle_range1 - real_range1, 2);
            double function_range1 = .001 + Math.Pow(E, -dRange / DENOMINATOR2);
            this.W = function_range1;
            double dRange2 = Math.Pow(particle_range2 - real_range2, 2);
            double function_range2 = .001 + Math.Pow(E, -dRange2 / DENOMINATOR2);
            this.W *= function_range2;
            /*
            Console.WriteLine("drange2");
            Console.WriteLine(dRange2);
            Console.WriteLine("function range 2");
            Console.WriteLine(function_range2);
            Console.WriteLine("weight");
            Console.WriteLine(this.W);
            */
        }
        public Particle DeepCopy()
        {
            Particle temp = (Particle)this.MemberwiseClone();
            temp.X = this.X;
            temp.Y = this.Y;
            temp.Z = this.Z;
            temp.THETA = this.THETA;
            temp.V = this.V;
            temp.W = this.W;
            temp.INITIAL_PARTICLE_RANGE = this.INITIAL_PARTICLE_RANGE;
            return temp;

        }
    }

    public static class MyGlobals

    {
        public static Random random_num = new Random();
        public static List<Robot> robot_list = new List<Robot>();
        public static Shark s1 = new Shark();
        public static List<Shark> shark_list = new List<Shark>();
        static public double angle_wrap(double ang)
        {
            if (-Math.PI <= ang & ang <= Math.PI)
            {
                return ang;
            }
            else if (ang > Math.PI)
            {
                ang -= 2 * Math.PI;
                return angle_wrap(ang);
            }
            else
            {
                ang += 2 * Math.PI;
                return angle_wrap(ang);
            }
        }
        static public double velocity_wrap(double vel)
        {
            if (vel <= 2)
            {
                return vel;
            }
            else
            {
                vel += -2;
                return velocity_wrap(vel);
            }
        }
    }

}