using Skyiv;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;

// Mao Weiqing, Perth, Australia
// Lucas-Lehmer Test for Mersenne Prime Number 2^n-1  =  Mn
// So far SSFFT performance is not tuned, due to the massive result of FFT
namespace isMersennePrime
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        //Num of Thread:4
        bool isStartA = false;
        bool isPauseA = false;
        bool stopBtnPushedA = false;
        string fileName = "PrimeNumbers.txt";
        string saveName = "LastNumbers.txt";
        int addNumber = 4;//must larger than 1
        int lastNumber;
        List<int> listA = null;
        Stopwatch sw1;
        System.Timers.Timer t1;
        TimeSpan tms1 = new TimeSpan(0, 0, 0);

        static BigInteger ZERO = new BigInteger(0);
        static BigInteger ONE = new BigInteger(1);
        static BigInteger TWO = new BigInteger(2);
        static BigInteger FOUR = new BigInteger(4);

        public MainWindow()
        {
            InitializeComponent();
            SaveBtnA.IsEnabled = false;
            StopBtnA.IsEnabled = false;
            PauseBtnA.IsEnabled = false;
            t1 = new System.Timers.Timer(1000);
            t1.Enabled = true;
            t1.Elapsed += new ElapsedEventHandler(OnTimedEventA);
            sw1 = new Stopwatch();
            ReadFile();
            startValueLabel.Content = lastNumber + 1;
            addValueLabel.Text = addNumber.ToString();
            autoSaveChk.IsChecked = true;
            currentValueLabel.Content = lastNumber + 1;
            lastValueLabel.Content = lastNumber;
    
            //test
            try
            {
                Stopwatch ssw1 = new Stopwatch();
                ssw1.Start();
                //String inputData = "1234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678123456781234567812345678";
                String inputData = "012345678123456789";
                BigInteger input1 = Utility.BigIntegerFromString(inputData);
                BigInteger resultToomCook = Toom_Cook.ToomCook_2_Multiply(input1,input1);
                //BigInteger resultToomCook = ToomCook_3_Multiply(input1, input1);
                ssw1.Stop();
                TimeSpan tts1 = ssw1.Elapsed;
                String oout1 = tts1.TotalMilliseconds.ToString();
                //Console.WriteLine("Toom Cook 3 Time: " + oout1);
                Console.WriteLine("Toom Cook 2 Time: " + oout1);

                Stopwatch ssw2 = new Stopwatch();
                ssw2.Start();
                BigInteger resultGradeSchool = BigInteger.Multiply(input1, input1);
                ssw2.Stop();
                TimeSpan tts2 = ssw2.Elapsed;
                String oout2 = tts2.TotalMilliseconds.ToString();
                Console.WriteLine("Grade-school Multiply Result: " + resultGradeSchool);
                Console.WriteLine("Grade-school Multiply Time: " + oout2);

                Stopwatch wat1 = new Stopwatch();
                wat1.Start();

                AForge.Math.Complex[] ddata = Schonhage_Strassen_FFT.data_builder(input1);
                AForge.Math.Complex[] fdata = Schonhage_Strassen_FFT.data_builder(input1);
                BigInteger resultSSFFT = Schonhage_Strassen_FFT.Schonhage_Strassen_Fast_Fourier_Transform(ddata, fdata);

                wat1.Stop();
                TimeSpan wts1 = wat1.Elapsed;
                String wout1 = wts1.TotalSeconds.ToString();
                Console.WriteLine("Schonhage_Strassen_FFT Time: " + wout1);

                Stopwatch wat2 = new Stopwatch();
                wat2.Start();
                BigInteger resultBMFFT = BigIntegerMultiplicationFFT.BigIntegerMultiplication_Fast_Fourier_Transform(BigIntegerMultiplicationFFT.Reverse(inputData));
                wat2.Stop();
                TimeSpan wts2 = wat2.Elapsed;
                String wout2 = wts2.TotalSeconds.ToString();
                Console.WriteLine("BigIntegerMultiplicationFFT Time: " + wout2);
                Console.WriteLine("Lenght(): " + resultBMFFT.ToString().Length);
                Console.WriteLine("Is Answer Correct? " + ((resultBMFFT == resultSSFFT)==(resultToomCook==resultGradeSchool)));
            }
            catch (Exception r)
            {
                System.Windows.MessageBox.Show(r.Message);
            }
        }

        
        private void OnTimedEventA(object source, ElapsedEventArgs e)
        {
            TimeSpan ts = sw1.Elapsed;
            string elapsedTime = String.Format("{0:00}:{1:00}:{2:00}",
            ts.Hours, ts.Minutes, ts.Seconds);
            this.Dispatcher.BeginInvoke(new Action(() => TimeLabel.Content = elapsedTime));
        }

        private void Button_StartA(object sender, RoutedEventArgs e)
        {
            if (stopBtnPushedA)
            {
                tabcontent1.Text = "";
                TimeLabel.Content = "";
                ResultLabel.Content = "";
            }
            StopBtnA.IsEnabled = true;
            PauseBtnA.IsEnabled = true;
            sw1.Start();
            isStartA = true;
            isPauseA = false;

            if (isStartA && !isPauseA)
            {
                try
                {
                    autoSaveChk.IsEnabled = false;
                    addNumber = Convert.ToInt32(addValueLabel.Text);
                    StartBtnA.IsEnabled = false;
                    ResultLabel.Content = "Test +" + addNumber;
                    for (int i = 0; i < addNumber * 0.25; i++)
                    {
                        this.GetMersennePrimeNumbers(lastNumber + 1, lastNumber + addNumber, useFFT.IsChecked == true);
                        //MersennePrimeNumber(lastNumber + 2);
                        //Console.WriteLine("Finished");
                    }
                    sw1.Stop();
                    ResultLabel.Content = "Done";
                    autoSaveChk.IsEnabled = true;
                    SaveBtnA.IsEnabled = true;
                    StartBtnA.IsEnabled = true;
                }
                catch (Exception err)
                {
                    Console.WriteLine(err.Message);
                }
            }
        }

        //Lucas-Lehmer Test for Mersenne Prime Number
        private bool isMersennePrime(int p, bool isSSFFT)
        {
            //this.Dispatcher.BeginInvoke(new Action(() => currentValueLabel.Content = p));//reduce the performance, consider improvement
            if (p % 2 == 0) return (p == 2);
            else
            {
                for (int i = 3; i <= (int)Math.Sqrt(p); i += 2)
                    if (p % i == 0) return false; //not prime
                BigInteger m_p = BigInteger.Pow(TWO, p) - ONE;
                BigInteger s = FOUR;
                for (int i = 3; i <= p; i++)
                {
                    //Console.WriteLine("Current:["+i+"]/["+p+"]");
                    if (!isSSFFT)
                    {
                        s = (s * s - TWO) % m_p;
                    }
                    else
                    {
                        if (s < 1416317956)
                        {
                            s = (s * s - TWO) % m_p;
                        }
                        else
                        {
                            try
                            {
                                //s = (Schonhage_Strassen_FFT(data_builder(s), data_builder(s)) - TWO) % m_p;
                                s = (BigIntegerMultiplicationFFT.BigIntegerMultiplication_Fast_Fourier_Transform(BigIntegerMultiplicationFFT.Reverse(s.ToString())) * BigIntegerMultiplicationFFT.BigIntegerMultiplication_Fast_Fourier_Transform(BigIntegerMultiplicationFFT.Reverse(s.ToString())) - TWO) % m_p;
                            }
                            catch (Exception err)
                            {
                                System.Windows.MessageBox.Show(err.Message);
                            }
                        }
                    }
                }
                return s == ZERO;
            }
        }

        private void outMethod(out bool sign)
        {
            sign = autoSaveChk.IsChecked==true;
        }

        private List<int> GetMersennePrimeNumbers(int from, int upTo, bool isFFT)
        {
            bool chk = false;
            outMethod(out chk);//give autosaveChk ischecked value to chk

            List<int> response = new List<int>();
            Parallel.For(from, upTo, i =>
            {
                //System.Windows.Forms.Application.DoEvents();
                bool useFFT = isFFT;
                if (isMersennePrime(i, useFFT))
                {
                    response.Add(i);
                }
            });
            this.Dispatcher.BeginInvoke(new Action(() => currentValueLabel.Content = upTo));
            int len = response.Count;
            if (len > 0)
            {
                this.Dispatcher.BeginInvoke(new Action(() => tabcontent1.AppendText("M" + response[len - 1] + " ")));
                if (chk)
                {
                    for (int j = 0; j < len; j++)
                    {
                        SaveOneNumber(response[j], fileName, true);
                    }
                }
            }
            this.Dispatcher.BeginInvoke(new Action(() => tabcontent1.ScrollToEnd()));
            SaveOneNumber(upTo, saveName, false);
            lastNumber = upTo;
            lastValueLabel.Content = lastNumber;
            currentValueLabel.Content = lastNumber + 1;
            startValueLabel.Content = lastNumber + 1;
            response.Sort();
            return response;
        }

        private void MersennePrimeNumber(int i, bool isFFT)
        {
            bool chk = false;
            outMethod(out chk);//give autosaveChk ischecked value to chk

            System.Windows.Forms.Application.DoEvents();
            if (isMersennePrime(i, isFFT))
            {
                this.Dispatcher.BeginInvoke(new Action(() => tabcontent1.AppendText("M" + i + " ")));
                this.Dispatcher.BeginInvoke(new Action(() => tabcontent1.ScrollToEnd()));
                listA.Add(i);
                if (chk)
                {
                    SaveOneNumber(i, fileName, true);
                }
            }
        }

        private void Button_PauseA(object sender, RoutedEventArgs e)
        {
            sw1.Stop();
            tms1 += sw1.Elapsed;
            isPauseA = true;
        }

        private void SaveBtnA_Click(object sender, RoutedEventArgs e)
        {
            SaveFile();
            SaveBtnA.IsEnabled = false;
        }

        private void SaveOneNumber(int n, string fileName, bool append)
        {
            try
            {
                using (StreamWriter file = new StreamWriter(fileName, append, Encoding.Unicode))
                {
                    file.Write(n.ToString());
                    file.WriteLine(",");
                }
            }
            catch (Exception err)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(err.Message);
            }
        }

        private void SaveFile()
        {
            try
            {
                using (StreamWriter file = new StreamWriter(fileName, true, Encoding.Unicode))
                {
                    if (listA != null && listA.Count > 0)
                    {
                        for (int i = 0; i < listA.Count; i++)
                        {
                            file.Write(listA[i].ToString());
                            file.WriteLine(",");
                        }
                    }
                }
            }
            catch (Exception err)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be written:");
                Console.WriteLine(err.Message);
            }
        }

        private void ReadFile()
        {
            try
            {
                using (StreamReader sr = new StreamReader(fileName, Encoding.Unicode))
                {
                    string line, allLine = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        allLine += line;
                    }
                    totalNumLabel.Text = allLine;
                }
                using (StreamReader sr2 = new StreamReader(saveName, Encoding.Unicode))
                {
                    string line, tmp = null;
                    while ((line = sr2.ReadLine()) != null)
                    {
                        tmp = line;
                    }
                    int len = tmp.Length;
                    String last = tmp.Substring(0, len - 1);
                    lastNumber = Convert.ToInt32(last);
                }
            }
            catch (Exception e)
            {
                // Let the user know what went wrong.
                Console.WriteLine("The file could not be read:");
                Console.WriteLine(e.Message);
            }
        }

        private void Button_StopA(object sender, RoutedEventArgs e)
        {
            SaveBtnA.IsEnabled = true;
            isStartA = false;
            stopBtnPushedA = true;
            sw1.Stop();
        }

        private void winClosed(object sender, EventArgs e)
        {
            sw1.Stop();
            t1.Stop();
            System.Windows.Forms.Application.Exit();
        }

        private void addValueLabel_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            string str = e.Key.ToString();
            e.Handled = !(str == "D0" || str == "D1" || str == "D2" || str == "D3" || str == "D4" || str == "D5" || str == "D6" || str == "D7" || str == "D8" || str == "D9");
        }
    }
}
