using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HomeworkTwo
{
    abstract public class State<T>
    {
        public abstract void Enter(T entity);
        public abstract void Execute(T entity);
        public abstract void Exit(T entity);
        public abstract bool OnMessage(T entity, Telegram telegram);
    }
}
