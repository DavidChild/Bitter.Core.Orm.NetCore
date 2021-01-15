using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;

namespace Bitter.Core
{
    internal class ModelProxy
    {

       
        private const MethodAttributes GetSetMethodAttributes = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.CheckAccessOnOverride | MethodAttributes.Virtual | MethodAttributes.HideBySig;

        /// <summary>
        /// 创建动态程序集,返回AssemblyBuilder
        /// </summary>
        /// <param name="isSavaDll"></param>
        /// <returns></returns>
        //private static object CreateProxyOfInherit(Type impType , Type interceptorType = null ,bool isSavaDll = false)
        //{
      

        //    ModuleBuilder moduleBuilder = assembly.DefineDynamicModule(nameOfModule);
        //    TypeBuilder typeBuilder = moduleBuilder.DefineType(nameOfType, TypeAttributes.Public, impType);

        //}

        /// <summary>
        /// 创建动态模块,返回ModuleBuilder
        /// </summary>
        /// <param name="isSavaDll"></param>
        /// <returns>ModuleBuilder</returns>
        private static ModuleBuilder DefineDynamicModule(AssemblyBuilder dynamicAssembly)
        {
            ModuleBuilder moduleBuilder = null;
            moduleBuilder = dynamicAssembly.DefineDynamicModule(ProxyDefineConst.DynamicModuleName);
            return moduleBuilder;
        }

        /// <summary>
        /// 创建动态代理类,重写属性Get Set 方法,并监控属性的Set方法,把变更的属性名加入到list集合中,需要监控的属性必须是virtual
        /// 如果你想保存修改的属性名和属性值,修改Set方法的IL实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isSavaDynamicModule"></param>
        /// <returns></returns>
        public static T CreateDynamicProxy<T>()
        {
            string nameOfAssembly = typeof(T).Name + "ProxyAssembly";
            string nameOfModule = typeof(T) + "ProxyModule";
            string nameOfType = typeof(T) + "Proxy";

            Type modifiedPropertyNamesType = typeof(HashSet<string>);
            Type orgmodelType = typeof(T);

            Type typeNeedProxy = typeof(T);
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run); 
            //动态创建模块
            ModuleBuilder moduleBuilder = DefineDynamicModule(assemblyBuilder);
            string proxyClassName = string.Format(ProxyDefineConst.ProxyClassNameFormater, typeNeedProxy.Name);
            //动态创建类代理
            TypeBuilder typeBuilderProxy = moduleBuilder.DefineType(proxyClassName, TypeAttributes.Public, typeNeedProxy);
            //定义一个变量存放属性变更名
            FieldBuilder fbModifiedPropertyNames = typeBuilderProxy.DefineField(ProxyDefineConst.ModifiedPropertyNamesFieldName, modifiedPropertyNamesType, FieldAttributes.Public);
           


            FieldBuilder  orgmodelPropertyNames = typeBuilderProxy.DefineField(ProxyDefineConst.OrgmodelFiledName, orgmodelType, FieldAttributes.Public);

            /*
             * 构造函数 实例化 ModifiedPropertyNames,生成类似于下面的代码
               ModifiedPropertyNames = new List<string>();
            */
            ConstructorBuilder constructorBuilder = typeBuilderProxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            ILGenerator ilgCtor = constructorBuilder.GetILGenerator();
            ilgCtor.Emit(OpCodes.Ldarg_0);//加载当前类
            ilgCtor.Emit(OpCodes.Newobj, modifiedPropertyNamesType.GetConstructor(new Type[0]));//实例化对象入栈
            ilgCtor.Emit(OpCodes.Stfld, fbModifiedPropertyNames);//设置fbModifiedPropertyNames值,为刚入栈的实例化对象
            ilgCtor.Emit(OpCodes.Ldarg_0);
            ilgCtor.Emit(OpCodes.Newobj, orgmodelType.GetConstructor(new Type[0]));//实例化对象入栈
            ilgCtor.Emit(OpCodes.Stfld, orgmodelPropertyNames);//,为刚入栈的实例化对

            ilgCtor.Emit(OpCodes.Ret);//返回

