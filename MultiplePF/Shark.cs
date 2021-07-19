using System;
using System.Collections.Generic;
namespace MultiplePF
{
    public class Shark
    {
        Random random_num = new Random();
        // SETS TYPE OF MEMBER VARIABLE
        public double X;
        public double Y;
        public double Z;
        public double THETA;
        public double V;
        public List<double> shark_list_x;
        public List<double> shark_list_y;

        public Shark()
        {
            this.X = 45;
            this.Y = 45;
            this.Z = 3.0;
            this.THETA = Math.PI / 3;
            this.V = 3.0;
            this.shark_list_x = new List<double>();
            this.shark_list_y = new List<double>();

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
        public void create_shark_list()
        {
            this.shark_list_x = new List<double>();
            this.shark_list_y = new List<double>();

            this.shark_list_x.Add(this.X);
            this.shark_list_y.Add(this.Y);
        }
        public void update_shark()
        {
            // should update the sharks position after 
            double RANDOM_VELOCITY = 2;
            double RANDOM_THETA = Math.PI / 2;

            // updates velocity of particles
            this.V += MyGlobals.random_num.NextDouble() * RANDOM_VELOCITY;
            this.V = velocity_wrap(this.V);

            //change theta & pass through angle_wrap
            this.THETA += MyGlobals.random_num.NextDouble() * (2 * RANDOM_THETA) - RANDOM_THETA;
            this.THETA = angle_wrap(this.THETA);

            // change x & y coordinates to match
            this.X += this.V * Math.Cos(this.THETA);
            this.Y += this.V * Math.Sin(this.THETA);

        }
        public bool get_shark_measurement()
        {
            bool get_measurement = new bool();
            int get_measurement_value = MyGlobals.random_num.Next(1, 10);
            if (get_measurement_value <= 3)
            {
                get_measurement = false;
            }
            else
            {
                get_measurement = true;
            }
            return get_measurement;
        }
    }
}