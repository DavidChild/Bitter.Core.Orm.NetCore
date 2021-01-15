using Bitter.Tools.Helper;
using Bitter.Tools.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
 
namespace Bitter.Base
{
    /********************************************************************************
    ** auth： davidchild
    ** date： 2017/3/24 15:06:22   
    ** desc： 异常扩展
    ** Ver.:  V1.0.0
    ** Copyright (C) 2016 Bitter 版权所有。
    *********************************************************************************/

    public static class ParamterCheckExtensions
    {
        #region //关于文件或者路径的校验
        /// <summary>
        /// 检查指定路径的文件夹必须存在，否则抛出异常。
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="paramName">参数名称。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="DirectoryNotFoundException"></exception>
        public static Exception CheckDirectoryExists(this string directory, string paramName = null)
        {
            Exception ex = null;
            ex=CheckNotNull(directory, paramName);
            if (ex != null) return ex;
           ex= Require<DirectoryNotFoundException>(Directory.Exists(directory),
                string.Format(SysErrorCodeDefinition.ParameterCheck_DirectoryNotExists,
                    directory),paramName);
           return ex;
        }
        /// <summary>
        /// 检查指定路径的文件必须存在，否则抛出 <see cref="FileNotFoundException"/> 异常。
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="paramName">参数名称。</param>
        /// <exception cref="ArgumentNullException">当文件路径为null时</exception>
        /// <exception cref="FileNotFoundException">当文件路径不存在时</exception>
        public static Exception CheckFileExists(this string filename, string paramName = null)
        {
            Exception ex = null;
           ex= CheckNotNull(filename, paramName);
           if (ex != null) return ex;
            ex=Require<FileNotFoundException>(File.Exists(filename),
                string.Format(SysErrorCodeDefinition.ParameterCheck_FileNotExists,
                    filename),paramName);
            return ex;
        }

        #endregion

