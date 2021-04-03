﻿// *******************************************************************************************************
// This code is auto generated by Platz.ObjectBuilder template, any changes made to this code will be lost
// *******************************************************************************************************
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Platz.ObjectBuilder;
using Platz.SqlForms;
using Default;

namespace Default
{
    #region Data Context 

    public partial class CrmDataContext : DataContextBase
    {
        protected override void Configure(DataContextSettings settings)
        {
            settings.SetSchema("Crm2");
            settings.SetDriver<SqlJsonStoreDatabaseDriver>();
            settings.MigrationsPath = @"\StoreNew\Crm2.schema.migrations.json";

            settings.AddTable<Address>();
            settings.AddTable<Person>();
            settings.AddTable<PersonAddress>();
        }
    }

    #endregion

    #region Entities

    public partial class Address
    { 
        public virtual int Id { get; set; }
        public virtual string Line1 { get; set; }
        public virtual string Suburb { get; set; }
        public virtual string State { get; set; }
        public virtual string PostCode { get; set; }
        public virtual bool Deleted { get; set; }
        public virtual string Country { get; set; }
    }

    public partial class Person
    { 
        public virtual int Id { get; set; }
        public virtual string Name { get; set; }
        public virtual string Surname { get; set; }
        public virtual string Phone { get; set; }
        public virtual DateTime? Dob { get; set; }
    }

    public partial class PersonAddress
    { 
        public virtual int Id { get; set; }
        public virtual int? PersonId { get; set; }
        public virtual int? AddressId { get; set; }
        public virtual bool Deleted { get; set; }
    }

    #endregion
}    

