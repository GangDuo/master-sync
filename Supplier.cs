using CsvHelper.Configuration.Attributes;
using CsvHelper.TypeConversion;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Master.Sync
{
    class Supplier
    {
        [Index(0)]
        public string Code { get; set; }

        [Index(1)]
        public string Name { get; set; }

        [Index(8)]
        public string Postcode { get; set; }

        [Index(9)]
        public string Prefecture { get; set; }

        [Index(10)]
        public string Address1 { get; set; }

        [Index(12)]
        public string Tel { get; set; }

        [Index(13)]
        public string Fax { get; set; }
    }
}
