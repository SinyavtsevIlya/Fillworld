using System;
using System.Collections.Generic;
using System.Linq;
using UniRx;

namespace CleanRx
{
    [Serializable]
    public class Entity
    {
        public int ID;
        public int Generation;
        public bool IsReserved;
        public readonly Context Context;
        internal readonly List<Group> Groups = new List<Group>();

        readonly CompositeDisposable _lifecycle;
        readonly ReactiveDictionary<int, object> _datas = new ReactiveDictionary<int, object>();

        object _hasDataCache;

        public Entity(Context context)
        {
            Context = context;
            _lifecycle = new CompositeDisposable();
        }

        public CompositeDisposable Lifecycle => _lifecycle;

        public ReactiveDictionary<int, object> Composition => _datas;

        public Action OnDestroy = () => { };

        public T Add<T>() where T : class, new()
        {
            var instance = System.Activator.CreateInstance<T>();
            if (Has<T>()) Remove<T>();
            var dataIndex = typeof(T).GetHashCode();
            _datas.Add(dataIndex, instance);
            Context.FilterEntityOnDataChange(this, dataIndex, true);
            return instance;
        }

        public T Get<T>() where T : class, new()
        {
            if (Has<T>())
            {
                return _datas[typeof(T).GetHashCode()] as T;
            }
            throw new Exception($"Entity doesn't have a {typeof(T)} data");
        }

        public Entity Remove<T>() where T : new()
        {
            if (!Has<T>()) return this;
            var dataIndex = typeof(T).GetHashCode();
            return Remove(dataIndex);
        }

        Entity Remove(int dataIndex)
        {
            _datas.Remove(dataIndex);
            Context.FilterEntityOnDataChange(this, dataIndex, false);
            return this;
        }

        public void RemoveAll()
        {
            Context.FilterEntityOnCreateOrDestroy(this, false);
            _datas.Clear();
        }

        public Entity Toggle<T>() where T : class, new()
        {
            if (Has<T>())
            {
                Remove<T>();
            }
            else
            {
                Add<T>();
            }

            return this;
        }

        public IObservable<bool> HasAsObservable<T>() where T : new()
        {
            IObservable<bool> addObservable = _datas.ObserveAdd().Select(d => d.Key == typeof(T).GetHashCode()).Where(isTrue => isTrue);
            IObservable<bool> removeObservable = _datas.ObserveRemove().Select(d => d.Key == typeof(T).GetHashCode()).Where(isTrue => isTrue);

            var result = Observable.Merge(addObservable, removeObservable.Select(isTrue => !isTrue));
            result.Publish();
            return result;
        }

        public bool Has<T>() where T : new()
        {
            object hasDataCache;
            return _datas.TryGetValue(typeof(T).GetHashCode(), out hasDataCache);
        }

        internal bool Has(int dataIndex)
        {
            object hasDataCache;
            return _datas.TryGetValue(dataIndex, out hasDataCache);
        }

        public string CompositionFormated
        {
            get
            {
                return $"Entity {ID} Composition: " + _datas.Keys
                    .Select(d => $"<b>{EcsTypesHelper.GetTypeByHashCode(d)}</b>")
                    .Aggregate((a, b) => a + ", " + b);
            }
        }

        public override string ToString()
        {
            return "Entity:" + ID;
        }
    }
}
