using System;
using System.CodeDom;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using Bitter.Base;
using Bitter.Tools;

namespace Bitter.Core
{ 
    /// <summary>
    /// 如果要使用事务二次提交的时候,请使用Using(dbscope db=new dbscope()){}, 来确保尽可能早的释放对象资源
    /// 
    /// </summary>
    public class dbscope : IDisposable
    {


       
        private TaskCompletionSource<bool> taskSource;

        public dbscope()
        {

        }

        /// <summary>
        /// 将外部的List<SqlQuery> 初始化本地事务中受事务管理的--List<SqlQuery> doScopeList对象
        /// </summary>
        /// <param name="list">将外部的List<SqlQuery> 初始化本地事务中受事务管理的--List<SqlQuery> doScopeList对象</param>
        public dbscope(List<BaseQuery> list)
        {
            doScopeList = list;
        }

        /// <summary>
        /// 将外部的List<BaseModel> 初始化本地事务中受事务管理的--List<BaseModel> objList对象
        /// </summary>
        /// <param name="list">将外部的List<BaseModel> 初始化本地事务中受事务管理的--List<BaseModel> objList对象</param>
        public dbscope(List<BaseModel> list)
        {
            objList = list;
        }

        /// <summary>
        /// 将外部的List<BaseModel> 初始化本地事务中受事务管理的--List<BaseModel> objList对象
        /// 将外部的List<SqlQuery> 初始化本地事务中受事务管理的--List<SqlQuery> doScopeList对象
        /// </summary>
        /// <param name="doScopeList">外部的List<SqlQuery> 初始化本地事务中受事务管理的--List<SqlQuery> doScopeList对象</param>
        /// <param name="objList">将外部的List<BaseModel> 初始化本地事务中受事务管理的--List<BaseModel> objList对象</param>
        public dbscope(List<BaseQuery> doScopeList,List<BaseModel>  objList)
        {
            this.objList = objList;
            this.doScopeList = doScopeList;
        }



        private string _targetdb { get; set; }


        /// <summary>
        /// doScopeList 事物执行语句容器
        /// </summary>
        private List<BaseQuery> doScopeList = new List<BaseQuery>();
        /// <summary>
        /// 执行成功后的记录
        /// </summary>
        private List<BaseModel> objList = new List<BaseModel>();


        /// <summary>
        ///  大批量操作的执行语句
        /// </summary>
        private List<BulkCopyModel> bulkList = new List<BulkCopyModel>();

        /// <summary>
        ///  大批量操作的DataTable
        /// </summary>
        private List<DataTable> bulkTablies = new List<DataTable>();
        /// <summary>
        ///  大批量操作的DataTable
        /// </summary>
        public List<DataTable> BulkTablies
        {
            get { return bulkTablies; }

        }

        /// <summary>
        /// 方法执行过程中是否有异常
        /// </summary>
        private bool isNoneException = true;

        

        /// <summary>
        /// 事务执行失败后，此值必有
        /// </summary>
        public Exception exception { get; set; }

        /// <summary>
        /// 获取当前执行过程中获取到的异常信息
        /// </summary>
        public Exception ScopeException
        {
            get { return exception; }
        }

        /// <summary>
        /// 事务执行详情
        /// </summary>
        public ScopeCommandInfo CommandInfo = new ScopeCommandInfo();

        /// <summary>
        /// ExternalSucessed【外部事务执行情况】:分布式事务提交,二次提交依赖外部条件,二次提交时一定要注意外部事务提交或者应用执行的时间
        /// 如果外部事务执行时间过长，会导致本事务的连接线程占用过长,会出现数据库连接泄露的情况出现(尤其是在分布式事务中,一定需要调整好外部服务的调用时间)。
        /// 默认情况为true,具体情况请根据当时业务场景处理
        /// </summary>
        public bool ExternalSucessed = true;

        /// <summary>
        /// 对事务二次提交的支持
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        private DistributedTransaction dbSecodeScope;


        /// <summary>
        /// 是否是立即提交，非分布式事务二次提交
        /// </summary>
        private bool IsImmediately = true;