        #region //关于算数类型的校验
        /// <summary>
        /// 检查参数必须在指定范围之间，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="start">比较范围的起始值。</param>
        /// <param name="end">比较范围的结束值。</param>
        /// <param name="startEqual">是否可等于起始值</param>
        /// <param name="endEqual">是否可等于结束值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckBetween<T>(this T value, string paramName, T start, T end, bool startEqual = false,
            bool endEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            bool flag = startEqual ? value.CompareTo(start) < 0 : value.CompareTo(start) <=0;
            string message = startEqual
                ? string.Format(SysErrorCodeDefinition.ParameterCheck_BetweenNotEqual,
                    paramName, start, end, start)
                : string.Format(SysErrorCodeDefinition.ParameterCheck_Between, paramName,
                    start, end);
            ex = Require<ArgumentOutOfRangeException>(flag, message, paramName);
           if (ex != null) return ex;
           flag = endEqual ? value.CompareTo(end) >0 : value.CompareTo(end) >= 0;
            message = endEqual
                ? string.Format(SysErrorCodeDefinition.ParameterCheck_BetweenNotEqual,
                    paramName, start, end, end)
                : string.Format(SysErrorCodeDefinition.ParameterCheck_Between, paramName,
                    start, end);
            ex = Require<ArgumentOutOfRangeException>(flag, message, paramName);
            return ex;
        }
        /// <summary>
        /// 检查参数必须大于[或可等于，参数canEqual]指定值，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckGreaterThan<T>(this T value, string paramName, T target, bool canEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            bool flag = canEqual ? value.CompareTo(target) <0 : value.CompareTo(target) <= 0;
            string format = canEqual
                ? SysErrorCodeDefinition.ParameterCheck_NotGreaterThanOrEqual
                : SysErrorCodeDefinition.ParameterCheck_NotGreaterThan;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), paramName);
            return ex;
        }

        /// <summary>
        /// 检查参数必须小于[或可等于，参数canEqual]指定值，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckLessThan<T>(this T value, string paramName, T target, bool canEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            bool flag = canEqual ? value.CompareTo(target) > 0 : value.CompareTo(target) >= 0;
            string format = canEqual
                ? SysErrorCodeDefinition.ParameterCheck_NotLessThanOrEqual
                : SysErrorCodeDefinition.ParameterCheck_NotLessThan;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), paramName);
            return ex;
        }
        
        /// <summary>
        /// 检查INT 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this int? value, string paramName)
        {
            Exception ex = null;
            ex = value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查INT 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this int value, string paramName)
        {
            Exception ex = null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=Require<ArgumentException>(value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查decimal 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this decimal? value, string paramName)
        {
            Exception ex = null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查decimal 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this decimal value, string paramName)
        {
            Exception ex = null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=Require<ArgumentException>(value <= 0M,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查long 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this long? value, string paramName)
        {
            Exception ex = null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        } 
        /// <summary>
        /// 检查long 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this long value, string paramName)
        {
            Exception ex = null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=Require<ArgumentException>(value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        #endregion

        #region//关于算数类型的校验,依赖条件的Lambda表达式
        /// <summary>
        /// 检查参数必须在指定范围之间，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="start">比较范围的起始值。</param>
        /// <param name="end">比较范围的结束值。</param>
        /// <param name="startEqual">是否可等于起始值</param>
        /// <param name="endEqual">是否可等于结束值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckBetween<T, TDto>(this T value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName, T start, T end, bool startEqual = false,
            bool endEqual = false)where TDto:new()
            where T : IComparable<T>
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() ==0)
            {
                return null;
            }
            bool flag = startEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            string message = startEqual
                ? string.Format(SysErrorCodeDefinition.ParameterCheck_BetweenNotEqual,
                    paramName, start, end, start)
                : string.Format(SysErrorCodeDefinition.ParameterCheck_Between, paramName,
                    start, end);
            ex = Require<ArgumentOutOfRangeException>(flag, message, paramName);
           if (ex != null) return ex;
            flag = endEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            message = endEqual
                ? string.Format(SysErrorCodeDefinition.ParameterCheck_BetweenNotEqual,
                    paramName, start, end, end)
                : string.Format(SysErrorCodeDefinition.ParameterCheck_Between, paramName,
                    start, end);
            ex = Require<ArgumentOutOfRangeException>(flag, message, paramName);
           return ex;
        }
        /// <summary>
        /// 检查参数必须大于[或可等于，参数canEqual]指定值，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckGreaterThan<T, TDto>(this T value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName, T target, bool canEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            bool flag = canEqual ? value.CompareTo(target) >= 0 : value.CompareTo(target) > 0;
            string format = canEqual
                ? SysErrorCodeDefinition.ParameterCheck_NotGreaterThanOrEqual
                : SysErrorCodeDefinition.ParameterCheck_NotGreaterThan;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), paramName);
            return ex;
        }
        /// <summary>
        /// 检查参数必须小于[或可等于，参数canEqual]指定值，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckLessThan<T, TDto>(this T value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName, T target, bool canEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            bool flag = canEqual ? value.CompareTo(target) <= 0 : value.CompareTo(target) < 0;
            string format = canEqual
                ? SysErrorCodeDefinition.ParameterCheck_NotLessThanOrEqual
                : SysErrorCodeDefinition.ParameterCheck_NotLessThan;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), paramName);
            return ex;
        }
        /// <summary>
        /// 检查INT 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero<TDto>(this int? value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName)
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
           ex=  Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
           return ex;
        }
        /// <summary>
        /// 检查INT 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero<TDto>(this int value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName)
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex= Require<ArgumentException>(value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
           return ex;
        }
        /// <summary>
        /// 检查decimal 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero<TDto>(this decimal? value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName)
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
           ex= value.CheckNotNull(paramName);
           if (ex != null) return ex;
            ex=Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查decimal 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero<TDto>(this decimal value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName)
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            value.CheckNotNull(paramName);
            ex=Require<ArgumentException>(value <= 0M,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查long 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero<TDto>(this long? value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName)
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            value.CheckNotNull(paramName);
           ex= Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
           return ex;
        }
        /// <summary>
        /// 检查long 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero<TDto>(this long value, TDto dto, Func<TDto, bool> lambdaCondition, string paramName)
        {
            Exception ex = null;
            System.Collections.Generic.List<TDto> ls = new List<TDto>();
            ls.Add(dto);
            if (ls.Where(lambdaCondition).Count() == 0)
            {
                return null;
            }
            value.CheckNotNull(paramName);
           ex= Require<ArgumentException>(value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
           return ex;
        }
        #endregion

        #region//关于算数类型的校验,依赖断言式真假
        /// <summary>
        /// 检查参数必须在指定范围之间，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="start">比较范围的起始值。</param>
        /// <param name="end">比较范围的结束值。</param>
        /// <param name="startEqual">是否可等于起始值</param>
        /// <param name="endEqual">是否可等于结束值</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckBetween<T>(this T value, bool condition, string paramName, T start, T end, bool startEqual = false,
            bool endEqual = false)
           where T : IComparable<T>
        {
            Exception ex = null;
            if (!condition) return null;
            bool flag = startEqual ? value.CompareTo(start) >= 0 : value.CompareTo(start) > 0;
            string message = startEqual
                ? string.Format(SysErrorCodeDefinition.ParameterCheck_BetweenNotEqual,
                    paramName, start, end, start)
                : string.Format(SysErrorCodeDefinition.ParameterCheck_Between, paramName,
                    start, end);
            ex = Require<ArgumentOutOfRangeException>(flag, message, paramName);
           if (ex != null) return ex;

            flag = endEqual ? value.CompareTo(end) <= 0 : value.CompareTo(end) < 0;
            message = endEqual
                ? string.Format(SysErrorCodeDefinition.ParameterCheck_BetweenNotEqual,
                    paramName, start, end, end)
                : string.Format(SysErrorCodeDefinition.ParameterCheck_Between, paramName,
                    start, end);
            ex = Require<ArgumentOutOfRangeException>(flag, message, paramName);
           return ex;
        }
        /// <summary>
        /// 检查参数必须大于[或可等于，参数canEqual]指定值，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckGreaterThan<T>(this T value, bool condition, string paramName, T target, bool canEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            if (!condition) return null;
            bool flag = canEqual ? value.CompareTo(target) >= 0 : value.CompareTo(target) > 0;
            string format = canEqual
                ? SysErrorCodeDefinition.ParameterCheck_NotGreaterThanOrEqual
                : SysErrorCodeDefinition.ParameterCheck_NotGreaterThan;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), paramName);
            return ex;
        }

        /// <summary>
        /// 检查参数必须小于[或可等于，参数canEqual]指定值，否则抛出 <see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <typeparam name="T">参数类型。</typeparam>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <param name="target">要比较的值。</param>
        /// <param name="canEqual">是否可等于。</param>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static Exception CheckLessThan<T>(this T value, bool condition, string paramName, T target, bool canEqual = false)
            where T : IComparable<T>
        {
            Exception ex = null;
            if (!condition) return null;

            bool flag = canEqual ? value.CompareTo(target) <= 0 : value.CompareTo(target) < 0;
            string format = canEqual
                ? SysErrorCodeDefinition.ParameterCheck_NotLessThanOrEqual
                : SysErrorCodeDefinition.ParameterCheck_NotLessThan;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, target), paramName);
           return ex;
        }

        /// <summary>
        /// 检查INT 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this int? value, bool condition, string paramName)
        {
            Exception ex = null;
            if (!condition) return null;
          ex=value.CheckNotNull(paramName);
          if (ex != null) return ex;
           ex= Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查INT 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this int value, bool condition, string paramName)
        {
            Exception ex = null;
            if (!condition) return null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex= Require<ArgumentException>(value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查decimal 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this decimal? value, bool condition, string paramName)
        {
            Exception ex = null;
            if (!condition) return null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
           ex= Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
           return ex;
        }
        /// <summary>
        /// 检查decimal 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this decimal value, bool condition, string paramName)
        {
            Exception ex = null;
            if (!condition) return null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
             ex= Require<ArgumentException>(value <= 0M,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
           return ex;
        }
        /// <summary>
        /// 检查long 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this long? value, bool condition, string paramName)
        {
            Exception ex = null;
            if (!condition) return null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex=   Require<ArgumentException>(value.Value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        /// <summary>
        /// 检查long 类型的参数必须大于零，否则就抛出指定断言的异常<see cref="ArgumentOutOfRangeException"/> 异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception CheckNotNullAndNotIsZero(this long value, bool condition, string paramName)
        {
            Exception ex = null;
            if (!condition) return null;
            ex=value.CheckNotNull(paramName);
            if (ex != null) return ex;
            ex= Require<ArgumentException>(value <= 0,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNullAndNotIsZero,
                    paramName), paramName);
            return ex;
        }
        #endregion

        #region //正则表达式验证
        /// <summary>
        /// 正则表达式验证
        /// </summary>
        /// <param name="value"></param>
        /// <param name="pattern">正则表达式</param>
        /// <param name="paramName"></param>
        public static Exception CheckRegular(this string value, string pattern, string paramName)
        {
            Exception ex = null;
            Regex gex = new Regex(pattern);
            bool flag=false;
            if (!gex.IsMatch(value))
                flag = true;
            string format = SysErrorCodeDefinition.ParameterCheck_Regular;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(format, paramName, pattern), paramName);
           return ex;
        }

        #endregion
        
        #region //String类型参数相关校验
        /// <summary>
        /// 检测字符串是否在指定长度内
        /// </summary>
        /// <param name="value"></param>
        /// <param name="MaxLength">最大长度</param>
        /// <param name="MinLength">最小长度</param>
        public static Exception CheckStringLength(this string value, int MaxLength, int MinLength, string paramName)
        {
            Exception ex = null;
            bool flag = false;
            if (value.Length <= MaxLength)
            {
                if (MinLength > 0)
                {
                    if (value.Length < MinLength)
                        flag = true;
                }
            }
            else
                flag = true;
            string fomat = SysErrorCodeDefinition.ParameterCheck_StringLength;
            ex = Require<ArgumentOutOfRangeException>(flag, string.Format(fomat, paramName, MinLength, MaxLength), paramName);
           return ex;
        }

        /// <summary>
        /// 校验是否是正确格式的电话号码
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName"></param>
        public static Exception IsPhoneNumber(this string value, string paramName)
        {
            Exception ex = null;
            value = value.Trim();
            var _d = new Regex(@"/^1[3578][01379]\d{8}$/g");
            var _l = new Regex(@"/^1[34578][01256]\d{8}$/g");
            var _y = new Regex(@"/^(134[012345678]\d{7}|1[34578][012356789]\d{8})$/g");
            var flag = true;
            if (_d.IsMatch(value))
                flag = false;
            else if (_l.IsMatch(value))
                flag = false;
            else if (_y.IsMatch(value))
                flag = false;
            else
                flag = true;
            string format = SysErrorCodeDefinition.ParameterCheck_PhpneNumber;
            ex = Require<ArgumentException>(flag, string.Format(format, paramName), paramName);
            return ex;
        }
        #endregion

        #region //校验不能为空或者null
        /// <summary>
        /// 检查Guid值不能为Guid.Empty，否则抛出异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <exception cref="ArgumentException"></exception>
        public static Exception CheckNotEmpty(this Guid value, string paramName)
        {
            Exception ex = null;
            ex= Require<ArgumentException>(value == Guid.Empty,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotEmpty_Guid,
                    paramName), paramName);
           return ex;
        }

        /// <summary>
        /// 检查参数不能为空引用，否则抛出&gt;异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static Exception CheckNotNull<T>(this T value, string paramName )
        {
            Exception ex = null;
            ex=  Require<ArgumentException>(value == null,
                string.Format(SysErrorCodeDefinition.ParameterCheck_NotNull, paramName), paramName);
            return ex;
        }


        /// <summary>
        /// 检查字符串不能为空引用或空字符串，否则抛出异常或异常。
        /// </summary>
        /// <param name="value"></param>
        /// <param name="paramName">参数名称。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Exception CheckNotNullOrEmpty(this string value, string paramName)
        {
            Exception ex = null;
            ex= value.CheckNotNull(paramName);
          
            if (ex != null) return ex;
            ex=Require<ArgumentException>(!(value.Length > 0),
                string.Format(
                    SysErrorCodeDefinition.ParameterCheck_NotNullOrEmpty_String, paramName), paramName);
            return ex;
        }

        /// <summary>
        /// 检查集合不能为空引用或空集合，否则抛出 <see cref="ArgumentNullException"/> 异常或 <see
        /// cref="ArgumentException"/> 异常。
        /// </summary>
        /// <typeparam name="T">集合项的类型。</typeparam>
        /// <param name="collection"></param>
        /// <param name="paramName">参数名称。</param>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public static Exception CheckNotNullOrEmpty<T>(this IEnumerable<T> collection, string paramName)
        {
              Exception ex = null;
              ex=collection.CheckNotNull(paramName);
              if (ex != null) return ex;
              ex=Require<ArgumentException>(collection.Any(),
                    string.Format(
                        SysErrorCodeDefinition.ParameterCheck_NotNullOrEmpty_Collection,
                        paramName), paramName);
             return ex;
        }

        /// <summary>
        /// 服务器错误匹配(10000+)，匹配不上的直接返回Exception，匹配上的返回自定义MSG
        /// </summary>
        /// <param name="ex"></param>
        public static Exception CheckRequestException(Exception ex)
        {
            Exception ex1 = null;
            if (!(ex is WebException))
            {
                return ex;
            }
            string key = ((System.Net.WebException)ex).Status.ToString();
            string val = BaseResource.GetResourceValue(key);
            if (val == "none")
            {
                ex1 = Require<Exception>(true, ex.Message);
            }
            else
            {
                string[] lst = StringHelper.GetSplitException(val);
                if (lst.Length == 2 && lst[1].ToSafeInt32(0) > 10000)
                {
                    ex1 = Require<ResultException>(true, val);
                }
                else
                {
                    ex1 = Require<Exception>(true, ex.Message);
                }
            }
            return ex;
        }

        /// <summary>
        /// 验证指定值的断言表达式是否为真，不为值抛出异常
        /// </summary>
        /// <param name="value"></param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        public static Exception Required<T>(this T value, Func<T, bool> assertionFunc, string message, string paramName = null)
        {
            if (assertionFunc == null)
            {
                return new ArgumentNullException("assertionFunc");
            }
            return Require<Exception>(assertionFunc(value), message, paramName);
        }

        /// <summary>
        /// 验证指定值的断言表达式是否为真，不为真抛出异常
        /// </summary>
        /// <typeparam name="T">要判断的值的类型</typeparam>
        /// <typeparam name="T Bitter.BaseException">抛出的异常类型</typeparam>
        /// <param name="value">要判断的值</param>
        /// <param name="assertionFunc">要验证的断言表达式</param>
        /// <param name="message">异常消息</param>
        public static Exception Required<T, TException>(this T value, Func<T, bool> assertionFunc, string message, string paramName = null)
            where TException : Exception
        {
            if (assertionFunc == null)
            {
               return new  ArgumentNullException("assertionFunc");
            }
            return Require<TException>(assertionFunc(value), message, paramName);
        }
        /// <summary>
        /// 验证指定值的断言是否为真，如果不为真，抛出指定消息的指定类型异常
        /// </summary>
        /// <typeparam name="TException">异常类型</typeparam>
        /// <param name="assertion">要验证的断言。</param>
        /// <param name="message">异常消息。</param>
        private static Exception Require<TException>(bool assertion, string message, string paramName=null) where TException : Exception
        {
            if (assertion)
            {
                TException exception = (TException)Activator.CreateInstance(typeof(TException), message);
                if (!string.IsNullOrEmpty(paramName))
                {
                    exception.Data.Add("paramName", paramName);
                }
                return exception;
            }
            else if (string.IsNullOrEmpty(message))
            {
                ArgumentNullException ex = new ArgumentNullException("message");
               if (!string.IsNullOrEmpty(paramName))
                {
                    ex.Data.Add("paramName", paramName);
                }
                
                return ex;
            }
            else 
            {
                return null;
            }
         
        }
        #endregion

       
    }
}