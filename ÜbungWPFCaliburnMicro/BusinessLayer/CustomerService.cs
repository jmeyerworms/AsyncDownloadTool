using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ÜbungWPFCaliburnMicro.Model;

namespace ÜbungWPFCaliburnMicro.BusinessLayer
{
    public class CustomerService : ICustomerService
    {
        public IEnumerable<Customer> Load()
        {
            yield return new Customer {FirstName = "Müller", LastName = "Mustermann", Age = 13};
            yield return new Customer {FirstName = "Satter", LastName = "jens", Age = 25};
        }

        public void Save(Customer customer)
        {
            Debug.WriteLine($"{customer.FirstName} - {customer.LastName}");
            // Send to server/Save to Database
        }
    }
}
