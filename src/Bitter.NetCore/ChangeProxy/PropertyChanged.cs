using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Text;

namespace Bitter.Core
{
    public class MyPropertyChanged
    {
        public List<ChangedInfo> changedList = new List<ChangedInfo>();

        //public void CheckedChanged(string oldValue, string newValue, string filedName, string orgType)
        //{
        //    changedList.Add(new ChangedInfo { filedName = filedName, newValue = newValue, oldValue = oldValue, orgType = orgType });
        //}
        public void CheckedChanged(string oldValue)
        {
            changedList.Add(new ChangedInfo { FFiledName =oldValue});
        }

      
    }
    public class ChangedInfo
    {
        public string FFiledName { get; set; }
        public string FOldValue { get; set; }
        public string FNewValue { get; set; }
        public string FOrgType { get; set; }

        public string FFiledDes { get; set; }

        public string FTableName { get; set; }

        public  string  FKeyFiledName  { get; set; }

        public string FKeyValue { get; set; }



    }

}
