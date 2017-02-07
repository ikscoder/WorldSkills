using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Timers;
using System.IO.Ports;
using System.Net;
using System.IO;

namespace LocalServer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        System.Timers.Timer aTimer;
        SerialPort currentPort;
        private delegate void updateDelegate(string txt);
        public MainWindow()
        {
            InitializeComponent();
            Ports.DataContext = SerialPort.GetPortNames().ToList();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            bool ArduinoPortFound = false;

            try
            {
                string[] ports = SerialPort.GetPortNames();
                foreach (string port in ports)
                {
                    currentPort = new SerialPort(port, 9600);
                    if (ArduinoDetected())
                    {
                        ArduinoPortFound = true;
                        break;
                    }
                    else
                    {
                        ArduinoPortFound = false;
                    }
                }
            }
            catch { }

            if (ArduinoPortFound == false) return;
            System.Threading.Thread.Sleep(500); // немного подождем

            currentPort.BaudRate = 9600;
            currentPort.DtrEnable = true;
            currentPort.ReadTimeout = 1000;
            try
            {
                currentPort.Open();
            }
            catch { }

            aTimer = new System.Timers.Timer(1000);
            aTimer.Elapsed += OnTimedEvent;
            aTimer.AutoReset = true;
            aTimer.Enabled = true;
        }

        private bool ArduinoDetected()
        {
            try
            {
                currentPort.Open();
                System.Threading.Thread.Sleep(1000);
                // небольшая пауза, ведь SerialPort не терпит суеты

                string returnMessage = currentPort.ReadLine();
                currentPort.Close();

                // необходимо чтобы void loop() в скетче содержал код Serial.println("Info from Arduino");
                if (returnMessage.Contains("Info from Arduino"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception e)
            {
                return false;
            }
        }

        private void OnTimedEvent(object sender, ElapsedEventArgs e)
        {
            if (!currentPort.IsOpen) return;
            try // так как после закрытия окна таймер еще может выполнится или предел ожидания может быть превышен
            {
                // удалим накопившееся в буфере
                currentPort.DiscardInBuffer();
                // считаем последнее значение 
                string strFromPort = currentPort.ReadLine();
                //lblPortData.Dispatcher.BeginInvoke(new updateDelegate(updateTextBox), strFromPort);
            }
            catch { }
        }

        private void Update_Click(object sender, RoutedEventArgs e)
        {
            //var selected = (string)Ports.SelectedItem;
            Ports.DataContext = SerialPort.GetPortNames().ToList();
            TryReadFromPortAsync();
            //Ports.SelectedItem = selected;
        }

        private void Ports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryReadFromPortAsync();
        }

        private void Speeds_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            TryReadFromPortAsync();
        }

        /*private async void TryReadFromPortAsync()
        {
            await Task.Run(async () =>
            {
                try
                {
                    SerialPort currentPort=null;
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        if (Ports.SelectedItem != null && Speeds.SelectedItem != null)
                            try { currentPort = new SerialPort((string)Ports.SelectedItem, (int)Speeds.SelectedItem); } catch { }
                    }));
                    System.Threading.Thread.Sleep(500);
                    currentPort.BaudRate = 9600;
                    currentPort.DtrEnable = true;
                    currentPort.ReadTimeout = 1000;
                    currentPort.Open();
                    await Dispatcher.BeginInvoke(new Action(delegate ()
                    {
                        output.Text = currentPort.ReadExisting();
                    }));
                }
                catch { }
            });
        }*/
        volatile bool checker=true;
        private async void TryReadFromPortAsync()
        {
            checker = false;
            try
            {
                SerialPort currentPort = null;
                currentPort = new SerialPort((string)Ports.SelectedItem ?? SerialPort.GetPortNames()[0], (int?)Speeds.SelectedItem ?? 9600);
            }
            catch { }
            await Task.Run(async () =>
                {
                    System.Threading.Thread.Sleep(500);
                    checker = true;
                    currentPort.BaudRate = 9600;
                    currentPort.DtrEnable = true;
                    currentPort.ReadTimeout = 1500;
                    currentPort.Open();
                    /* while (currentPort.BytesToRead != 0)
                     {
                         output.Text += currentPort.ReadExisting();
                         currentPort.DiscardInBuffer();
                         System.Threading.Thread.Sleep(500);
                     }*/
                    while (checker)
                {
                    try
                    {
                            string line= currentPort.ReadLine();
                            await Dispatcher.BeginInvoke(new Action(delegate ()
                            {
                                output.Text += line;
                            }));
                            System.Threading.Thread.Sleep(50);
                        }
                    catch (TimeoutException) { }
                }
                    currentPort.Close();

        });


        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            string responsetext = new StreamReader(WebRequest.Create("УРЛ вашего сервиса").GetResponse().GetResponseStream()).ReadToEnd();

           /* WebClient wc = new WebClient();
            wc.DownloadStringCompleted +=
                delegate (object sender, DownloadStringCompletedEventArgs e)
                {
                    string responsetext = e.Result;
                };
            wc.DownloadStringAsync(new Uri("URL вашего сервиса"));

            WebClient wc = new WebClient();
            wc.UploadDataCompleted +=
                delegate (object sender, UploadDataCompletedEventArgs e)
                {
                    string responsetext = Encoding.UTF8.GetString(e.Result);
                };
            wc.UploadDataAsync(new Uri("URL вашего сервиса"), Encoding.UTF8.GetBytes("То что вы будете отправлять POSTом"));*/
        }
    }
}
