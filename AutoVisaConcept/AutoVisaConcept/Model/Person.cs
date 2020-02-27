using IronOcr;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AutoVisaConcept.Model
{
    class Person:DependencyObject
    {

        public string FilePath
        {
            get { return (string)GetValue(FilePathpr); }
            set { SetValue(FilePathpr, value); }
        }
        public static readonly DependencyProperty FilePathpr =
            DependencyProperty.Register("FilePath", typeof(string), typeof(Person), new PropertyMetadata(""));

        public string Name {
            get { return (string)GetValue(Namepr); }
            set { SetValue(Namepr, value); }
        }

        public static readonly DependencyProperty Namepr =
            DependencyProperty.Register("Name", typeof(string), typeof(Person), new PropertyMetadata(""));

        public string Surname
        {
            get { return (string)GetValue(Surnamepr); }
            set { SetValue(Surnamepr, value); }
        }

        public static readonly DependencyProperty Surnamepr =
            DependencyProperty.Register("Surname", typeof(string), typeof(Person), new PropertyMetadata(""));
        public string Patronomic
        {
            get { return (string)GetValue(Patronomicpr); }
            set { SetValue(Patronomicpr, value); }
        }

        public static readonly DependencyProperty Patronomicpr =
            DependencyProperty.Register("Patronomic", typeof(string), typeof(Person), new PropertyMetadata(""));


        public string Birthdate
        {
            get { return (string)GetValue(Birthdatepr); }
            set { SetValue(Birthdatepr, value); }
        }

        public static readonly DependencyProperty Birthdatepr =
            DependencyProperty.Register("Birthdate", typeof(string), typeof(Person), new PropertyMetadata(""));


        public int? Passport_id
        {
            get { return (int?)GetValue(Passport_idpr); }
            set { SetValue(Passport_idpr, value); }
        }

        public static readonly DependencyProperty Passport_idpr =
            DependencyProperty.Register("Passport_id", typeof(int?), typeof(Person), new PropertyMetadata(0));


        AutoOcr Ocr = new AutoOcr();

        
        public Person(string FilePath1)
        {
            FilePath = FilePath1;
            
        }
        
        public async Task OcrProceed()
        {
            var Results = Ocr.Read(FilePath);
            StringReader strReader = new StringReader(Results.Text);
            string aLine = null;
            while (true)
            {
                aLine = strReader.ReadLine();
                if (aLine != null)
                {
                    if (aLine.Contains("<<<<"))
                    {
                        string[] temp1 = (aLine.Substring(5)).Split(new string[] { "<<" }, StringSplitOptions.None);
                        Surname = temp1[0];
                        Name = temp1[1].Split('<')[0];
                        Patronomic = temp1[1].Split('<')[1];
                    }
                    else if (aLine.Length<25)//((aLine.Contains("МУЖ") || aLine.Contains("ЖЕН")))//&&(Regex.Matches(aLine, "\\d\\d.\\d\\d.\\d\\d\\d\\d").Count!=0))
                    {
                        try
                        {
                            Birthdate =Regex.Matches(aLine, "\\d\\d.\\d\\d.\\d\\d\\d\\d")[0].Value;
                        }
                        catch {
                            try
                            {
                                Birthdate = Regex.Matches(aLine, "\\d\\d.\\s\\d\\d.\\d\\d\\d\\d")[0].Value.Trim();
                            }
                            catch { }
                        }
                        //Console.WriteLine(res);
                    }
                }
                else { break; }
            }
        }
    }
}
