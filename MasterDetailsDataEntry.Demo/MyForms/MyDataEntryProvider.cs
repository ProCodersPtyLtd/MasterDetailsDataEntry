using MasterDetailsDataEntry.Demo.Data;
using MasterDetailsDataEntry.Demo.Database.Model;
using MasterDetailsDataEntry.Shared;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace MasterDetailsDataEntry.Demo
{
    public class MyDataEntryProvider : IDataEntryProvider
    {
        // read fields
        public IEnumerable<DataField> GetFormFields(IModelDefinitionForm form)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DataField> GetFormFields(Type formType)
        {
            throw new NotImplementedException();
        }

        public Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetFormFields(IMultiModelDefinitionForm form)
        {
            var fieldsSet = form.GetFields();
            return fieldsSet;

            var masterType = form.GetType().GetGenericArguments()[0];
            var detailsType = form.GetType().GetGenericArguments()[1];
            var masterFields = GenerateFormFields(masterType);
            var detailsFields = GenerateFormFields(detailsType);
            


            throw new NotImplementedException();
        }

        private IEnumerable<DataField> GenerateFormFields(Type type)
        {
            var result = new List<DataField>();
            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (var prop in properties)
            {
                var field = new DataField { BindingProperty = prop.Name,  };
            }

            return null;
        }

        public Tuple<IEnumerable<DataField>, IEnumerable<DataField>> GetMultiFormFields(Type formType)
        {
            throw new NotImplementedException();
        }

        // read data
        public IList GetModelData(IModelDefinitionForm form)
        {
            return new Order[] { new Order { ClientName = "New Client", CreateDate = DateTime.Now, Id = 1 } };
        }

        public object GetModelData(IModelDefinitionForm form, int Id)
        {
            throw new NotImplementedException();
        }

        public Tuple<object, IList> GetModelData(IMultiModelDefinitionForm form, int id)
        {
            using (var db = GetDbContext(form))
            {
                var masterType = form.GetMasterType();
                var detailsType = form.GetDetailsType();

                var master = db.Find(masterType, id);
                var navigation = db.Entry(master).Navigations.FirstOrDefault(n => n.Metadata.Name == detailsType.Name);
                navigation.Load();
                var details = (navigation.CurrentValue as IEnumerable).Cast<object>().ToList();
                return new Tuple<object, IList>(master, details);
            }
                
            //var d1 = new Order { ClientName = "New Client", CreateDate = DateTime.Now, Id = 1 };
            //var d2 = new OrderItem[] { new OrderItem { OrderId = 1, Id = 1, ItemName = "Lola" }, new OrderItem { OrderId = 1, Id = 2, ItemName = "Pola" } };
            //return new Tuple<object, IList>(d1, d2);
        }

        private DbContext GetDbContext(IMultiModelDefinitionForm form)
        {
            var type = form.GetDbContextType();
            var context = Activator.CreateInstance(type) as DbContext;
            return context;
        }
    }
}
