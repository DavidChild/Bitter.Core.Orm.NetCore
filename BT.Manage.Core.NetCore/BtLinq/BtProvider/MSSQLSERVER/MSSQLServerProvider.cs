using BT.Manage.Core;


namespace BT.Manage.Core.Provider.MSSQLServer
{

    
    public class MSSQLServerProvider : ProviderBase
    {
        public override IModelOprator CreateEntityOperator()
        {
          
            return   new ModelOperator();
        }


       
    }
}

