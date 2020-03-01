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

namespace AutoVisaConcept.VM
{
    class ViewModel: INotifyPropertyChanged
    {
        private Relay_Command _download;

        private Relay_Command _proceed_data;

        public ObservableCollection<Person> Persons { get; set; }

        public Person temp;

        BDcontext connection;

        SelenuimVisa SelenuimActions=new SelenuimVisa();

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

        public ViewModel()
        {
            //
            Persons = new ObservableCollection<Person>();
            BindingOperations.EnableCollectionSynchronization(Persons, _urlsLock);
            connection = new BDcontext();
        }

        public async Task OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
               // Parallel.ForEach(openFileDialog.FileNames,(string filepath)=>  { temp = new Person(filepath);
                    //temp.OcrProceed1();
                    //Persons.Add(temp);
                //});
                //await Task.Run(OcrProceed());
                
                foreach (var strr in openFileDialog.FileNames)
                {
                    //temp = new Person(strr);
                    //temp.OcrProceed();
                    // Persons.Add(temp);
                    //OcrProceed(File);
                    temp = new Person(strr,connection,SelenuimActions);
                    Persons.Add(temp);
                    
                }
                //await Task.WhenAll(Persons.Select(e => e.OcrProceed().ContinueWith(t => Barvalue++)).ToArray());
                //Persons.Select(e => e.OcrProceed().ContinueWith(t => Barvalue++)).ToArray();
                Parallel.ForEach(Persons,async (Person current)=>  { await current.OcrProceed(); });


            }
        }

        public async Task Proceed()
        {
            Persons.Select(e => e.Add_todb()).ToArray();
            Parallel.ForEach(Persons, async (Person current) => { await current.Get_visa(); });
            
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
