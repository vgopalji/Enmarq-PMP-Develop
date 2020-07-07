using System;
using System.Collections.Generic;
using System.Text;

namespace CareStream.Models
{
    public class UserAttributeModel
    {
        public string Id { get; set; }
        public string AppDisplayName { get; set; }
        public List<string> TargetObjects { get; set; }

        public string Name { get; set; }

        public string DataType { get; set; }

        public List<Properties> Properties { get; set; }
        public Properties Property { get; set; }

    }

    public class Properties
    {
        public string Name { get; set; }

        public string DataType { get; set; }

    }

}
