using System;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Xunit;

namespace Viewer_ASP.NET_Core.Controllers
{
    public class AutomatedUILogin : IDisposable
    {
        private readonly IWebDriver _driver;
        public AutomatedUILogin()
        {
            _driver = new ChromeDriver();
        }

        [Fact]
        public void SahibindenLogin()
        {
            _driver.Navigate()
                .GoToUrl("https://secure.sahibinden.com/giris");

            Assert.Equal("sahibinden.com üye girişi", _driver.Title);
            Assert.Contains("Giriş Yap ve Devam Et", _driver.PageSource);

            _driver.FindElement(By.Id("username")).SendKeys("egemen999@hotmail.com");

            _driver.FindElement(By.Id("password")).SendKeys("1qAZ2wSX");

            _driver.FindElement(By.Id("userLoginSubmitButton")).Click();
        }

        public void Dispose()
        {
            _driver.Quit();
            _driver.Dispose();
        }
    }
}

