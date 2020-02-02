using System;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;

namespace Veeam_task
{
    class Program
    {
        static void Main(string[] args)
        {
            string strCountry = "Romania";
            string strLanguage = "English";
            int iCountJobs = 31;

            Boolean bLanguage = false;

            if (args.Length == 3)
            {
                strCountry = args[0];
                strLanguage = args[1];
                try
                {
                    iCountJobs = Convert.ToInt32(args[2]);
                }
                catch (FormatException)
                {
                    Console.WriteLine("Input string for number of expected jobs is not a sequence of digits.");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("The number of expected jobs cannot fit in an Int32.");
                }
            }
            
            IWebDriver driver = new ChromeDriver();
            driver.Navigate().GoToUrl("https://careers.veeam.com");
            driver.Manage().Window.Maximize();
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(3);


            driver.FindElement(By.XPath("//a[@class='cookie-messaging__button js-btn-close']")).Click();
            System.Threading.Thread.Sleep(500);
            driver.FindElement(By.Id("country-element")).Click();
            System.Threading.Thread.Sleep(500);
            try {
                var element = driver.FindElement(By.XPath("//span[@data-value='" + strCountry + "']"));
                Actions actions = new Actions(driver);
                actions.MoveToElement(element);
                actions.Perform();
                driver.FindElement(By.XPath("//span[@data-value='" + strCountry + "']")).Click();
            }
            catch (NoSuchElementException)
            {
                Console.WriteLine(strCountry + " - not found!");
            }
            catch (Exception e)
            {
                Console.WriteLine("Error: " + e.Message);
            }
            System.Threading.Thread.Sleep(500);
            driver.FindElement(By.Id("language")).Click();
            System.Threading.Thread.Sleep(500);


            IList<IWebElement> listOfElements = driver.FindElements(By.XPath("//label[@class='controls-checkbox']"));
            foreach (IWebElement language in listOfElements)
            {
                Console.WriteLine(language.Text);
                if (language.Text.Equals(strLanguage))
                {
                    language.Click();
                    bLanguage = true;
                    break;
                }
            }
            driver.FindElement(By.Id("language")).Click();
            
            if (!bLanguage)
            {
                Console.WriteLine(strLanguage + " - not found!");
                Console.WriteLine("End of test.");
                driver.Close();
                return;
            }
            System.Threading.Thread.Sleep(500);
            driver.FindElement(By.XPath("//a[@class='content-loader-button load-more-button']")).Click();
            System.Threading.Thread.Sleep(5000);
            listOfElements = driver.FindElements(By.XPath("//a[@class='vacancies-blocks-item-header ability-link']"));
            Console.WriteLine(listOfElements.Count);
            if (listOfElements.Count != iCountJobs) Console.WriteLine("Number of jobs not corresponding to expected value!");  
            else Console.WriteLine("Number of jobs correct.");
            
            driver.Close();


        }
    }
}
