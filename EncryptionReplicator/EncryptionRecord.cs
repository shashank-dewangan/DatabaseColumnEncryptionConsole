using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EncryptionReplicator
{
    public class EncryptionRecord
    {
        public int ID { get; set; }
        public string ColumnValue { get; set; }
        public string ColumnEncryptedValue { get; set; }
    }
}
