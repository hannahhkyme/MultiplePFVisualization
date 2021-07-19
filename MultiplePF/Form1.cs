using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Threading;
namespace MultiplePF
{
    public partial class Form1 : Form
    {
        private Thread cpuThread;
        List<double> w1xList_pf1 = new List<double>();
        List<double> w1yList_pf1 = new List<double>();
        List<double> w2xList_pf1 = new List<double>();
        List<double> w2yList_pf1 = new List<double>();
        List<double> w3xList_pf1 = new List<double>();
        List<double> w3yList_pf1 = new List<double>();
        ParticleFilter particle_filter = new ParticleFilter();

        Boolean stopHere = true;
        List<double> errorList = new List<double>();
        List<double> errorList2 = new List<double>();
        List<double> PredictedSharkXList = new List<double>();
        List<double> PredictedSharkYList = new List<double>();
        List<double> real_range_list = new List<double>();

        public Form1()
        {
            InitializeComponent();
            MyGlobals.robot_list = new List<Robot>();
            this.real_range_list = new List<double>();

        }
        public void create_simulation()
        {
            MyGlobals.shark_list.Add(MyGlobals.s1);
            Robot robot1 = new Robot();
            robot1.Y = 0;
            robot1.X = 45;
            Robot robot2 = new Robot();
            robot2.X = -100;
            robot2.Y = 45;
            MyGlobals.robot_list.Add(robot1);
            MyGlobals.robot_list.Add(robot2);
            particle_filter.create();
        }


        private void update_weight_lists()
        {
            w1xList_pf1 = particle_filter.w1_list_x;
            w1yList_pf1 = particle_filter.w1_list_y;
            w2xList_pf1 = particle_filter.w2_list_x;
            w2yList_pf1 = particle_filter.w2_list_y;
            w3yList_pf1 = particle_filter.w3_list_y;
            w3xList_pf1 = particle_filter.w3_list_x;




        }
        private void shark_motion()
        {
            particle_filter.correct();
            
          
        }
        /*
        private void create_range_error()
        {
            List<double> predictedList = particle_filter.predicting_shark_location();
            double currentError = Math.Abs(calculateRangeError(predictedList));
            calculate_predicted_shark(predictedList);
            errorList.Add(currentError);

            List<double> predictedList2 = particle_filter.predicting_shark_location();
            double currentError2 = Math.Abs(calculateRangeError2(predictedList2));
            errorList2.Add(currentError2);
        }

        private void update_robot_location()
        {
            particle_filter.r1.update_robot_position();
            particle_filter.r1.create_robot_list();


        }
        */
        public void update_real_range_list()
        {
            this.real_range_list = new List<double>();
            for (int i = 0; i < MyGlobals.robot_list.Count; i++)
            {
                double real_range1 = MyGlobals.robot_list[i].calc_range_error(MyGlobals.shark_list[0]);
                this.real_range_list.Add(real_range1);
            }
        }
        public void update_robot_list()
        {
            for (int i = 0; i < MyGlobals.robot_list.Count; i++)
            {
                MyGlobals.robot_list[i].update_robot_position();
            }
        }

        public double calc_range_error(List<double> predict_shark_location)
        {
            double x_component = Math.Pow((MyGlobals.s1.X - predict_shark_location[0]), 2);
            double y_component = Math.Pow((MyGlobals.s1.Y - predict_shark_location[1]), 2);
            double range_error = Math.Sqrt(x_component + y_component);
            return range_error;
        }
        private void getParticleCoordinates()
        {
            create_simulation();
            this.update_real_range_list();
            particle_filter.update_weights(this.real_range_list);

            double count = 0;
            while (stopHere)
            {
                count += 10;
                MyGlobals.shark_list[0].update_shark();
                update_real_range_list();

                particle_filter.update();
                particle_filter.update_weights(this.real_range_list);
                particle_filter.correct();

                List<double> predict_shark_location = new List<double>();
                predict_shark_location = particle_filter.predicting_shark_location();


                particle_filter.weight_list_x();
                particle_filter.weight_list_y();


                // make coordinate list 
                update_weight_lists();

                // update_robot_location();

                // range_error creator
                // create_range_error();



                if (chart1.IsHandleCreated)
                {
                    this.Invoke((MethodInvoker)delegate { UpdateMap(); });
                }
                else
                {
                    //......
                }


                Thread.Sleep(100);
            }
        }
        /*
        private double calculateRangeError(List<double> meanList)
        {
            double realRange = particle_filter.calc_range_error();
            // calc particle range
            Particle meanParticle = new Particle();
            meanParticle.X = meanList[0];
            meanParticle.Y = meanList[1];
            double particleRange = meanParticle.calc_particle_range(particle_filter.r1.X, particle_filter.r1.Y);
            double rangeError = realRange - particleRange;
            return rangeError;
        }
        private double calculateRangeError2(List<double> meanList)
        {
            double realRange = particle_filter2.calc_range_error();
            // calc particle range
            Particle meanParticle = new Particle();
            meanParticle.X = meanList[0];
            meanParticle.Y = meanList[1];
            double particleRange = meanParticle.calc_particle_range(particle_filter2.r1.X, particle_filter2.r1.Y);
            double rangeError = realRange - particleRange;
            return rangeError;
        }

        private void calculate_predicted_shark(List<double> meanList)
        {
            PredictedSharkXList = new List<double>();
            PredictedSharkYList = new List<double>();
            PredictedSharkXList.Add(meanList[0]);
            PredictedSharkYList.Add(meanList[1]);
        }
        */
        private void UpdateMap()
        {

            chart1.Series["Weight1"].Points.Clear();
            chart1.Series["Weight2"].Points.Clear();
            chart1.Series["Weight3"].Points.Clear();
            chart1.Series["Shark"].Points.Clear();
           //hart1.Series["AUV1"].Points.Clear();
            //art1.Series["AUV2"].Points.Clear();

            //art1.Series["Shark"].Points.AddXY(MyGlobals.shark_list[0].X, MyGlobals.shark_list[0].Y);

            //add in one of pf2
            for (int i = 0; i < w1xList_pf1.Count; ++i)
            {
                chart1.Series["Weight1"].Points.AddXY(w1xList_pf1[i], w1yList_pf1[i]);
            }
            for (int i = 0; i < w2xList_pf1.Count; ++i)
            {
                chart1.Series["Weight2"].Points.AddXY(w2xList_pf1[i], w2yList_pf1[i]);
            }
            for (int i = 0; i < w3xList_pf1.Count; ++i)
            {
                chart1.Series["Weight3"].Points.AddXY(w3xList_pf1[i], w3yList_pf1[i]);
            }

           //ouble hey = particle_filter.r1.robot_list_x.Count;
            //double yes = particle_filter.r1.robot_list_y[0];
            //art1.Series["AUV1"].Points.AddXY(MyGlobals.robot_list[0].X, MyGlobals.robot_list[0].Y);
            //art1.Series["AUV2"].Points.AddXY(MyGlobals.robot_list[1].X, MyGlobals.robot_list[1].Y);

        }
        /*
        private void UpdateChart1()
        {
            errorchart1.Series["Range Error"].Points.Clear();
            for (int i = 0; i < errorList.Count; ++i)
            {
                errorchart1.Series["Range Error"].Points.AddY(errorList[i]);
            }

        }
        */
        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void chart1_Click(object sender, EventArgs e)
        {

        }


        private void Form1_Load_1(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            cpuThread = new Thread(new ThreadStart(this.getParticleCoordinates));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }


        /*
private void button2_Click(object sender, EventArgs e)
{
   cpuThread.Abort();
   stopHere = false;
}

*/
    }
}
