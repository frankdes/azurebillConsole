using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace azurebill
{
    public class TreeFlowParam : ITreeFlowParam
    {
        public ConcurrentDictionary<string, object> data { get; set; }
        public Dictionary<string, object> securityData { get; set; }

        public bool IsException { get; set; }

        public List<Exception> exceptions { get; set; }

        public TreeFlowParam()
        {
            data = new ConcurrentDictionary<string, object>();
            securityData = new Dictionary<string, object>();
            exceptions = new List<Exception>();
            IsException = false;
        }
        
        public void AddException(Exception exception)
        {
            IsException = true;
            exceptions.Add(exception);
        }
    }
}
