using AutoVisaConcept.Model;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using IronOcr;
using System.Threading.Tasks;
using System.IO;
using System.Text.RegularExpressions;
using System.Windows.Data;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Npgsql;
using System.Data.SQLite;
using System.Windows;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace AutoVisaConcept.VM
{
    class Repository: INotifyPropertyChanged
    {
        private Relay_Command _download;

        private Relay_Command _proceed_data;

        public ObservableCollection<Person> Persons { get; set; }

        public Person temp;

        SQLiteConnection connection;

        private string dbFileName="Persons.sqlite";

        IWebDriver _driver;

        ChromeOptions options = new ChromeOptions();


        private readonly object _urlsLock = new object();

        private int _barvalue = 0;//Текущее значение для progressbar
        

        public int Barvalue
        {
            get { return _barvalue; }
            set
            {
                _barvalue = value;
                OnPropertyChanged("Barvalue");
            }
        }

        public Repository()
        {
            
            Persons = new ObservableCollection<Person>();
            BindingOperations.EnableCollectionSynchronization(Persons, _urlsLock);
            _driver = new ChromeDriver(Environment.CurrentDirectory, options);
            ////////////////////////////////////////////////////////////////////////////
            db_handle();
        }

        ~Repository()
        {
            _driver.Quit();
        }

        void db_handle()
        {
            if (!File.Exists(dbFileName ))
                SQLiteConnection.CreateFile(dbFileName);

            try
            {
                connection = new SQLiteConnection("Data Source=" + dbFileName + ";Version=3;");
                connection.Open();
                SQLiteCommand m_sqlCmd=new SQLiteCommand();
                m_sqlCmd.Connection = connection;

                m_sqlCmd.CommandText = "CREATE TABLE IF NOT EXISTS Persons (id INTEGER PRIMARY KEY AUTOINCREMENT, name TEXT," +
                    " surname TEXT,patronomic TEXT,birthdate DATETIME,passport_id INTEGER)";

                m_sqlCmd.ExecuteNonQuery();

            }
            catch (SQLiteException ex)
            {
                //lbStatusText.Text = "Disconnected";
                //MessageBox.Show("Error: " + ex.Message);
            }
        }

        public async Task OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
               
                foreach (var strr in openFileDialog.FileNames)
                {
                    
                    temp = new Person(strr,connection,_driver);
                    Persons.Add(temp);
                    
                }
                //Для столь процессорозависимой задачи количество потоков равно количеству потоков процессора
                Parallel.ForEach(Persons,async (Person current)=>  { await current.OcrProceed(); });
                MessageBox.Show("Проверьте корректность данных" +
                    " после завершения и измените их в таблице при необходимости");
                
            }
        }

        public async Task Proceed()
        {
            Persons.Select(e => e.Add_todb()).ToArray();
            //Parallel.ForEach(Persons, async (Person current) => { await current.Get_visa(Persons.IndexOf(current));
            //}); 
            //chrome driver не может асинхронно работать с каждым окном браузера
            //В связи с чем не для тестового варианта необходимо масштабировать драйверы вместе с браузерами

            Persons.Select(e => e.Get_visa(Persons.IndexOf(e))).ToArray();

        }





        public Relay_Command Download
        {
            get
            {
                return _download ??
                  (_download = new Relay_Command(obj =>  OpenFileDialog()));
            }
        }

        public Relay_Command Proceed_data
        {
            get
            {
                return _proceed_data ??
                  (_proceed_data = new Relay_Command(obj => Proceed()));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
