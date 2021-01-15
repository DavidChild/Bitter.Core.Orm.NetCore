using System;
using System.Collections;

namespace BT.Manage.Core
{
    public interface IEntityOperator
    {
        void AddEditing(IList list);
        void ClearAdding();
        void ClearEditing();
        void ClearRemoving();
        ArrayList GetAdding();
        IList GetEditing();
        Type GetEntityType();
        IList GetRemoving();
    }
}