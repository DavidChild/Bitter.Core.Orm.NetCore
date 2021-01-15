using System;
using System.Collections.Generic;
using System.Text;
using Bitter.Tools.Utils;
using Bitter.Tools;

namespace Bitter.Core
{
   internal static class BaseQueryExtend
    {
        internal static BaseQuery MapToExutQuery(this BaseQuery o)
        {

            BaseQuery vdb = new BaseQuery();
            switch (o.databaseType)
            {

                case DatabaseType.MSSQLServer:
                    vdb= o.MapTo<MsSqlQuery>( new Action<AutoMapper.IMapperConfigurationExpression>(cfg=> { cfg.ShouldMapProperty = pi => pi.CanWrite; })  );
                 break;
                case DatabaseType.MySql:
                    vdb=o.MapTo<MySqlQuery>(new Action<AutoMapper.IMapperConfigurationExpression>(cfg => { cfg.ShouldMapProperty = pi => pi.CanWrite; }));
                break;
                default:
                    vdb=o.MapTo<MsSqlQuery>(new Action<AutoMapper.IMapperConfigurationExpression>(cfg => { cfg.ShouldMapProperty = pi => pi.CanWrite; }));
                break;

            }

            return vdb;


        }
    }
}
