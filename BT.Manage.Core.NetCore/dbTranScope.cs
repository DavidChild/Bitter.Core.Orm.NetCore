using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BT.Manage.Tools;
using BT.Manage.Tools.Helper;
using NLog.Internal;

namespace BT.Manage.Core
{
    public class dbcontent
    {
       

        public dbcontent()
        {
            scope = new ModelOperationScope(ModelOpretionScopeOption.RequiresNew);//事务开启
        }

    

        public dbcontent(string name)
        {
          
          
        }



        private ModelOperationScope scope { get; set; }

        public ModelOperationScope dotrancation(Action Fun)
        {
            try
            {
                Fun();
            }
            catch (Exception ex)
            {
                scope.IsOperationScopeBool = false;
                LogService.Default.Debug("do trancation  error, message:" + ex.Message);
            }
            finally
            {
            }
            return scope;
        }

        /// <summary>
        ///     主动销毁对象
        /// </summary>
        public void Dispose()
        {
            scope.Close();
        }
    }
}