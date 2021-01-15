using System;

namespace Bitter.Tools
{
    /// <summary>
    /// 返回计算的结果值
    /// </summary>
    public class CalculationResult
    {
        public string ErrorInfo { get; set; }
        public Exception ex { get; set; }
        public decimal Result { get; set; }
    }

    public class FinanceCalculation
    {
        /// <summary>
        /// 获取银行手续费
        /// </summary>
        /// <param name="FEvaluatePrice">评估价</param>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <param name="FBankRate">银行利率</param>
        /// <returns>计算结果值</returns>
        public static CalculationResult GetFBankFee(decimal FEvaluatePrice, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                //var z = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)); //实际贷款额
                r.Result = Math.Round(((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) * FBankRate) / 100, 2);
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 计算上浮比例
        /// </summary>
        /// <param name="FEvaluatePrice">评估价</param>
        /// <param name="FCarAmount">车价</param>
        /// <returns>计算结果值</returns>
        public static CalculationResult GetFEvaluatePriceRatio(decimal FEvaluatePrice, decimal FCarAmount)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                var re = (FEvaluatePrice - FCarAmount);
                if (re < 0)
                {
                    r.Result = 0;
                }
                var k = (re / FEvaluatePrice) * 100;
                r.Result = k;
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 计算首付比例
        /// </summary>
        /// <param name="FCarAmount">车价</param>
        /// <param name="FEvaluatePrice">评估价</param>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <param name="FBankRate">银行利率</param>
        /// <returns>执行结果</returns>
        public static CalculationResult GetFFirstPayAmountRatio(decimal FEvaluatePrice, decimal FCarAmount, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                r.Result = Math.Round((((FEvaluatePrice - ((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)))) / FCarAmount) * 100), 2);
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary> 计算担保手续费 <param name="FEvaluatePrice">评估价</param> <param
        /// name="FMonthPayAmount">月还款</param> <param name="FLoanPeriod">按揭期限</param> <param
        /// name="FLoanAmount">贷款金额</param> <param name="FBankRate">银行利率</param> <returns>计算结果值</returns>
        public static CalculationResult GetFGuaranteeFee(decimal FEvaluatePrice, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                //var z = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)); //实际贷款额
                r.Result = ((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) - FLoanAmount);
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 计算贷款担保额（本息合计）
        /// </summary>
        /// <param name="FEvaluatePrice">评估价</param>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <param name="FBankRate">银行利率</param>
        /// <returns>计算结果值</returns>
        public static CalculationResult GetFLoanSumAmount(decimal FEvaluatePrice, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                //var z = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)); //实际贷款额
                r.Result = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) + (Math.Round(((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) * FBankRate) / 100, 2));
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 获取实际首付款
        /// </summary>
        /// <param name="FEvaluatePrice">评估价</param>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <param name="FBankRate">银行利率</param>
        /// <returns>执行结果</returns>
        public static CalculationResult GetFRealFirstAmoun(decimal FEvaluatePrice, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                //var z = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)); //实际贷款金额
                r.Result = (FEvaluatePrice - (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)));
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary> 计算实际首付比例 <param name="FCarAmount">车价</param> <param
        /// name="FEvaluatePrice">评估价</param> <param name="FMonthPayAmount">月还款</param> <param
        /// name="FLoanPeriod">按揭期限</param> <param name="FLoanAmount">贷款金额</param> <param
        /// name="FBankRate">银行利率</param> <returns>执行结果</returns>
        public static CalculationResult GetFRealFirstPayAmountRatio(decimal FEvaluatePrice, decimal FCarAmount, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                // var p = (FEvaluatePrice - (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod /
                // FLoanAmount) - 1) * 100)) / (100 + FBankRate))); //实际首付款
                r.Result = Math.Round((((FEvaluatePrice - (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate))) / FCarAmount) * 100), 2);
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 实际贷款额
        /// </summary>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FBankRate">银行利率</param>
        /// <returns>执行结果</returns>
        public static CalculationResult GetFRealLoanAmount(decimal FLoanAmount, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FBankRate, int FLoanBank)
        {
            CalculationResult r = new CalculationResult();

            decimal FLoanRate = Math.Round(FMonthPayAmount * FLoanPeriod / FLoanAmount - 1, 5, MidpointRounding.AwayFromZero);

            decimal a = Math.Ceiling(FLoanAmount * Math.Round((Math.Ceiling(FLoanRate * 1000000) / 1000000 + 1), 5, MidpointRounding.AwayFromZero) / (FBankRate / 100 + 1)) % 2;

            try
            {
                if (FLoanBank == 20)
                {
                    r.Result = Math.Ceiling(FLoanAmount * Math.Round((Math.Ceiling(FLoanRate * 1000000) / 1000000 + 1), 5, MidpointRounding.AwayFromZero) / (FBankRate / 100 + 1));
                }
                else if (FLoanBank == 3)
                {
                    r.Result = Math.Round(FLoanAmount * Math.Round((Math.Ceiling(FLoanRate * 1000000) / 1000000 + 1), 5, MidpointRounding.AwayFromZero) / (FBankRate / 100 + 1) / 100, 0, MidpointRounding.AwayFromZero) * 100;
                }
                else
                {
                    if (a == 1)
                    {
                        r.Result = Math.Ceiling(FLoanAmount * Math.Round((Math.Ceiling(FLoanRate * 1000000) / 1000000 + 1), 5, MidpointRounding.AwayFromZero) / (FBankRate / 100 + 1)) + 1;
                    }
                    else
                    {
                        r.Result = Math.Ceiling(FLoanAmount * Math.Round((Math.Ceiling(FLoanRate * 1000000) / 1000000 + 1), 5, MidpointRounding.AwayFromZero) / (FBankRate / 100 + 1));
                    }
                }
                //decimal k=((FMonthPayAmount*FLoanPeriod/FLoanAmount)-1)*100 //实际贷款利率
                //r.Result = Math.Ceiling((FMonthPayAmount * FLoanPeriod * 100) / (100 + FBankRate));
                //r.Result = ((FLoanAmount) * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100))) / (100 + FBankRate);
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 计算实际贷款利率
        /// </summary>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <returns>计算结果值</returns>
        public static CalculationResult GetFRealLoanRate(decimal FMonthPayAmount, decimal FLoanPeriod, decimal FLoanAmount)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                r.Result = ((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100;
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 计算实际贷款比例
        /// </summary>
        /// <param name="FEvaluatePrice">评估价</param>
        /// <param name="FMonthPayAmount">月还款</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="FLoanAmount">贷款金额</param>
        /// <param name="FBankRate">银行利率</param>
        /// <returns>计算结果值</returns>
        public static CalculationResult GetFRealLoanRatio(decimal FEvaluatePrice, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                //var z = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)); //实际贷款额
                //var k = (((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) * FBankRate) / 100); //银行手续费
                r.Result = Math.Round(((((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) + (((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) * FBankRate) / 100)) / FEvaluatePrice) * 100), 2);
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }

        /// <summary>
        /// 计算每月实际还款
        /// </summary>
        /// <param name="FRealLoanAmount">实际贷款额</param>
        /// <param name="FLoanPeriod">按揭期限</param>
        /// <param name="BankEnum">银行枚举</param>
        /// <returns>计算结果值</returns>
        public static CalculationResult GetFRealMonthPayAmount(decimal FEvaluatePrice, decimal FMonthPayAmount, Int32 FLoanPeriod, decimal FLoanAmount, decimal FBankRate, Int32 iSCeiling)
        {
            CalculationResult r = new CalculationResult();

            try
            {
                // var z = (FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1)
                // * 100)) / (100 + FBankRate)); //实际贷款额
                var k = (((FLoanAmount * (100 + (((FMonthPayAmount * FLoanPeriod / FLoanAmount) - 1) * 100)) / (100 + FBankRate)) * ((FBankRate / 100) + 1)) / FLoanPeriod);

                if (iSCeiling == 1)
                {
                    r.Result = Math.Ceiling(k);
                }
                else
                {
                    r.Result = Math.Floor(k);
                }
            }
            catch (Exception ex)
            {
                r.ex = ex;
                r.ErrorInfo = "系统计算错误!";
            }
            return r;
        }
    }
}