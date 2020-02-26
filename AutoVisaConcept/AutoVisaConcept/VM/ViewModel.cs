using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoVisaConcept.VM
{
    class ViewModel
    {
        private Relay_Command _download;


        public List<string> FilePaths;

        public bool OpenFileDialog()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                FilePaths = openFileDialog.FileNames.ToList();
                return true;
            }
            return false;
        }

        public Relay_Command Download
        {
            get
            {
                return _download ??
                  (_download = new Relay_Command(obj => OpenFileDialog()));
            }
        }
    }
}
