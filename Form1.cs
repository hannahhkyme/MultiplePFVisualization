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
        List<List<double>> real_range_list = new List<List<double>>();
        public Form1()
        {
            InitializeComponent();
            MyGlobals.robot_list = new List<Robot>();
            this.real_range_list = new List<List<double>>();
        }
        public void create_simulation()
        {
            MyGlobals.shark_list.Add(MyGlobals.s1);
            //MyGlobals.shark_list.Add(MyGlobals.s2);
            create_robots();
            
        }
        public void create_robots()
        {
            for (int i = 0; i < NUMBER_OF_ROBOTS; i++)
            {
                Robot auv = new Robot();
                MyGlobals.robot_list.Add(auv);
            }
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
        public void update_robot_list()
        {
            for (int i = 0; i < MyGlobals.robot_list.Count; i++)
            {
                MyGlobals.robot_list[i].update_robot_position();
            }
        }
        public int what_robot(int index)
        {
            int robotNumber = index / NUMBER_OF_SHARKS;
            return robotNumber;
        }
        public int what_shark(int index)
        {
            int sharkNumber = 0;
            if (index % NUMBER_OF_SHARKS==0)
            {
                sharkNumber = 0;
            }
            else
            {
                sharkNumber = index % NUMBER_OF_SHARKS;
            }
            
            return sharkNumber;
        }
        
        //List<List<double>> real_range_list where the two lists inside are range lists per robot
        public void update_real_range_list(int robotNumber)
        {
            for (int r = 0; r < MyGlobals.robot_list.Count; r++)
            {
                List<double> robotRangeList = new List<double>();
                for (int s = 0; s < MyGlobals.shark_list.Count; s++)
                {
                    double real_range1 = MyGlobals.shark_list[s].calc_range_error_real(MyGlobals.robot_list[robotNumber]);
                    robotRangeList.Add(real_range1);
                }
                this.real_range_list.Add(robotRangeList);
            }
        }

        public void update_noisy_range_list()
        {
            for (int r = 0; r < MyGlobals.robot_list.Count; r++)
            {
                List<double> robotRangeList = new List<double>();
                for (int s = 0; s < MyGlobals.shark_list.Count; s++)
                {
                    double real_range1 = MyGlobals.shark_list[s].calc_range_error(MyGlobals.robot_list[r]);
                    robotRangeList.Add(real_range1);
                }
                this.real_range_list.Add(robotRangeList);
            }
        }
        public void get_measurements()
        {
            update_noisy_range_list();
            //get_real_measurements();
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
            if (index == 1)
            { 
                w1xList_pf2 = ParticleFilterList[index].w1_list_x;
                w1yList_pf2 = ParticleFilterList[index].w1_list_y;
                w2xList_pf2 = ParticleFilterList[index].w2_list_x;
                w2yList_pf2 = ParticleFilterList[index].w2_list_y;
                w3yList_pf2 = ParticleFilterList[index].w3_list_y;
                w3xList_pf2 = ParticleFilterList[index].w3_list_x;
            }
           
        }
        

        

        private void getParticleCoordinates()
        {
            NUMBER_OF_ROBOTS = 1;
            NUMBER_OF_SHARKS = 1;
            NUMBER_OF_PARTICLEFILTERS = NUMBER_OF_ROBOTS * NUMBER_OF_SHARKS;
            create_simulation();
            createParticleFilters();
            
            get_measurements();

            int index = 0;
            foreach (ParticleFilter p1 in ParticleFilterList)
            {
                index += 1;
                int SharkNumber = what_shark(index);
                p1.update_weights(this.real_range_list, SharkNumber);
                p1.NUMBER_OF_AUVS = MyGlobals.robot_list.Count;
                
            }
            double count = 0;
            while (stopHere)
            {
                //updates shark position
                count += 10;
                if(count %80 == 0)
                {   
                    for (int i = 0; i < NUMBER_OF_SHARKS; ++i)
                    {
                        MyGlobals.shark_list[i].update_shark();
                    }
                }
                //updates robot position
                for (int i = 0; i < NUMBER_OF_ROBOTS; ++i)
                {
                    MyGlobals.robot_list[i].update_robot_position();
                }
                //calculates real_range_list
                get_measurements();
                int index2 = 0;
                foreach (ParticleFilter p1 in ParticleFilterList)
                {
                    index2 += 1;
                    int SharkNumber2 = what_shark(index2);
                    p1.update();
                    p1.update_weights(this.real_range_list, SharkNumber2);
                    p1.correct();
                    
                }

                for (int i = 0; i < ParticleFilterList.Count(); ++i)
                {
                    //int sharkNumber = what_shark(i);
                    ParticleFilterList[i].weight_list_x();
                    ParticleFilterList[i].weight_list_y();
                    update_weight_lists(i);
                }
                
                //clear this.realrangelist

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
            chart1.Series["Weight1_2"].Points.Clear();
            chart1.Series["Weight2_2"].Points.Clear();
            chart1.Series["Weight3_2"].Points.Clear();
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
                chart1.Series["Weight1_2"].Points.AddXY(w1xList_pf2[i], w1yList_pf2[i]);
            }
            for (int i = 0; i < w2xList_pf2.Count; ++i)
            {
                chart1.Series["Weight2_2"].Points.AddXY(w2xList_pf2[i], w2yList_pf2[i]);
            }
            for (int i = 0; i < w3xList_pf2.Count; ++i)
            {
                chart1.Series["Weight3_2"].Points.AddXY(w3xList_pf2[i], w3yList_pf2[i]);
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
