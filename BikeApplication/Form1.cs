using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;
using ZedGraph;

// this application parses .HRM files, which includes bike data, and processors it into an easy to view format. 

namespace BikeApplication
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            // run methods in order in the constructor
            InitializeComponent();
            
            
            
        }

        //delcare various lists and variables which will be uses through application
        private string[] data;  
        List<double> heartRate = new List<double>();
        List<double> speed = new List<double>();
        List<double> cadence = new List<double>();
        List<double> altitude = new List<double>();
        List<double> power = new List<double>();
        private double speedUnit = 1;
        private string stringSpeedUnit = "KPH";
        private string startTime;
        // create a new double array same length as data list
        

       






        // method all data and processors and displays header statistics
        public void getData(string path)
        {

            
            string line;
            StreamReader sr = new StreamReader(path);
            string smode = "";
            while ((line = sr.ReadLine()) != null)
            {

                if (line.StartsWith("SMode"))
                {
                    smode = line.Substring(6);
                    
                }


                // finds date in header then converts it to date format using string builder
                if(line.StartsWith("Date"))
                {
                    string date = line.Substring(5);
                    StringBuilder sb = new StringBuilder();
                    sb.Append(date);
                    sb.Insert(4, "-");
                    sb.Insert(7, "-");       
                    date = sb.ToString();
                    DateTime dt = Convert.ToDateTime(date);
                    label1.Text = date;
                }

                // finds start time in header and displays
                if (line.StartsWith("StartTime"))
                {
                    startTime = line.Substring(10);

                    label2.Text = startTime;

                }

                //finds length and displays
                if (line.StartsWith("Length"))
                {
                    string Length = line.Substring(7);

                    label3.Text = Length + (" (hh:mm:ss)");

                }
                // finds max heart rate and disaplys
                if (line.StartsWith("MaxHR"))
                {
                    string maxHR = line.Substring(6);

                    label4.Text = maxHR + (" (BPM)");

                }
                // finds resting heart rate and displays
                if (line.StartsWith("RestHR"))
                {
                    string restHR = line.Substring(7);

                    label5.Text = restHR + (" (BPM)");

                }
                // finds VO2max and displays
                if (line.StartsWith("VO2max"))
                {
                    string VO2max = line.Substring(7);

                    label6.Text = VO2max + (" (ml/min/kg)");

                }
                //finds weight and displays
                if (line.StartsWith("Weight"))
                {
                    string weight = line.Substring(7);

                    label7.Text = weight + (" (kg)");

                }
                // finds HR data then processes all data after that by inputting into data array for later use, breaks loop once data is found
                if(line.Contains("[HRData]"))
                {
                    data = sr.ReadToEnd().Split(new Char [] {'\t','\n'});
                    
                    break;

                }
                
            }
            sortData(smode);
            

        }
        private List<double> powerCalc(double[] doubleData)
        {
            int count = 0;
            double averagePower = 0;

            for (int i = 4; i < data.Length; i = i + 6)
            {
                
                // same as before with power
                power.Add(doubleData[i]);
                if (doubleData[i] != 0)
                {

                    averagePower = averagePower + doubleData[i];
                    count++;

                }

            }
            // work out max/averages and display
            double maximumPower = power.Max();
            label30.Text = maximumPower.ToString();

            averagePower = averagePower / count;
            averagePower = Math.Round(averagePower, 2);
            label22.Text = averagePower.ToString();


            return power;



        }
        private List<double> heartRateCalc(double[] doubleData)
        {

            double heartRateAverage = 0;
            int count = 0;

            // loops through the data array and picks out heart rate information
            for (int i = 0; i < data.Length; i = i + 6)
            {

                heartRate.Add(doubleData[i]);

                // if heart rate is zero ignore it when calculation averages (shouldnt ever reach 0!)
                if (doubleData[i] != 0)
                {
                    heartRateAverage = heartRateAverage + doubleData[i];
                    count++;

                }


            }

            // calculation averages, rounds them to two decimal places and displays
            heartRateAverage = heartRateAverage / count;
            heartRateAverage = Math.Round(heartRateAverage, 2);
            label9.Text = heartRateAverage.ToString();

            return heartRate;


        }
        private List<double> speedDistanceCalc(double[] doubleData)
        {

            // variables for speed and distance
            double speedAverage = 0;
            int count = 0;
            double distanceSpeed = 0;
            for (int i = 1; i < data.Length; i = i + 6)
            {

                doubleData[i] = doubleData[i] / 10;

                // same as other loop for speed but calculates based on users choice of units (MPH/KPH)
                doubleData[i] = doubleData[i] * speedUnit;


                speed.Add(doubleData[i]);




                //divide speed by 60 twice to get distance covered in 1 second and add to speedDistance variable to get total distance covered based on stats
                distanceSpeed = distanceSpeed + (doubleData[i] / 60 / 60);


                distanceSpeed = Math.Round(distanceSpeed, 2);

                string distance;

                if (stringSpeedUnit == "KPH")
                {
                    distance = " Kilometers";

                }
                else
                {

                    distance = " Miles";

                }
                //display distance with units
                label25.Text = distanceSpeed.ToString() + distance;

                if (doubleData[i] != 0)
                {
                    speedAverage = speedAverage + doubleData[i];
                    count++;

                }

            }
            // finds the highest value in speed and sotre
            double maximumSpeed = speed.Max();

            // display max speed
            label27.Text = maximumSpeed.ToString();

            //calc averages and display
            speedAverage = speedAverage / count;

            speedAverage = Math.Round(speedAverage, 2);
            label11.Text = speedAverage.ToString();

            return speed;


        }
        private List<double> cadenceCalc(double[] doubleData)
        {
            int count = 0;
            double averageCadence = 0;

            for (int i = 2; i < data.Length; i = i + 6)
            {
                // same loop as before but for cadence data
                cadence.Add(doubleData[i]);

                if (doubleData[i] != 0)
                {
                    averageCadence = averageCadence + doubleData[i];
                    count++;
                }

            }

            // max cadence calc
            double maximumCadence = cadence.Max();
            label29.Text = maximumCadence.ToString();
            // average cadence and display
            averageCadence = averageCadence / count;
            averageCadence = Math.Round(averageCadence, 2);
            label13.Text = averageCadence.ToString();



            return cadence;
        }

        private List<double> altitudeCalc(double[] doubleData)
        {

            double averageAltitude = 0;
            int count = 0;
            for (int i = 3; i < data.Length; i = i + 6)
            {

                // same loop for altitude
                altitude.Add(doubleData[i]);
                if (doubleData[i] != 0)
                {
                    averageAltitude = averageAltitude + doubleData[i];
                    count++;
                }

            }

            //work out max/average and display
            averageAltitude = averageAltitude / count;
            averageAltitude = Math.Round(averageAltitude, 2);
            label33.Text = averageAltitude.ToString();

            double maximumAltitude = altitude.Max();

            label35.Text = maximumAltitude.ToString();

            return altitude;

        }


        private void sortData(string smode)
        {

            double[] doubleData = new double[data.Length];

            // converts data into double array
            for (int x = 0; x < data.Length; x++)
            {
                // trys to parse the data in the data list and then outputs to doubledata array
                double.TryParse(data[x], out doubleData[x]);

            }

            if (smode == "000000000")
            {

                powerCalc(doubleData);



            }
            else if (smode == "111111100")
            {

                powerCalc(doubleData);
                heartRateCalc(doubleData);
                altitudeCalc(doubleData);
                cadenceCalc(doubleData);
                speedDistanceCalc(doubleData);
                Console.WriteLine(speed.Count.ToString());

            }
           else
            {





            }

            displayData();
            displayGraph(heartRate, cadence);
            
        }

        public void displayGraph(List<double> heartRate, List<double> cadence)
        {
            GraphPane myPane = zedGraphControl1.GraphPane;

            myPane.Title = "Plotted Data";
            myPane.XAxis.Title = "Time (Minutes)";
            myPane.YAxis.Title = "Scale";
            
            PointPairList heartRatePP = new PointPairList();
            PointPairList cadencePP = new PointPairList();
            double count = 1;
            for (int i = 0; i < cadence.Count; i = i+60 )
            {
                
                heartRatePP.Add(count, heartRate[i]);
                cadencePP.Add(count, cadence[i]);
                count++;
            }


            LineItem CurveHeartRate = myPane.AddCurve("Heart Rate", heartRatePP, Color.Red, SymbolType.Default);
            LineItem CurveCadence = myPane.AddCurve("Cadence", cadencePP, Color.Blue, SymbolType.Diamond);
            zedGraphControl1.AxisChange();

        }
        public void displayData()
        {

            








                // create data table and convert to array so length can be used to determine loop sizes later (possibly can be done with lists?)
                DataTable dt = new DataTable();
                double[] heartRateArray = heartRate.ToArray();
                // create colum headers
                dt.Columns.Add("Time");
                dt.Columns.Add("Heart Rate (BPM)", typeof(int));
                dt.Columns.Add("Speed (" + stringSpeedUnit + ")", typeof(int));
                dt.Columns.Add("Cadence (RPM)", typeof(int));
                dt.Columns.Add("Altitude (MASL)", typeof(int));
                dt.Columns.Add("Power (watts)", typeof(int));



                // converts startimte string into date format so seconds can be added
                DateTime dateTime = DateTime.ParseExact(startTime, "HH:mm:ss.f", null);
                
                for (int i = 0; i < heartRateArray.Length - 1; i++)
                {
                    // add all data from array/lists and add seconds to datetime
                    string result;
                    result = dateTime.AddSeconds(i).ToString("HH:mm:ss");

                    dt.Rows.Add(result,heartRate[i], speed[i], cadence[i], altitude[i], power[i]);

                }
                // customize datagridview to make it better looking. Also using datatable as souce to populate it.
                dataGridView1.DataSource = dt;
                dataGridView1.Columns[0].DefaultCellStyle.Font = new Font("Calibri", 12, FontStyle.Regular);
                dataGridView1.Columns[1].DefaultCellStyle.Font = new Font("Calibri", 12, FontStyle.Regular);
                dataGridView1.Columns[2].DefaultCellStyle.Font = new Font("Calibri", 12, FontStyle.Regular);
                dataGridView1.Columns[3].DefaultCellStyle.Font = new Font("Calibri", 12, FontStyle.Regular);
                dataGridView1.Columns[4].DefaultCellStyle.Font = new Font("Calibri", 12, FontStyle.Regular);
                dataGridView1.Columns[5].DefaultCellStyle.Font = new Font("Calibri", 12, FontStyle.Regular);
                dataGridView1.RowHeadersVisible = false;
                dataGridView1.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.RowsDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
                dataGridView1.AllowUserToResizeColumns = false;
                dataGridView1.AllowUserToAddRows = false;
                dataGridView1.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
                dataGridView1.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
                dataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
                dataGridView1.Columns[0].Width = 150;
                dataGridView1.Columns[1].Width = 150;
                dataGridView1.Columns[2].Width = 150;
                dataGridView1.Columns[3].Width = 150;
                dataGridView1.Columns[4].Width = 150;
                dataGridView1.Columns[5].Width = 150;
              
                dataGridView1.ColumnHeadersDefaultCellStyle.Font = new Font("Calibri", 15, FontStyle.Regular);
                dataGridView1.ColumnHeadersHeight = 75;
                Console.Write("getting here");
            
        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
        // if radio buttons checked state is changed
        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {

            if (radioButton1.Checked == true)
            {
                // data in KPH to begin with so data is multiplied by 1 and units set to KPH
                speedUnit = 1.60934;
                stringSpeedUnit = "KPH";
                for (int i = 0; i < speed.Count; i++)
                {

                    speed[i] = speed[i] * speedUnit;



                }

            }
            else
            {
                // MPH calculations
                stringSpeedUnit = "MPH";
                speedUnit = 0.621371;

                for (int i = 0; i < speed.Count; i++ )
                {

                    speed[i] = speed[i] * speedUnit;



                }

            }


            // clear everything and rerun methods to populate with alternative units


            displayData();
           

        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }

        private void btnFile_Click(object sender, EventArgs e)
        {
            OpenFileDialog t = new OpenFileDialog();

            t.Filter = "HRM|*.hrm";
            if (t.ShowDialog() == DialogResult.OK)
            {

                string path = t.FileName;
                getData(path);



            }


        }
      
    }
}
