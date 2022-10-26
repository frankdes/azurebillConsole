using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azurebill
{
    public interface ITreeFlowParam
    {
        public ConcurrentDictionary<string, object> data { get; set; }
        public Dictionary<string, object> securityData { get; set; }

        public bool IsException { get; set; }

        public List<Exception> exceptions { get; set; }

        public void AddException(Exception exception);
      

    }
}
