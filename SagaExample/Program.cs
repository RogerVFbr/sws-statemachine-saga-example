using System;
using System.Text.Json.Serialization;
using Newtonsoft.Json;
using SagaExample.Controllers;
using SagaExample.Services;
using SagaExample.Services.Interfaces;
using Formatting = System.Xml.Formatting;

namespace SagaExample
{
    class Program
    {
        static void Main(string[] args)
        {
            ConfigurationModule.GetService<ICheckoutController>().ExecuteCheckout();
        }
    }
}