            //获取被代理对象的所有属性,循环属性进行重写
            PropertyInfo[] properties = typeNeedProxy.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {

                string propertyName = propertyInfo.Name;
                Type typePepropertyInfo = propertyInfo.PropertyType;
                //动态创建字段和属性
                FieldBuilder fieldBuilder = typeBuilderProxy.DefineField("_" + propertyName, typePepropertyInfo, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilderProxy.DefineProperty(propertyName, PropertyAttributes.SpecialName, typePepropertyInfo, null);

                List<object> attrs = propertyInfo.GetCustomAttributes(true).ToList();

                attrs.ForEach(p =>
                {

                    Type myAttributeType = p.GetType();
                    //获取构造器信息
                    ConstructorInfo classAttributeInfo = myAttributeType.GetConstructor(new Type[0]);

                    //动态创建ClassCreatorAttribute
                    CustomAttributeBuilder myCABuilder = new CustomAttributeBuilder(
                                   classAttributeInfo, new object[] { });

                    propertyBuilder.SetCustomAttribute(myCABuilder);
                });




                //重写属性的Get Set方法
                var methodGet = typeBuilderProxy.DefineMethod("get_" + propertyName, GetSetMethodAttributes, typePepropertyInfo, Type.EmptyTypes);
                var methodSet = typeBuilderProxy.DefineMethod("set_" + propertyName, GetSetMethodAttributes, null, new Type[] { typePepropertyInfo });

                //il of get method
                var ilGetMethod = methodGet.GetILGenerator();
                ilGetMethod.Emit(OpCodes.Ldarg_0);
                ilGetMethod.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGetMethod.Emit(OpCodes.Ret);
                //il of set method
                ILGenerator ilSetMethod = methodSet.GetILGenerator();
                ilSetMethod.Emit(OpCodes.Ldarg_0);
                ilSetMethod.Emit(OpCodes.Ldarg_1);
                ilSetMethod.Emit(OpCodes.Stfld, fieldBuilder);
                ilSetMethod.Emit(OpCodes.Ldarg_0);
                ilSetMethod.Emit(OpCodes.Ldfld, fbModifiedPropertyNames);
                ilSetMethod.Emit(OpCodes.Ldstr, propertyInfo.Name);



                ilSetMethod.Emit(OpCodes.Callvirt, modifiedPropertyNamesType.GetMethod("Add", new Type[] { typeof(string) }));
                ilSetMethod.Emit(OpCodes.Pop);
                ilSetMethod.Emit(OpCodes.Ret);

                //设置属性的Get Set方法
                propertyBuilder.SetGetMethod(methodGet);
                propertyBuilder.SetSetMethod(methodSet);
            }

            //使用动态类创建类型
            TypeInfo proxyClassType = typeBuilderProxy.CreateTypeInfo();
           //创建类实例
            var instance = Activator.CreateInstance(proxyClassType);
            return (T)instance;
        }


        /// <summary>
        /// 创建动态代理类,重写属性Get Set 方法,并监控属性的Set方法,把变更的属性名加入到list集合中,需要监控的属性必须是virtual
        /// 如果你想保存修改的属性名和属性值,修改Set方法的IL实现
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="isSavaDynamicModule"></param>
        /// <returns></returns>
        public static object CreateDynamicProxy(Type t)
        {
            string nameOfAssembly = t.Name + "ProxyAssembly";
            string nameOfModule = t + "ProxyModule";
            string nameOfType = t + "Proxy";

            Type modifiedPropertyNamesType = typeof(HashSet<string>);
            Type orgmodelType = t;

            Type typeNeedProxy = t;
            AssemblyBuilder assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName(nameOfAssembly), AssemblyBuilderAccess.Run);
            //动态创建模块
            ModuleBuilder moduleBuilder = DefineDynamicModule(assemblyBuilder);
            string proxyClassName = string.Format(ProxyDefineConst.ProxyClassNameFormater, typeNeedProxy.Name);
            //动态创建类代理
            TypeBuilder typeBuilderProxy = moduleBuilder.DefineType(proxyClassName, TypeAttributes.Public, typeNeedProxy);
            //定义一个变量存放属性变更名
            FieldBuilder fbModifiedPropertyNames = typeBuilderProxy.DefineField(ProxyDefineConst.ModifiedPropertyNamesFieldName, modifiedPropertyNamesType, FieldAttributes.Public);
            FieldBuilder orgmodelPropertyNames = typeBuilderProxy.DefineField(ProxyDefineConst.OrgmodelFiledName, orgmodelType, FieldAttributes.Public);

