using IronOcr;
using Npgsql;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Collections.Generic;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;

namespace AutoVisaConcept.Model
{
    class Person : DependencyObject
    {

        IWebDriver _driver;
        //SelenuimVisa 

        SQLiteConnection connection;

        public string FilePath
        {
            get { return (string)GetValue(FilePathpr); }
            set { SetValue(FilePathpr, value); }
        }
        public static readonly DependencyProperty FilePathpr =
            DependencyProperty.Register("FilePath", typeof(string), typeof(Person), new PropertyMetadata(""));

        public string Name
        {
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



        public Person(string FilePath1, SQLiteConnection connection1 , IWebDriver selenium_instance)
        {
            //FilePath = FilePath1;
            Application.Current.Dispatcher.Invoke(() => this.FilePath = FilePath1);
            connection = connection1;
            _driver = selenium_instance;
        }

       
        Regex regex1 = new Regex(@"[0-3]{1}\d\.\s?[0|1]{1}\d\.\s?[1|9]{2}[2-9]{1}\d");//all space

        public async Task OcrProceed()
            => await Task.Run(() =>
            {
                string tempdate = null;
                var FilePath = Application.Current.Dispatcher.Invoke(() => this.FilePath);
                var Results = Ocr.Read(FilePath);
                StringReader strReader = new StringReader(Results.Text);
                string aLine = null;
                while (true)
                {
                    aLine = strReader.ReadLine();
                    if (aLine != null)
                    {
                        if (aLine.Contains("<<<<") && aLine.Substring(0, 5).Contains("RUS"))
                        {
                            string[] temp1 = (aLine.Substring(5)).Split(new string[] { "<<" }, StringSplitOptions.None);
                            Application.Current.Dispatcher.Invoke(() => Surname = temp1[0]);
                            //Surname = temp1[0];
                            //Name = temp1[1].Split('<')[0];
                            Application.Current.Dispatcher.Invoke(() => Name = temp1[1].Split('<')[0]);
                            //Patronomic = temp1[1].Split('<')[1];
                            Application.Current.Dispatcher.Invoke(() => Patronomic = temp1[1].Split('<')[1]);
                        }
                        else if (aLine.Length < 100)
                        {
                            try
                            {
                                tempdate = regex1.Matches(aLine)[0].Value.Replace(" ", "");
                                Application.Current.Dispatcher.Invoke(() => Birthdate = tempdate);

                            }
                            catch
                            {

                            }

                        }
                    }
                    else { break; }
                }
            });

        public Task Add_todb()
            => Task.Run(() =>
            {

                using (var cmd = new SQLiteCommand("INSERT INTO Persons (surname,name,patronomic,birthdate,passport_id) VALUES (@a,@b,@c,@d,@e)", connection))
                {

                    cmd.Parameters.AddWithValue("a", Application.Current.Dispatcher.Invoke(() => Surname));
                    cmd.Parameters.AddWithValue("b", Application.Current.Dispatcher.Invoke(() => Name));
                    cmd.Parameters.AddWithValue("c", Application.Current.Dispatcher.Invoke(() => Patronomic));
                    try
                    {
                        cmd.Parameters.AddWithValue("d", DateTime.Parse(Application.Current.Dispatcher.Invoke(() => Birthdate)));
                    }
                    catch { cmd.Parameters.AddWithValue("d", null); }
                    cmd.Parameters.AddWithValue("e", Application.Current.Dispatcher.Invoke(() => Passport_id));
                    cmd.ExecuteNonQuery();
                }

            });

        public async Task Get_visa(int i)
            // => await Task.Run(() =>
             {
                 String a = "window.open('https://videx.diplo.de/videx/desktop/index.html#start','_blank');";
                 ((IJavaScriptExecutor)_driver).ExecuteScript(a);
                 
                 IList<string> tabs = new List<string>(_driver.WindowHandles);

                 _driver.SwitchTo().Window(tabs[i+1]);

                 var inputs = _driver.FindElements(By.TagName("input"));
                 foreach (IWebElement element in inputs)
                 {
                     if ((element.GetAttribute("id")).Contains("lastname"))
                     {
                         try
                         {
                             element.Click();
                             element.SendKeys(Application.Current.Dispatcher.Invoke(() => Surname));
                         }
                         catch { }
                     }

                     else if ((element.GetAttribute("id")).Contains("firstname"))
                     {
                         try
                         {
                             element.Click();
                             element.SendKeys(Application.Current.Dispatcher.Invoke(() => Name));
                         }
                         catch { }
                     }

                     else if ((element.GetAttribute("id")).Contains("date-of-birth"))
                     {
                         try
                         {
                             element.Click();
                             element.SendKeys(Application.Current.Dispatcher.Invoke(() => Birthdate));
                         }
                         catch { }
                     }

                 }
             }//);
    }
}