        /// <summary>
        /// 重置资源,以便下个事务使用
        /// </summary>
        public void Reset()
        {
            taskSource = null;
            doScopeList.Clear();
            objList.Clear();
            bulkList.Clear();
            bulkTablies.Clear();
            exception = null;
            ExternalSucessed = true;
            CommandInfo.Reset();
            dbSecodeScope = null;
            IsImmediately = true;
        }

        /// <summary>
        /// 事务一次提交
        /// timeOut【指定操作的调用的外部服务或者方法执行的超时时间】:分布式事务提交,二次提交依赖外部条件,二次提交时一定要注意外部事务提交或者应用执行的时间
        /// 如果外部事务执行时间过长，会导致本事务的连接线程占用过长,会出现数据库连接泄露的情况出现(尤其是在分布式事务中,一定需要调整好外部服务的调用时间)。
        /// timeOut默认情况下可以不用设置，建议根据业务实际情况设置此值.
        /// </summary>
        public dbscope FirstSubmit(int? timeOut = null)
        {

            IsImmediately = false;
            dbSecodeScope = new DistributedTransaction();
            dbSecodeScope.DistributedEventException += (sender) =>
            {
                isNoneException = false;
                exception = (Exception)sender;
                CommonDealException();
            };
            dbSecodeScope.SetListQuery = doScopeList;
            //处理bulkList

            dbSecodeScope.BulkTablies = this.bulkTablies;
            dbSecodeScope.SetbulkList = this.bulkList;
            
            CommandInfo = dbSecodeScope.ComamInfo;
            dbSecodeScope.BeginTransaction();
            if (timeOut.HasValue)
            {
                taskSource = new TaskCompletionSource<bool>();
                RegisetiOverTime(taskSource, timeOut.Value);

            }
            return this;
        }


        /// <summary>
        /// 事务一次提交
        /// timeOut【指定操作的调用的外部服务或者方法执行的超时时间】:分布式事务提交,二次提交依赖外部条件,二次提交时一定要注意外部事务提交或者应用执行的时间
        /// 如果外部事务执行时间过长，会导致本事务的连接线程占用过长,会出现数据库连接泄露的情况出现(尤其是在分布式事务中,一定需要调整好外部服务的调用时间)。
        /// timeOut默认情况下可以不用设置，建议根据业务实际情况设置此值.
        /// </summary>
        public dbscope FirstSubmit(out bool noneError,out string errorMessage, int? timeOut = null)
        {

            dbscope db = new dbscope();
           
            noneError = true;
            IsImmediately = false;
            errorMessage = string.Empty;
            dbSecodeScope = new DistributedTransaction(_targetdb);
            dbSecodeScope.DistributedEventException += (sender) =>
            {
                isNoneException = false;
                exception = (Exception)sender;
                
                CommonDealException();
            };
            dbSecodeScope.SetListQuery = doScopeList;
            //处理bulkList

            dbSecodeScope.BulkTablies = this.bulkTablies;
            dbSecodeScope.SetbulkList = this.bulkList;

            CommandInfo = dbSecodeScope.ComamInfo;
            noneError= dbSecodeScope.BeginTransaction();
            if (!noneError)
            {
                errorMessage = exception.Message;
            }
            
            if (timeOut.HasValue)
            {
                taskSource = new TaskCompletionSource<bool>();
                RegisetiOverTime(taskSource, timeOut.Value);

            }
            return this;
        }

        /// <summary>
        /// 超时，二次提交处理超时问题
        /// </summary>
        /// <param name="timeOut"></param>
        private void RegisetiOverTime(TaskCompletionSource<bool> ntaskSource, int timeOut)
        {
            var tcs = ntaskSource;
            if (tcs == null) return;
            var task = tcs.Task;
            var taskTimeOut = Task.Delay(timeOut);
            Task.Factory.StartNew(() =>
            {

             
                if (Task.WhenAny(task, taskTimeOut).Result == taskTimeOut)
                {
                    isNoneException = false;
                    exception = new Exception("操作外部依赖服务超时.或者委托的Action方法执行超时.");
                    LogService.Default.Fatal("操作外部依赖服务超时.或者委托的Action方法执行超时.");
                  
                    //如果超时事务回滚
                    foreach (var po in this.objList)
                    {
                       po.Delete().Submit(po._targetdb);
                    }
                    dbSecodeScope.Rollback();
                    objList.Clear();
                    doScopeList.Clear();
                    bulkList.Clear();
                    bulkTablies.Clear();
                    _targetdb = string.Empty;
                
                    //为了兼容老版本
                   
                    //最终记录
                    if (dbSecodeScope.InsideSucessed) //如果当时一次提交成功，但是超时导致的,就直接记录调用及记录
                    {
                        

                    }

                }
            });
        }