            /*
             * 构造函数 实例化 ModifiedPropertyNames,生成类似于下面的代码
               ModifiedPropertyNames = new List<string>();
            */
            ConstructorBuilder constructorBuilder = typeBuilderProxy.DefineConstructor(MethodAttributes.Public, CallingConventions.Standard, null);
            ILGenerator ilgCtor = constructorBuilder.GetILGenerator();
            ilgCtor.Emit(OpCodes.Ldarg_0);//加载当前类
            ilgCtor.Emit(OpCodes.Newobj, modifiedPropertyNamesType.GetConstructor(new Type[0]));//实例化对象入栈
            ilgCtor.Emit(OpCodes.Stfld, fbModifiedPropertyNames);//设置fbModifiedPropertyNames值,为刚入栈的实例化对象
            ilgCtor.Emit(OpCodes.Ldarg_0);
            ilgCtor.Emit(OpCodes.Newobj, orgmodelType.GetConstructor(new Type[0]));//实例化对象入栈
            ilgCtor.Emit(OpCodes.Stfld, orgmodelPropertyNames);//,为刚入栈的实例化对

            ilgCtor.Emit(OpCodes.Ret);//返回

            //获取被代理对象的所有属性,循环属性进行重写
            PropertyInfo[] properties = typeNeedProxy.GetProperties();
            foreach (PropertyInfo propertyInfo in properties)
            {


                string propertyName = propertyInfo.Name;
                Type typePepropertyInfo = propertyInfo.PropertyType;
                //动态创建字段和属性
                FieldBuilder fieldBuilder = typeBuilderProxy.DefineField("_" + propertyName, typePepropertyInfo, FieldAttributes.Private);
                PropertyBuilder propertyBuilder = typeBuilderProxy.DefineProperty(propertyName, PropertyAttributes.SpecialName, typePepropertyInfo, null);

                //重写属性的Get Set方法
                var methodGet = typeBuilderProxy.DefineMethod("get_" + propertyName, GetSetMethodAttributes, typePepropertyInfo, Type.EmptyTypes);
                var methodSet = typeBuilderProxy.DefineMethod("set_" + propertyName, GetSetMethodAttributes, null, new Type[] { typePepropertyInfo });

                //il of get method
                var ilGetMethod = methodGet.GetILGenerator();
                ilGetMethod.Emit(OpCodes.Ldarg_0);
                ilGetMethod.Emit(OpCodes.Ldfld, fieldBuilder);
                ilGetMethod.Emit(OpCodes.Ret);
                //il of set method
                ILGenerator ilSetMethod = methodSet.GetILGenerator();
                ilSetMethod.Emit(OpCodes.Ldarg_0);
                ilSetMethod.Emit(OpCodes.Ldarg_1);
                ilSetMethod.Emit(OpCodes.Stfld, fieldBuilder);
                ilSetMethod.Emit(OpCodes.Ldarg_0);
                ilSetMethod.Emit(OpCodes.Ldfld, fbModifiedPropertyNames);
                ilSetMethod.Emit(OpCodes.Ldstr, propertyInfo.Name);



                ilSetMethod.Emit(OpCodes.Callvirt, modifiedPropertyNamesType.GetMethod("Add", new Type[] { typeof(string) }));
                ilSetMethod.Emit(OpCodes.Pop);
                ilSetMethod.Emit(OpCodes.Ret);

                //设置属性的Get Set方法
                propertyBuilder.SetGetMethod(methodGet);
                propertyBuilder.SetSetMethod(methodSet);
            }

            //使用动态类创建类型
            TypeInfo proxyClassType = typeBuilderProxy.CreateTypeInfo();
            //创建类实例
            var instance = Activator.CreateInstance(proxyClassType);
            return instance;
        }

    }



}
