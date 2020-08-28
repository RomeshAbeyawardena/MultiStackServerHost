using MultiStackServiceHost.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MultiStackServiceHost.Services
{
    public class Attempt : IAttempt
    {
        public static IAttempt Create(
            Action methodToAttempt, 
            Action<IDescriptor<Type>> describeTypes,
            Action finallyDelegate = null)
        {
            var exceptionTypeDescriber = new Descriptor<Type>();
            describeTypes(exceptionTypeDescriber);
            try
            {
                methodToAttempt();
                return new Attempt(true);
            }
            catch(Exception exception)
            {
                if (exceptionTypeDescriber.Described.Contains(exception.GetType()))
                {
                    return new Attempt(exception);
                }

                throw;
            }
            finally
            {
                finallyDelegate?.Invoke();
            }
        }

        public static IAttempt<T> Create<T>(
            Func<T> methodToAttempt, 
            Action<IDescriptor<Type>> describeTypes,
            Action finallyDelegate = null)
            where T: class
        {
            var exceptionTypeDescriber = new Descriptor<Type>();
            describeTypes(exceptionTypeDescriber);
            try
            {
                return Create(methodToAttempt());
            }
            catch(Exception exception)
            {
                if (exceptionTypeDescriber.Described.Contains(exception.GetType()))
                {
                    return Create<T>(exception);
                }

                throw;
            }
            finally
            {
                finallyDelegate?.Invoke();
            }
        }

        public static IAttempt<T> Create<T>(T result)
            where T: class
        {
            return new Attempt<T>(result);
        }

        public static IAttempt<T> Create<T>(Exception exception)
            where T: class
        {
            return new Attempt<T>(exception);
        }

        public Attempt(bool successful)
        {
            Successful = successful;
        }

        public Attempt(object result)
        {
            Result = result;
            Successful = result != null;
        }

        public Attempt(Exception exception)
        {
            Exception = exception;
        }

        public Exception Exception { get; }

        public bool Successful { get; }

        public object Result { get; }
    }

    public class Attempt<T> : Attempt, IAttempt<T>
        where T: class
    {
        public Attempt(T result) : base(result)
        {
        }

        public Attempt(Exception exception) : base(exception)
        {
        }

        T IAttempt<T>.Result => Result as T;
    }
}
