using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CleanRx
{
    public class PresentationBinding
    {
        readonly Dictionary<int, object> _presenterByModel;
        readonly Dictionary<int, object> _modelByPresenter;

        public PresentationBinding()
        {
            _presenterByModel = new Dictionary<int, object>();
            _modelByPresenter = new Dictionary<int, object>();
        }

        public void Bind(Entity entity, object presenter)
        {
            _presenterByModel.Add(entity.GetHashCode(), presenter);
            _modelByPresenter.Add(entity.GetHashCode(), presenter);
        }
    }
}