        private object obj;
        /// <summary>
        ///anFunc 是一个返回为布尔值得匿名委托函数,当前的分布式事务会根据当前anFunc返回的布尔值进行决定对当前的本地事务是提交还是回滚.
        ///如果外部事务执行时间过长，会导致本事务的连接线程占用过长,会出现数据库连接泄露的情况出现(尤其是在分布式事务中,一定需要调整好外部服务的调用时间)。因此建议设置好一次提交的超时时间
        /// </summary>
        /// <param name="anFunc">外部服务调用的返回结果,返回的True,当前的分布式事务会根据当前anFunc返回的布尔值进行决定对当前的的本地事务时提交还是回滚.</param>
        public bool SecondSubmit(Func<bool> anFunc)
        {
            bool bl = false;
            if (dbSecodeScope.InsideSucessed)
            {
                 bl = doscoendtrancation(anFunc);
                if (taskSource != null)
                {
                    taskSource.SetResult(bl);
                }
            }
            else
            {
                if (taskSource != null)
                {
                    taskSource.SetResult(false);
                }

            }
            return doscoendsubmit(bl);
        }


        /// <summary>
        /// ExternalSucessed【外部事务执行情况】:分布式事务提交,二次提交依赖外部条件,二次提交时一定要注意外部事务提交或者应用执行的时间
        ///如果外部事务执行时间过长，会导致本事务的连接线程占用过长,会出现数据库连接泄露的情况出现(尤其是在分布式事务中,一定需要调整好外部服务的调用时间)。因此建议设置好一次提交的超时时间
        ///默认情况为true,具体情况请根据当时业务场景处理
        /// </summary>
        /// <param name="ExternalSucessed">外部事务执行情况或者外部服务执行情况.</param>
        /// <returns>返回分布事务（既二次提交的最终事务成功与否,成功:true,失败:false）</returns>
        public bool SecondSubmit(bool externalSucessed)
        {
            if (taskSource != null)
            {
                taskSource.SetResult(externalSucessed);
            }
            bool bl = externalSucessed;

            return doscoendsubmit(bl);
        }

        private bool doscoendsubmit(bool externalSucessed)
        {

            bool bl = true;
           
            try
            {
                if (!isNoneException)
                {
                    if (dbSecodeScope != null) dbSecodeScope.ExternalSucessed = false;
                    return false;
                }

                SetIsImmediatelyException(false);

                if (!isNoneException)
                {
                    if (dbSecodeScope != null) dbSecodeScope.ExternalSucessed = false;
                    return false;
                }

                if (dbSecodeScope != null)
                {
                    dbSecodeScope.ExternalSucessed = externalSucessed; //外部事务情况
                    dbSecodeScope.CommitTransaction();
                    bl = dbSecodeScope.IsScopeSucessed;
                }

                isNoneException = (bl);
                if (!isNoneException)
                {
                    //如果发生异常,那么删除在数据库中
                    foreach (var po in this.objList)
                    {
                        po.Delete().Submit(po._targetdb);
                    }
                    objList.Clear();
                    bulkList.Clear();
                    bulkTablies.Clear();
                    _targetdb = string.Empty;
                    bl = false;
                }
                else
                {
                    bl = true;
                }
            }
            catch (Exception exception)
            {
                bl = false;
                exception = exception;
                LogService.Default.Fatal("doscoendsubmit.执行事务失败(dbscope):" + exception.Message, exception);

            }
            finally
            {
                objList.Clear();
                doScopeList.Clear();
                bulkList.Clear();
                bulkTablies.Clear();
                _targetdb = string.Empty;
                //为了兼容老版本
             
                isNoneException = true;
                IsImmediately = true;


            }
            return bl;
        }

       


