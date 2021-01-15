using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Linq;
namespace Bitter.Core
{
    internal class ExcutParBag_Update: ExcutParBag
    {

        public ExcutParBag_Update()
        {
            _updatePair = new List<UpdatePair>();
        }
        public void SetUpdatePair(UpdatePair updatePair)
        {

            _updatePair.Add(updatePair);
        }
        public LambdaExpression condition { get; set; }

        private bool isReSetValueInUpdatePair { get; set; } = false;


        private List<UpdatePair> _updatePair { get; set; }
        public List<UpdatePair> updatePair
        { 
            get
            {
                if (this.data != null && (!isReSetValueInUpdatePair))
                {
                    var bl = false;
                    if (_updatePair.Count == 0)
                    {
                        bl = true;
                    }
                    foreach (var p in this.PropertyFileds)
                    {
                        if (bl)
                        {
                            if (p.isIdentity) continue;
                            if (p.isKey) continue;
                            //加入到updatePair
                            _updatePair.Add(new UpdatePair() { columnName = p.filedName, columnValue = p.value });
                        }
                        else
                        {
                            //更新
                            var k = _updatePair.Where(x => x.columnName == p.filedName);
                            if (k != null && k.Count() > 0)
                            {
                                k.ToList().ForEach(z =>
                                {
                                    z.columnValue = p.value;
                                });
                            }
                        }
                       

                    };

                    isReSetValueInUpdatePair = true;
                }
                return _updatePair;
            }
        }
    }
}
