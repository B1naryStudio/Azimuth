using System;
using System.Text.RegularExpressions;
using Azimuth.DataAccess.Infrastructure;
using Ninject;

namespace Azimuth.DataGenerator
{
    class Program
    {
        static void Main()
        {
            IKernel kernel = new StandardKernel(new DataAccessModule());

            var dg = new DataGenerator(kernel);
            dg.ClearDatabase();
            dg.GenerateData();
            dg.AddSharing();
        }
    }
}
