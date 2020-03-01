using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;

namespace AutoVisaConcept
{
    class SelenuimVisa
    {
        IWebDriver _driver;

        ChromeOptions options = new ChromeOptions();

        public SelenuimVisa()
        {
            //options.AddArgument("--headless");
            _driver = new ChromeDriver(Environment.CurrentDirectory, options);
        }

        public void get_visa()
        {
            _driver.Navigate().GoToUrl("https://videx.diplo.de/videx/desktop/index.html#start");
        }
    }
}
