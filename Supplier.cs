using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Sync
{
    class Supplier : IEquatable<Supplier>
    {
        [Index(0)]
        public string Code { get; set; }

        [Index(1)]
        public string Name { get; set; }

        [Index(8)]
        public string Postcode { get; set; }
        public object PostcodeAsNumberOrDbNull
        {
            get
            {
                uint postcode;
                if(uint.TryParse(Postcode.Replace("-", ""), out postcode))
                {
                    return postcode;
                }
                else
                {
                    return DBNull.Value;
                }
            }
        }

        [Index(9)]
        public string Prefecture { get; set; }

        [Index(10)]
        public string Address1 { get; set; }

        [Index(12)]
        public string Tel { get; set; }

        [Index(13)]
        public string Fax { get; set; }

        public bool Equals(Supplier other)
        {
            if (other is null)
                return false;

            return this.Name == other.Name &&
                this.Code == other.Code;// &&
#if false
            this.Postcode == other.Postcode &&
            this.Prefecture == other.Prefecture &&
            this.Address1 == other.Address1 &&
            this.Tel == other.Tel &&
            this.Fax == other.Fax;
#endif
        }
        public override bool Equals(object obj) => Equals(obj as Supplier);
        public override int GetHashCode() => (Name, Code).GetHashCode();
    }
}
