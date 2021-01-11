using BT.Manage.DataAccess;

namespace BT.Manage.Core
{
    internal interface IDataBaseProperty
    {
        DatabaseProperty conn { get; }
        string prefix { get; }
    }
}