        /// <summary>
        /// 装载事务
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public bool doscoendtrancation(Func<bool> anFunc)
        {
            bool bl = true;
            try
            {
                bl = anFunc();
                return bl;

            }
            catch (Exception ex)
            {
                isNoneException = false;
                exception = ex;
                bl = false;
                LogService.Default.Fatal("doscoendtrancation.执行事务失败(dbscope):" + ex.Message, ex);
                return false;
            }
            finally
            {
                if(exception!=null)CommonDealException();
            }
        }


        private void CommonDealException()
        {
            if (!isNoneException)
            {

                //如果发生异常,那么删除在数据库中objList中的对象
                foreach (var po in this.objList)
                {
                    try
                    {
                        po.Delete().Submit(po._targetdb);
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }

                }
                objList.Clear();
                doScopeList.Clear();
                _targetdb = string.Empty;
                //为了兼容老版本
             
            }
        }



        /// <summary>
        /// 装载事务
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public dbscope dotrancation(Action<List<BaseQuery>, List<BaseModel>, List<BulkCopyModel>> scope)
        {
            try
            {
                scope(doScopeList, objList, bulkList);
                return this;

            }
            catch (Exception ex)
            {
                isNoneException = false;
                exception = ex;
                LogService.Default.Fatal("dotrancation.执行事务失败(dbscope):" + ex.Message, ex);
                return this;
            }
            finally
            {
                CommonDealException();
            }
        }

        /// <summary>
        /// 装载事务
        /// </summary>
        /// <param name="scope"></param>
        /// <returns></returns>
        public dbscope dotrancation(Action<List<BaseQuery>, List<BaseModel>> scope)
        {
            try
            {
                scope(doScopeList, objList);
                return this;

            }
            catch (Exception ex)
            {
                isNoneException = false;
                exception = ex;
                LogService.Default.Fatal("dotrancation.执行事务失败(dbscope):" + ex.Message, ex);
                return this;
            }
            finally
            {
                CommonDealException();
            }
        }




        private void SetIsImmediatelyException(bool isImmediatelySubmit)
        {

            if (IsImmediately != isImmediatelySubmit)
            {
                isNoneException = false;
                string fatal = "执行事务失败,不能同时在一个事务提交对象中同时使用分布式事务二次提交和进行本地事务一次提交，或者在二次提交事务时，先进行事务一次提交！";
                LogService.Default.Fatal(fatal);
                exception = new Exception(fatal);
                //如果发生异常,那么删除在数据库中
                foreach (var po in this.objList)
                {
                    po.Delete().Submit(po._targetdb);
                }
                objList.Clear();

            }
        }



        /// <summary>
        /// 执行事务提交
        /// </summary>
        /// <returns>true:事务执行成功,false事务执行失败</returns>
        public bool Submit()
        {



            bool bl = true;
            try
            {
                if (!isNoneException)
                {
                    return false;
                }
                SetIsImmediatelyException(true);

                if (!isNoneException)
                {
                    return false;
                }

                Exception Ex = null;

                bl = doScopeList.Submit(CommandInfo, out Ex, bulkList, bulkTablies,_targetdb);

                
                if ((!bl))
                {
                    exception = Ex;
                
                }

                isNoneException = (bl);
                if (!isNoneException)
                {
                    //如果发生异常,那么删除在数据库中
                    foreach (var po in this.objList)
                    {
                        po.Delete().Submit(po._targetdb);
                    }
                    objList.Clear();
                    bulkList.Clear();
                    bulkTablies.Clear();
                    bl = false;
                }
                else
                {
                    bl = true;
                }
            }
            catch (Exception exception)
            {
                bl = false;
                exception = exception;
                LogService.Default.Fatal("Submit.执行事务失败(dbscope):" + exception.Message, exception);

            }
            finally
            {
                objList.Clear();
                doScopeList.Clear();
                bulkList.Clear();
                bulkTablies.Clear();
                _targetdb = string.Empty;
              
                isNoneException = true;
                IsImmediately = true;


            }
            return bl;
        }

        /// <summary>
        /// 指向数据库连接
        /// </summary>
        /// <param name="dbconnectionName"></param>
        /// <returns></returns>
        public dbscope targetdb(string dbconnectionName)
        {
            this._targetdb = dbconnectionName;
            return this;
        }


        public void Dispose()
        {
            if (dbSecodeScope != null)
            {
                dbSecodeScope.Dispose();
            }
        }
    }
}
