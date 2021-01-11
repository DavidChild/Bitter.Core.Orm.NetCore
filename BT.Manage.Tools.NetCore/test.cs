namespace BT.Manage.Tools
{
    /********************************************************************************
    ** auth： Jason
    ** date： 2017/3/29 13:15:46
    ** desc：
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 备胎 版权所有。
    *********************************************************************************/

    public class test
    {
        public void testx()
        {
            string bbb = "";
            bbb = null;
            //  var bb = bbb.Trim();
            //List<int> lstId=new List<int>();
            //lstId.Add(1);
            //lstId.Add(2);
            dynamic data = new
            {
                phone = "15654684856",
                name = "333",
                idCard = "233333",
                approvalOrderIDs = bbb.Trim()
            };
            astedx fdd = new astedx();
            axtes t = fdd.MapTo<axtes>();

        }

        public class axtes
        {
            public string a { get; set; }
        }

        public class astedx
        {
            public string b { get; set; }
        }
    }
}