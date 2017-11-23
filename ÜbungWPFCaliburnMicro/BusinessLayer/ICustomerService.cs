using System.Collections.Generic;
using ÜbungWPFCaliburnMicro.Model;

namespace ÜbungWPFCaliburnMicro.BusinessLayer
{
    public interface ICustomerService
    {
        IEnumerable<Customer> Load();
        void Save(Customer customer);
    }
}