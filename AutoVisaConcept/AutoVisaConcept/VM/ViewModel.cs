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

namespace AutoVisaConcept.VM
{
    class ViewModel
    {
        private Relay_Command _download;

        public ObservableCollection<Person> Persons { get; set; }

        public Person temp;

        private readonly object _urlsLock = new object();

        public ViewModel()
        {
            //
            Persons = new ObservableCollection<Person>();
            BindingOperations.EnableCollectionSynchronization(Persons, _urlsLock);
        }

        public void OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            if (openFileDialog.ShowDialog() == true)
            {
                //await Task.Run(OcrProceed());
                foreach (var strr in openFileDialog.FileNames)
                {
                    temp = new Person(strr);
                    temp.OcrProceed();
                    //Persons.Add(temp);
                    //OcrProceed(File);
                }
            }
        }

        


        public Relay_Command Download
        {
            get
            {
                return _download ??
                  (_download = new Relay_Command(obj =>  OpenFileDialog()));
            }
        }
    }
}
