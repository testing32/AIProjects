using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    public class EntityManager
    {
        #region Singleton
        private static object _lock = new object();
        private static EntityManager _instance;
        private EntityManager() {}
        public static EntityManager TheInstance
        {
            get
            {
                if (_instance == null)
                    lock (_lock)
                        if (_instance == null)
                            _instance = new EntityManager();

                return _instance;
            }
        }
        #endregion
    }
}
