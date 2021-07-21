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

        List<double> w1xList_pf2 = new List<double>();
        List<double> w1yList_pf2 = new List<double>();
        List<double> w2xList_pf2 = new List<double>();
        List<double> w2yList_pf2 = new List<double>();
        List<double> w3xList_pf2 = new List<double>();
        List<double> w3yList_pf2 = new List<double>();

        int NUMBER_OF_ROBOTS;
        int NUMBER_OF_SHARKS;
        int NUMBER_OF_PARTICLEFILTERS;
        List<ParticleFilter> ParticleFilterList = new List<ParticleFilter>();
        Boolean stopHere = true;
        List<double> errorList = new List<double>();
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

            create_robots();
            
        }

        private void update_weight_lists(int index)
        {
            if( index == 0)
            {
                w1xList_pf1 = ParticleFilterList[index].w1_list_x;
                w1yList_pf1 = ParticleFilterList[index].w1_list_y;
                w2xList_pf1 = ParticleFilterList[index].w2_list_x;
                w2yList_pf1 = ParticleFilterList[index].w2_list_y;
                w3yList_pf1 = ParticleFilterList[index].w3_list_y;
                w3xList_pf1 = ParticleFilterList[index].w3_list_x;
            }
            if (index == 2)
            { 
                w1xList_pf2 = ParticleFilterList[index].w1_list_x;
                w1yList_pf2 = ParticleFilterList[index].w1_list_y;
                w2xList_pf2 = ParticleFilterList[index].w2_list_x;
                w2yList_pf2 = ParticleFilterList[index].w2_list_y;
                w3yList_pf2 = ParticleFilterList[index].w3_list_y;
                w3xList_pf2 = ParticleFilterList[index].w3_list_x;
            }
           
        }
        public void update_real_range_list(int SharkNumber)
        {
            this.real_range_list = new List<double>();
            for (int i = 0; i < MyGlobals.robot_list.Count; i++)
            {
                double real_range1 = MyGlobals.robot_list[i].calc_range_error_real(MyGlobals.shark_list[SharkNumber]);
                this.real_range_list.Add(real_range1);
            }
        }

        public void update_noisy_range_list(int SharkNumber)
        {
            this.real_range_list = new List<double>();
            for (int i = 0; i < MyGlobals.robot_list.Count; i++)
            {
                double real_range1 = MyGlobals.robot_list[i].calc_range_error(MyGlobals.shark_list[SharkNumber]);
                this.real_range_list.Add(real_range1);
            }
        }

        public int what_shark(int index)
        {
            int sharkNumber = 0;
            if (index%NUMBER_OF_SHARKS == 0)
            {
                sharkNumber = NUMBER_OF_SHARKS;
            }
            else 
            {
                sharkNumber = index % NUMBER_OF_SHARKS;
            }
            int sharkIndex = sharkNumber - 1;
            return sharkIndex;
            // return sharkNUmber

        }
        public void create_robots()
        {
            for (int i = 0; i < NUMBER_OF_ROBOTS; i++)
            {
                Robot auv = new Robot();
                MyGlobals.robot_list.Add(auv);
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
        public void get_measurements()
        {
            get_noisy_measurements();
            //get_real_measurements();
        }

        public void get_noisy_measurements()
        {
            update_robot_list();
            update_noisy_range_list();
        }

        public void get_real_measurements()
        {
            update_robot_list();
            update_real_range_list();
        }

        public void createParticleFilters()
        {
            for (int p = 0; p < NUMBER_OF_PARTICLEFILTERS; ++p)
            {
                ParticleFilter p1 = new ParticleFilter();
                ParticleFilterList.Add(p1);
                p1.create();
            }
            

        }

        private void getParticleCoordinates()
        {
            NUMBER_OF_ROBOTS = 2;
            NUMBER_OF_SHARKS = 2;
            NUMBER_OF_PARTICLEFILTERS = NUMBER_OF_ROBOTS * NUMBER_OF_SHARKS;

            

            create_simulation();
            createParticleFilters();
            //updates PFs
            int index = 0;
            foreach (ParticleFilter p1 in ParticleFilterList)
            {
                index += 1;
                int sharkNumber = what_shark(index);
                this.update_noisy_range_list(sharkNumber);
                p1.update_weights(this.real_range_list);
                p1.NUMBER_OF_AUVS = MyGlobals.robot_list.Count;
            }
            double count = 0;
            while (stopHere)
            {
                count += 10;
                if(count %80 == 0)
                {   
                    for (int i = 0; i < NUMBER_OF_SHARKS; ++i)
                    {
                        MyGlobals.shark_list[i].update_shark();
                    }
                }
                //get real_measurements
                // work on updating PF and real_range_measurements for each PF
                get_measurements();
                foreach (ParticleFilter p1 in ParticleFilterList)
                {
                    p1.update();
                    p1.update_weights(this.real_range_list);
                    p1.correct();  
                }
                
                
                foreach (ParticleFilter p1 in ParticleFilterList)
                {
                    p1.weight_list_x();
                    p1.weight_list_y();
                }
                


                // make coordinate list 
                update_weight_lists();

            }
                // update_robot_location();

                // range_error creator
                // create_range_error();
            }
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
         private void UpdateMap()
        {

            chart1.Series["Weight1"].Points.Clear();
            chart1.Series["Weight2"].Points.Clear();
            chart1.Series["Weight3"].Points.Clear();
            chart1.Series["2ndWeight1"].Points.Clear();
            chart1.Series["2ndWeight2"].Points.Clear();
            chart1.Series["2ndWeight3"].Points.Clear();
            chart1.Series["Shark"].Points.Clear();
            chart1.Series["Shark2"].Points.Clear();
            chart1.Series["AUV1"].Points.Clear();
            chart1.Series["AUV2"].Points.Clear();
            //chart1.Series["AUV3"].Points.Clear();
            chart1.Series["Shark"].Points.AddXY(MyGlobals.shark_list[0].X, MyGlobals.shark_list[0].Y);
            chart1.Series["Shark2"].Points.AddXY(MyGlobals.shark_list[1].X, MyGlobals.shark_list[1].Y);
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


            for (int i = 0; i < w1xList_pf2.Count; ++i)
            {
                chart1.Series["2ndWeight1"].Points.AddXY(w1xList_pf2[i], w1yList_pf2[i]);
            }
            for (int i = 0; i < w2xList_pf2.Count; ++i)
            {
                chart1.Series["2ndWeight2"].Points.AddXY(w2xList_pf2[i], w2yList_pf2[i]);
            }
            for (int i = 0; i < w3xList_pf2.Count; ++i)
            {
                chart1.Series["2ndWeight3"].Points.AddXY(w3xList_pf2[i], w3yList_pf2[i]);
            }

           //double hey = particle_filter.r1.robot_list_x.Count;
            //double yes = particle_filter.r1.robot_list_y[0];
            chart1.Series["AUV1"].Points.AddXY(MyGlobals.robot_list[0].X, MyGlobals.robot_list[0].Y);
            chart1.Series["AUV2"].Points.AddXY(MyGlobals.robot_list[1].X, MyGlobals.robot_list[1].Y);
            //chart1.Series["AUV3"].Points.AddXY(MyGlobals.robot_list[2].X, MyGlobals.robot_list[2].Y);

        }
        private void chart1_Click(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            cpuThread = new Thread(new ThreadStart(this.getParticleCoordinates));
            cpuThread.IsBackground = true;
            cpuThread.Start();
        }
    }
}
